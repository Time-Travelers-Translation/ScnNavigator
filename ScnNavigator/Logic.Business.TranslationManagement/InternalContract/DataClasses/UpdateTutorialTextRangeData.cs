using Logic.Domain.GoogleSheetsManagement.Contract.Aspects;

namespace Logic.Business.TranslationManagement.InternalContract.DataClasses
{
    internal class UpdateTutorialTextRangeData
    {
        public int Row { get; set; }
        [Column("D")]
        public string TranslatedTitle { get; set; }
        [Column("G")]
        public string Translation { get; set; }
    }
}
