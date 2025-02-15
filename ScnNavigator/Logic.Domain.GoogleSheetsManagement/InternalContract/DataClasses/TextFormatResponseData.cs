namespace Logic.Domain.GoogleSheetsManagement.InternalContract.DataClasses
{
    internal class TextFormatResponseData
    {
        public ColorResponseData ForeGroundColor { get; set; }
        public string FontFamily { get; set; }
        public int FontSize { get; set; }
        public bool Bold { get; set; }
        public bool Italic { get; set; }
        public bool StrikeThrough { get; set; }
        public bool Underline { get; set; }
    }
}
