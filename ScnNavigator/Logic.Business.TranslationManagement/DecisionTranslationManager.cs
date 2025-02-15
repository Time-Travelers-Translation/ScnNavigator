using CrossCutting.Abstract.DataClasses;
using Logic.Business.TranslationManagement.Contract;
using Logic.Business.TranslationManagement.InternalContract.DataClasses;
using Logic.Domain.GoogleSheetsManagement.Contract;
using Logic.Domain.GoogleSheetsManagement.Contract.DataClasses;

namespace Logic.Business.TranslationManagement
{
    internal class DecisionTranslationManager : IDecisionTranslationManager
    {
        private const int DecisionEndRow_ = 222;
        private const string TableName_ = "Decisions";

        private readonly ISheetManager _sheetManager;

        private readonly IDictionary<string, IList<TranslatedTextData<TextData>>> _sceneDecisionLookup;

        public DecisionTranslationManager(TranslationManagementConfiguration config, IGoogleApiConnector apiConnector)
        {
            _sheetManager = apiConnector.CreateSheetManager(config.SheetId);

            _sceneDecisionLookup = new Dictionary<string, IList<TranslatedTextData<TextData>>>();
        }

        public async Task<TextData[]?> GetDecisions(string sceneName)
        {
            if (_sceneDecisionLookup.TryGetValue(sceneName, out IList<TranslatedTextData<TextData>>? texts))
                return texts.Select(x => x.Text).ToArray();

            CellIdentifier startCell = CellIdentifier.Parse("A2");
            CellIdentifier endCell = CellIdentifier.Parse($"D{DecisionEndRow_}");

            DecisionTextRangeData[]? range = await _sheetManager.GetRangeAsync<DecisionTextRangeData>(TableName_, startCell, endCell);
            if (range == null)
                return null;

            for (var i = 0; i < range.Length; i++)
            {
                if (!_sceneDecisionLookup.TryGetValue(range[i].SceneName, out texts))
                    _sceneDecisionLookup[range[i].SceneName] = texts = new List<TranslatedTextData<TextData>>();

                texts.Add(new TranslatedTextData<TextData>
                {
                    Row = i + 2,
                    Text = new TextData
                    {
                        Name = range[i].SceneName,
                        Text = range[i].Translation
                    }
                });
            }

            if (!_sceneDecisionLookup.TryGetValue(sceneName, out texts))
                return null;

            return texts.Select(x => x.Text).ToArray();
        }

        public Task UpdateDecisionText(string sceneName)
        {
            return UpdateDecisionText(new[] { sceneName });
        }

        public async Task UpdateDecisionText(string[] sceneNames)
        {
            var texts = new List<TranslatedTextData<TextData>>();
            foreach (string sceneName in sceneNames)
            {
                if (_sceneDecisionLookup.TryGetValue(sceneName, out IList<TranslatedTextData<TextData>>? sceneTexts))
                    texts.AddRange(sceneTexts);
            }

            var ranges = new List<IList<TranslatedTextData<TextData>>>();
            foreach (TranslatedTextData<TextData> translatedText in texts.OrderBy(x => x.Row))
            {
                if (ranges.Count <= 0 || ranges[^1][^1].Row + 1 != translatedText.Row)
                    ranges.Add(new List<TranslatedTextData<TextData>>());

                ranges[^1].Add(translatedText);
            }

            foreach (IList<TranslatedTextData<TextData>> textRange in ranges)
            {
                if (textRange.Count <= 0)
                    continue;

                UpdateDecisionTextRangeData[] updateRange = textRange.Select(r => new UpdateDecisionTextRangeData
                {
                    Translation = r.Text.Text
                }).ToArray();

                CellIdentifier textCellStart = CellIdentifier.Parse($"D{textRange[0].Row}");
                CellIdentifier textCellEnd = CellIdentifier.Parse($"D{textRange[^1].Row}");

                await _sheetManager.UpdateRangeAsync(updateRange, TableName_, textCellStart, textCellEnd);
            }
        }
    }
}
