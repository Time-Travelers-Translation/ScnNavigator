using CrossCutting.Abstract.DataClasses;
using Logic.Business.TranslationManagement.Contract;
using Logic.Business.TranslationManagement.InternalContract.DataClasses;
using Logic.Domain.GoogleSheetsManagement.Contract;
using Logic.Domain.GoogleSheetsManagement.Contract.DataClasses;

namespace Logic.Business.TranslationManagement
{
    internal class OutlineTranslationManager : IOutlineTranslationManager
    {
        private const int OutlineEndRow_ = 63;
        private const string TableName_ = "Outlines";

        private readonly ISheetManager _sheetManager;

        private IDictionary<string, IList<TranslatedTextData<TextData>>>? _routeOutlineLookup;

        public OutlineTranslationManager(TranslationManagementConfiguration config, IGoogleApiConnector apiConnector)
        {
            _sheetManager = apiConnector.CreateSheetManager(config.SheetId);
        }

        public async Task<TextData[]?> GetOutlines(string routeLetter)
        {
            if (_routeOutlineLookup != null)
            {
                if (_routeOutlineLookup.TryGetValue(routeLetter, out IList<TranslatedTextData<TextData>>? texts))
                    return texts.Select(x => x.Text).ToArray();

                return null;
            }

            CellIdentifier startCell = CellIdentifier.Parse("A2");
            CellIdentifier endCell = CellIdentifier.Parse($"D{OutlineEndRow_}");

            OutlineTextRangeData[]? range = await _sheetManager.GetRangeAsync<OutlineTextRangeData>(TableName_, startCell, endCell);
            if (range == null)
                return null;

            _routeOutlineLookup = new Dictionary<string, IList<TranslatedTextData<TextData>>>();
            for (var i = 0; i < range.Length; i++)
            {
                if (!_routeOutlineLookup.TryGetValue(range[i].Route, out IList<TranslatedTextData<TextData>>? routeTexts))
                    _routeOutlineLookup[range[i].Route] = routeTexts = new List<TranslatedTextData<TextData>>();

                routeTexts.Add(new TranslatedTextData<TextData>
                {
                    Row = i + 2,
                    Text = new TextData
                    {
                        Name = range[i].Route,
                        Text = range[i].Translation
                    }
                });
            }

            if (!_routeOutlineLookup.TryGetValue(routeLetter, out IList<TranslatedTextData<TextData>>? translatedTexts))
                return null;

            return translatedTexts.Select(x => x.Text).ToArray();
        }

        public async Task UpdateOutlines(string routeLetter)
        {
            if (_routeOutlineLookup == null)
                return;

            if (!_routeOutlineLookup.TryGetValue(routeLetter, out IList<TranslatedTextData<TextData>>? texts))
                return;

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

                UpdateOutlineTextRangeData[] updateRange = textRange.Select(r => new UpdateOutlineTextRangeData
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
