using Logic.Domain.GoogleSheetsManagement.Contract.DataClasses;

namespace Logic.Domain.GoogleSheetsManagement.Contract
{
    public interface ISheetManager
    {
        Task<SheetData[]?> GetSheetsAsync();
        Task<int?> GetSheetIdAsync(string sheetTitle);
        Task<TParse[]?> GetRangeAsync<TParse>(string sheetTitle, CellIdentifier start, CellIdentifier end);
        Task<bool> UpdateRangeAsync<TUpdate>(IList<TUpdate> updateData, string sheetTitle, CellIdentifier start, CellIdentifier end);
    }
}
