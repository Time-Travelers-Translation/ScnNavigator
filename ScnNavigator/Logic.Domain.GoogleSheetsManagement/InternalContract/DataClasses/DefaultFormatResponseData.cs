namespace Logic.Domain.GoogleSheetsManagement.InternalContract.DataClasses
{
    internal class DefaultFormatResponseData
    {
        public ColorResponseData BackgroundColor { get; set; }
        public PaddingResponseData Padding { get; set; }
        public string VerticalAlignment { get; set; }
        public string WrapStrategy { get; set; }
        public TextFormatResponseData TextFormat { get; set; }
    }
}
