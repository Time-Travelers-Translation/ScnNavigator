using Logic.Domain.GoogleSheetsManagement.Contract.Aspects;

namespace Logic.Business.TranslationManagement.InternalContract.DataClasses
{
    internal class UpdateTutorialTextRangeData
    {
        public int Row { get; set; }
        [Column("C")]
        public string TranslatedTitle { get; set; }
        [Column("F")]
        public string Translation { get; set; }
    }
}
