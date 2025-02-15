using Logic.Domain.GoogleSheetsManagement.Contract.Aspects;

namespace Logic.Business.TranslationManagement.InternalContract.DataClasses
{
    internal class UpdateDecisionTextRangeData
    {
        [Column("D")]
        public string Translation { get; set; }
    }
}
