using Logic.Domain.GoogleSheetsManagement.Contract.Aspects;

namespace Logic.Business.TranslationManagement.InternalContract.DataClasses
{
    internal class TutorialTextRangeData
    {
        [Column("D")]
        public string TranslatedTitle { get; set; }
        [Column("G")]
        public string? Translation { get; set; }
    }
}
