namespace Logic.Domain.GoogleSheetsManagement.InternalContract.DataClasses
{
    internal class SheetObjectResponseData
    {
        public string SpreadSheetId { get; set; }
        public PropertiesResponseData Properties { get; set; }
        public List<SheetResponseData> Sheets { get; set; }
        public string SpreadsheetUrl { get; set; }
    }
}
