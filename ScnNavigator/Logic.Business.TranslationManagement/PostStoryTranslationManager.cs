using CrossCutting.Abstract.DataClasses;
using Logic.Business.TranslationManagement.Contract;
using Logic.Business.TranslationManagement.InternalContract.DataClasses;
using Logic.Domain.GoogleSheetsManagement.Contract;
using Logic.Domain.GoogleSheetsManagement.Contract.DataClasses;

namespace Logic.Business.TranslationManagement
{
    internal class PostStoryTranslationManager : IPostStoryTranslationManager
    {
        private const int PostTextEndRow_ = 26;
        private const string TableName_ = "Story";

        private readonly ISheetManager _sheetManager;

        private IDictionary<string, IDictionary<int, IList<TranslatedTextData<TextData>>>>? _postTextLookup;
        private IDictionary<string, IDictionary<int, IList<TranslatedTextData<TextData>>>>? _postDecisionLookup;

        public PostStoryTranslationManager(TranslationManagementConfiguration config, IGoogleApiConnector apiConnector)
        {
            _sheetManager = apiConnector.CreateSheetManager(config.PostSheetId);
        }

        public async Task<TextData[]?> GetPostDecisions(string name, int id)
        {
            if (_postDecisionLookup == null)
            {
                await PopulatePostTexts();

                if (_postDecisionLookup == null)
                    return null;
            }

            if (!_postDecisionLookup.TryGetValue(name, out IDictionary<int, IList<TranslatedTextData<TextData>>>? fileTexts))
                return null;

            if (!fileTexts.TryGetValue(id, out IList<TranslatedTextData<TextData>>? texts))
                return null;

            return texts.Select(x => x.Text).ToArray();
        }

        private async Task PopulatePostTexts()
        {
            CellIdentifier startCell = CellIdentifier.Parse("A2");
            CellIdentifier endCell = CellIdentifier.Parse($"G{PostTextEndRow_}");

            PostTextRangeData[]? range = await _sheetManager.GetRangeAsync<PostTextRangeData>(TableName_, startCell, endCell);
            if (range == null)
                return;

            _postTextLookup ??= new Dictionary<string, IDictionary<int, IList<TranslatedTextData<TextData>>>>();
            _postDecisionLookup ??= new Dictionary<string, IDictionary<int, IList<TranslatedTextData<TextData>>>>();

            for (var i = 0; i < range.Length; i++)
            {
                IDictionary<string, IDictionary<int, IList<TranslatedTextData<TextData>>>> lookup =
                    string.IsNullOrEmpty(range[i].DecisionMarker) ? _postTextLookup : _postDecisionLookup;

                if (!lookup.TryGetValue(range[i].Name, out IDictionary<int, IList<TranslatedTextData<TextData>>>? fileTexts))
                    lookup[range[i].Name] = fileTexts = new Dictionary<int, IList<TranslatedTextData<TextData>>>();

                if (!fileTexts.TryGetValue(range[i].Id, out IList<TranslatedTextData<TextData>>? texts))
                    fileTexts[range[i].Id] = texts = new List<TranslatedTextData<TextData>>();

                texts.Add(new TranslatedTextData<TextData>
                {
                    Row = i + 2,
                    Text = new TextData
                    {
                        Name = $"{range[i].Name}_{range[i].Id}",
                        Text = range[i].TranslatedText
                    }
                });
            }
        }
    }
}
