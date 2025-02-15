using Logic.Domain.GoogleSheetsManagement.Contract.Aspects;

namespace Logic.Business.TranslationManagement.InternalContract.DataClasses
{
    internal class UpdateOutlineTextRangeData
    {
        [Column("E")]
        public string Translation { get; set; }
    }
}
