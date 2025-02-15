using Logic.Domain.GoogleSheetsManagement.Contract.Aspects;

namespace Logic.Business.TranslationManagement.InternalContract.DataClasses
{
    internal class RouteTextRangeData
    {
        [Column("B")]
        public string Text { get; set; }
    }
}
