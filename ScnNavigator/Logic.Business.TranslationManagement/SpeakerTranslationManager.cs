using CrossCutting.Abstract.DataClasses;
using Logic.Business.TranslationManagement.Contract;
using Logic.Business.TranslationManagement.InternalContract.DataClasses;
using Logic.Domain.GoogleSheetsManagement.Contract;
using Logic.Domain.GoogleSheetsManagement.Contract.DataClasses;

namespace Logic.Business.TranslationManagement
{
    internal class SpeakerTranslationManager : ISpeakerTranslationManager
    {
        private const int SpeakerEndRow_ = 115;
        private const string TableName_ = "Names";

        private readonly ISheetManager _sheetManager;

        private IDictionary<string, TranslatedTextData<TextData>>? _speakerNameLookup;

        public SpeakerTranslationManager(TranslationManagementConfiguration config, IGoogleApiConnector apiConnector)
        {
            _sheetManager = apiConnector.CreateSheetManager(config.SheetId);
        }

        public async Task<TextData?> GetSpeaker(string speaker)
        {
            if (_speakerNameLookup == null)
            {
                CellIdentifier startCell = CellIdentifier.Parse("A2");
                CellIdentifier endCell = CellIdentifier.Parse($"C{SpeakerEndRow_}");

                SpeakerNameRangeData[]? range = await _sheetManager.GetRangeAsync<SpeakerNameRangeData>(TableName_, startCell, endCell);
                if (range == null)
                    return null;

                _speakerNameLookup ??= new Dictionary<string, TranslatedTextData<TextData>>();
                for (var i = 0; i < range.Length; i++)
                {
                    if (_speakerNameLookup.ContainsKey(range[i].OriginalName))
                        continue;

                    _speakerNameLookup[range[i].OriginalName] = new TranslatedTextData<TextData>
                    {
                        Row = i + 4,
                        Text = new TextData
                        {
                            Name = range[i].OriginalName,
                            Text = range[i].TranslatedName
                        }
                    };
                }
            }

            if (_speakerNameLookup.TryGetValue(speaker, out TranslatedTextData<TextData>? translatedSpeaker))
                return translatedSpeaker.Text;

            return null;
        }

        public async Task UpdateSpeakers(string[] speakers)
        {
            if (_speakerNameLookup == null)
                return;

            var names = new List<TranslatedTextData<TextData>>();
            foreach (string speaker in speakers)
            {
                if (!_speakerNameLookup.TryGetValue(speaker, out TranslatedTextData<TextData>? translatedSpeaker))
                    continue;

                names.Add(translatedSpeaker);
            }

            var ranges = new List<IList<TranslatedTextData<TextData>>>();
            foreach (TranslatedTextData<TextData> translatedSpeaker in names.OrderBy(x => x.Row))
            {
                if (ranges.Count <= 0 || ranges[^1][^1].Row + 1 != translatedSpeaker.Row)
                    ranges.Add(new List<TranslatedTextData<TextData>>());

                ranges[^1].Add(translatedSpeaker);
            }

            foreach (IList<TranslatedTextData<TextData>> textRange in ranges)
            {
                if (textRange.Count <= 0)
                    continue;

                UpdateSpeakerNameRangeData[] updateRange = textRange.Select(r => new UpdateSpeakerNameRangeData
                {
                    TranslatedName = r.Text.Text
                }).ToArray();

                CellIdentifier textCellStart = CellIdentifier.Parse($"C{textRange[0].Row}");
                CellIdentifier textCellEnd = CellIdentifier.Parse($"C{textRange[^1].Row}");

                await _sheetManager.UpdateRangeAsync(updateRange, TableName_, textCellStart, textCellEnd);
            }
        }
    }
}
