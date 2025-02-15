using Logic.Domain.GoogleSheetsManagement.Contract.Aspects;

namespace Logic.Business.TranslationManagement.InternalContract.DataClasses
{
    internal class UpdateStoryTextRangeData
    {
        [Column("C")]
        public int Index { get; set; }
        [Column("I")]
        public string Translation { get; set; }
    }
}
