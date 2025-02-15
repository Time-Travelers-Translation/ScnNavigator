using CrossCutting.Abstract.DataClasses;
using Logic.Business.TranslationManagement.Contract;
using Logic.Business.TranslationManagement.InternalContract.DataClasses;
using Logic.Domain.GoogleSheetsManagement.Contract.DataClasses;
using Logic.Domain.GoogleSheetsManagement.Contract;

namespace Logic.Business.TranslationManagement
{
    internal class TutorialTranslationManager : ITutorialTranslationManager
    {
        private const int TutorialEndRow_ = 37;
        private const string TableName_ = "Tutorial";

        private readonly ISheetManager _sheetManager;

        private IDictionary<int, TranslatedTextData<TextData>>? _tutorialTitlesLookup;
        private IDictionary<int, IList<TranslatedTextData<TextData>>>? _tutorialTextsLookup;

        public TutorialTranslationManager(TranslationManagementConfiguration config, IGoogleApiConnector apiConnector)
        {
            _sheetManager = apiConnector.CreateSheetManager(config.SheetId);
        }

        public async Task<TextData?> GetTutorialTitle(int index)
        {
            if (_tutorialTitlesLookup != null)
            {
                if (_tutorialTitlesLookup.TryGetValue(index, out TranslatedTextData<TextData>? title))
                    return title.Text;

                return null;
            }

            await PopulateTutorialData();

            if (_tutorialTitlesLookup == null)
                return null;

            if (!_tutorialTitlesLookup!.TryGetValue(index, out TranslatedTextData<TextData>? translatedTitle))
                return null;

            return translatedTitle.Text;
        }

        public async Task<TextData[]?> GetTutorialTexts(int index)
        {
            if (_tutorialTextsLookup != null)
            {
                if (_tutorialTextsLookup.TryGetValue(index, out IList<TranslatedTextData<TextData>>? text))
                    return text.Select(t => t.Text).ToArray();

                return null;
            }

            await PopulateTutorialData();

            if (_tutorialTextsLookup == null)
                return null;

            if (!_tutorialTextsLookup!.TryGetValue(index, out IList<TranslatedTextData<TextData>>? translatedText))
                return null;

            return translatedText.Select(t => t.Text).ToArray();
        }

        public async Task UpdateTutorials(int[] indexes)
        {
            if (_tutorialTextsLookup == null || _tutorialTitlesLookup == null)
                return;

            var tutorials = new List<(int row, TranslatedTextData<TextData> title, IList<TranslatedTextData<TextData>> texts)>();
            foreach (int index in indexes)
            {
                bool hasTitle = _tutorialTitlesLookup.TryGetValue(index, out TranslatedTextData<TextData>? title);
                bool hasTexts = _tutorialTextsLookup.TryGetValue(index, out IList<TranslatedTextData<TextData>>? texts);

                if (!hasTitle && !hasTexts)
                    continue;

                if (texts!.Count == 0)
                    continue;

                if (title!.Row != texts[0].Row)
                    continue;

                tutorials.Add((title.Row, title, texts));
            }

            var ranges = new List<IList<UpdateTutorialTextRangeData>>();
            foreach ((int row, TranslatedTextData<TextData> title, IList<TranslatedTextData<TextData>> texts) tutorial in tutorials.OrderBy(x => x.row))
            {
                if (ranges.Count <= 0 || ranges[^1][^1].Row + 1 != tutorial.row)
                    ranges.Add(new List<UpdateTutorialTextRangeData>());

                ranges[^1].Add(new UpdateTutorialTextRangeData
                {
                    Row = tutorial.row,
                    TranslatedTitle = tutorial.title.Text.Text,
                    Translation = string.Join("\n\n", tutorial.texts.Select(t => t.Text.Text))
                });
            }

            foreach (IList<UpdateTutorialTextRangeData> textRange in ranges)
            {
                if (textRange.Count <= 0)
                    continue;

                CellIdentifier titleCellStart = CellIdentifier.Parse($"C{textRange[0].Row}");
                CellIdentifier titleCellEnd = CellIdentifier.Parse($"C{textRange[^1].Row}");

                await _sheetManager.UpdateRangeAsync(textRange, TableName_, titleCellStart, titleCellEnd);

                CellIdentifier textCellStart = CellIdentifier.Parse($"F{textRange[0].Row}");
                CellIdentifier textCellEnd = CellIdentifier.Parse($"F{textRange[^1].Row}");

                await _sheetManager.UpdateRangeAsync(textRange, TableName_, textCellStart, textCellEnd);
            }
        }

        private async Task PopulateTutorialData()
        {
            CellIdentifier startCell = CellIdentifier.Parse("C2");
            CellIdentifier endCell = CellIdentifier.Parse($"F{TutorialEndRow_}");

            TutorialTextRangeData[]? range = await _sheetManager.GetRangeAsync<TutorialTextRangeData>(TableName_, startCell, endCell);
            if (range == null)
                return;

            _tutorialTitlesLookup = new Dictionary<int, TranslatedTextData<TextData>>();
            for (var i = 0; i < range.Length; i++)
            {
                _tutorialTitlesLookup[i] = new TranslatedTextData<TextData>
                {
                    Row = i + 2,
                    Text = new TextData
                    {
                        Name = $"Title{i + 1:000}",
                        Text = range[i].TranslatedTitle
                    }
                };
            }

            _tutorialTextsLookup = new Dictionary<int, IList<TranslatedTextData<TextData>>>();
            for (var i = 0; i < range.Length; i++)
            {
                _tutorialTextsLookup[i] = new List<TranslatedTextData<TextData>>();

                string[] translatedTexts = range[i].Translation?.Split("\n\n") ?? Array.Empty<string>();
                if (translatedTexts.Length <= 0)
                {
                    _tutorialTextsLookup[i].Add(new TranslatedTextData<TextData>
                    {
                        Row = i + 2,
                        Text = new TextData
                        {
                            Name = $"Text{i + 1:000}_00",
                            Text = string.Empty
                        }
                    });
                    continue;
                }

                for (var j = 0; j < translatedTexts.Length; j++)
                {
                    _tutorialTextsLookup[i].Add(new TranslatedTextData<TextData>
                    {
                        Row = i + 2,
                        Text = new TextData
                        {
                            Name = $"Text{i + 1:000}_{j:00}",
                            Text = translatedTexts[j]
                        }
                    });
                }
            }
        }
    }
}
