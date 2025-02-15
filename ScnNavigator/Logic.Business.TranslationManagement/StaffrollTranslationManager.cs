using CrossCutting.Abstract.DataClasses;
using Logic.Business.TranslationManagement.Contract;
using Logic.Business.TranslationManagement.InternalContract.DataClasses;
using Logic.Domain.GoogleSheetsManagement.Contract;
using Logic.Domain.GoogleSheetsManagement.Contract.DataClasses;

namespace Logic.Business.TranslationManagement
{
    internal class StaffrollTranslationManager : IStaffrollTranslationManager
    {
        private const int StaffRollEndRow_ = 636;
        private const string TableName_ = "Staffroll";

        private readonly ISheetManager _sheetManager;

        private TranslatedTextData<StaffrollTextData>[]? _staffRollLookup;

        public StaffrollTranslationManager(TranslationManagementConfiguration config, IGoogleApiConnector apiConnector)
        {
            _sheetManager = apiConnector.CreateSheetManager(config.SheetId);
        }

        public async Task<StaffrollTextData[]?> GetStaffRolls()
        {
            if (_staffRollLookup != null)
                return _staffRollLookup.Select(x => x.Text).ToArray();

            CellIdentifier startCell = CellIdentifier.Parse("A2");
            CellIdentifier endCell = CellIdentifier.Parse($"D{StaffRollEndRow_}");

            StaffRollTextRangeData[]? range = await _sheetManager.GetRangeAsync<StaffRollTextRangeData>(TableName_, startCell, endCell);
            if (range == null)
                return null;

            _staffRollLookup = new TranslatedTextData<StaffrollTextData>[range.Length];
            for (var i = 0; i < range.Length; i++)
            {
                _staffRollLookup[i] = new TranslatedTextData<StaffrollTextData>
                {
                    Row = i + 2,
                    Text = new StaffrollTextData
                    {
                        Name = $"{range[i].Hash}",
                        Hash = range[i].Hash,
                        Flag = range[i].ID,
                        Text = range[i].Translation
                    }
                };
            }

            return _staffRollLookup.Select(x => x.Text).ToArray();
        }

        public async Task UpdateStaffRoles()
        {
            if (_staffRollLookup == null)
                return;

            var updateRange = new List<UpdateStaffRollTextRangeData>();
            foreach (TranslatedTextData<StaffrollTextData> translatedText in _staffRollLookup.OrderBy(x => x.Row))
            {
                updateRange.Add(new UpdateStaffRollTextRangeData
                {
                    Translation = translatedText.Text.Text
                });
            }

            CellIdentifier textCellStart = CellIdentifier.Parse("D2");
            CellIdentifier textCellEnd = CellIdentifier.Parse($"D{StaffRollEndRow_}");

            await _sheetManager.UpdateRangeAsync(updateRange, TableName_, textCellStart, textCellEnd);
        }
    }
}
