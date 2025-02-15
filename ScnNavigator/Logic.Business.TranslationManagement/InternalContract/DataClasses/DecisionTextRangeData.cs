using Logic.Domain.GoogleSheetsManagement.Contract.Aspects;

namespace Logic.Business.TranslationManagement.InternalContract.DataClasses
{
    internal class DecisionTextRangeData
    {
        [Column("A")]
        public string SceneName { get; set; }
        [Column("D")]
        public string Translation { get; set; }
    }
}
