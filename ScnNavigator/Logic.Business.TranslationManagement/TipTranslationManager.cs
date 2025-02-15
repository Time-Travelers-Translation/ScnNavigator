using CrossCutting.Abstract.DataClasses;
using Logic.Business.TranslationManagement.Contract;
using Logic.Business.TranslationManagement.InternalContract.DataClasses;
using Logic.Domain.GoogleSheetsManagement.Contract;
using Logic.Domain.GoogleSheetsManagement.Contract.DataClasses;

namespace Logic.Business.TranslationManagement
{
    internal class TipTranslationManager : ITipTranslationManager
    {
        private const int TipEndRow_ = 448;
        private const string TableName_ = "TIPS";

        private readonly ISheetManager _sheetManager;

        private IDictionary<int, TranslatedTextData<TextData>>? _tipTitlesLookup;
        private IDictionary<int, TranslatedTextData<TextData>>? _tipTextsLookup;

        public TipTranslationManager(TranslationManagementConfiguration config, IGoogleApiConnector apiConnector)
        {
            _sheetManager = apiConnector.CreateSheetManager(config.SheetId);
        }

        public async Task<TextData?> GetTipTitle(int index)
        {
            if (_tipTitlesLookup != null)
            {
                if (_tipTitlesLookup.TryGetValue(index, out TranslatedTextData<TextData>? title))
                    return title.Text;

                return null;
            }

            await PopulateTipData();

            if (_tipTitlesLookup == null)
                return null;

            if (!_tipTitlesLookup!.TryGetValue(index, out TranslatedTextData<TextData>? translatedTitle))
                return null;

            return translatedTitle.Text;
        }

        public async Task<TextData?> GetTipText(int index)
        {
            if (_tipTextsLookup != null)
            {
                if (_tipTextsLookup.TryGetValue(index, out TranslatedTextData<TextData>? text))
                    return text.Text;

                return null;
            }

            await PopulateTipData();

            if (_tipTextsLookup == null)
                return null;

            if (!_tipTextsLookup!.TryGetValue(index, out TranslatedTextData<TextData>? translatedText))
                return null;

            return translatedText.Text;
        }

        public async Task UpdateTips(int[] indexes)
        {
            if (_tipTextsLookup == null || _tipTitlesLookup == null)
                return;

            var tips = new List<(int row, TranslatedTextData<TextData> title, TranslatedTextData<TextData> text)>();
            foreach (int index in indexes)
            {
                bool hasTitle = _tipTitlesLookup.TryGetValue(index, out TranslatedTextData<TextData>? title);
                bool hasText = _tipTextsLookup.TryGetValue(index, out TranslatedTextData<TextData>? text);

                if (!hasTitle && !hasText)
                    continue;

                if (title!.Row != text!.Row)
                    continue;

                tips.Add((title.Row, title, text));
            }

            var ranges = new List<IList<UpdateTipTextRangeData>>();
            foreach ((int row, TranslatedTextData<TextData> title, TranslatedTextData<TextData> text) tip in tips.OrderBy(x => x.row))
            {
                if (ranges.Count <= 0 || ranges[^1][^1].Row + 1 != tip.row)
                    ranges.Add(new List<UpdateTipTextRangeData>());

                ranges[^1].Add(new UpdateTipTextRangeData
                {
                    Row = tip.row,
                    TranslatedTitle = tip.title.Text.Text,
                    Translation = tip.text.Text.Text,
                });
            }

            foreach (IList<UpdateTipTextRangeData> textRange in ranges)
            {
                if (textRange.Count <= 0)
                    continue;

                CellIdentifier titleCellStart = CellIdentifier.Parse($"H{textRange[0].Row}");
                CellIdentifier titleCellEnd = CellIdentifier.Parse($"H{textRange[^1].Row}");

                await _sheetManager.UpdateRangeAsync(textRange, TableName_, titleCellStart, titleCellEnd);

                CellIdentifier textCellStart = CellIdentifier.Parse($"K{textRange[0].Row}");
                CellIdentifier textCellEnd = CellIdentifier.Parse($"K{textRange[^1].Row}");

                await _sheetManager.UpdateRangeAsync(textRange, TableName_, textCellStart, textCellEnd);
            }
        }

        private async Task PopulateTipData()
        {
            CellIdentifier startCell = CellIdentifier.Parse("A2");
            CellIdentifier endCell = CellIdentifier.Parse($"K{TipEndRow_}");

            TipTextRangeData[]? range = await _sheetManager.GetRangeAsync<TipTextRangeData>(TableName_, startCell, endCell);
            if (range == null)
                return;

            _tipTitlesLookup = new Dictionary<int, TranslatedTextData<TextData>>();
            for (var i = 0; i < range.Length; i++)
            {
                _tipTitlesLookup[i + 1] = new TranslatedTextData<TextData>
                {
                    Row = i + 2,
                    Text = new TextData
                    {
                        Name = $"Title{range[i].Index:000}",
                        Text = range[i].TranslatedTitle
                    }
                };
            }

            _tipTextsLookup = new Dictionary<int, TranslatedTextData<TextData>>();
            for (var i = 0; i < range.Length; i++)
            {
                _tipTextsLookup[i + 1] = new TranslatedTextData<TextData>
                {
                    Row = i + 2,
                    Text = new TextData
                    {
                        Name = $"Text{range[i].Index:000}",
                        Text = range[i].Translation
                    }
                };
            }
        }
    }
}
