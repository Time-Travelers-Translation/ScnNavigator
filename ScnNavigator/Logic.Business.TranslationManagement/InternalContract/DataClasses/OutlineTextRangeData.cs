using Logic.Domain.GoogleSheetsManagement.Contract.Aspects;

namespace Logic.Business.TranslationManagement.InternalContract.DataClasses
{
    internal class OutlineTextRangeData
    {
        [Column("A")]
        public string Route { get; set; }
        [Column("D")]
        public string Translation { get; set; }
    }
}
