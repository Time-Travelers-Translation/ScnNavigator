using Logic.Domain.GoogleSheetsManagement.Contract.Aspects;

namespace Logic.Business.TranslationManagement.InternalContract.DataClasses
{
    internal class PostTextRangeData
    {
        [Column("A")]
        public string Name { get; set; }
        [Column("B")]
        public int Id { get; set; }
        [Column("C")]
        public string? DecisionMarker { get; set; }
        [Column("G")]
        public string TranslatedText { get; set; }
    }
}
