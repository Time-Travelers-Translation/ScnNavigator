using Logic.Domain.GoogleSheetsManagement.Contract.Aspects;

namespace Logic.Business.TranslationManagement.InternalContract.DataClasses
{
    internal class TutorialTextRangeData
    {
        [Column("C")]
        public string TranslatedTitle { get; set; }
        [Column("F")]
        public string? Translation { get; set; }
    }
}
