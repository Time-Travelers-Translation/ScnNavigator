using Logic.Domain.GoogleSheetsManagement.Contract.Aspects;

namespace Logic.Business.TranslationManagement.InternalContract.DataClasses
{
    internal class TipTextRangeData
    {
        [Column("A")]
        public int Index { get; set; }
        [Column("H")]
        public string TranslatedTitle { get; set; }
        [Column("K")]
        public string Translation { get; set; }
    }
}
