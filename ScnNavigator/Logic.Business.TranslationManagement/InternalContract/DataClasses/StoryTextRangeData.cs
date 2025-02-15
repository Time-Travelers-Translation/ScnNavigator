using Logic.Domain.GoogleSheetsManagement.Contract.Aspects;

namespace Logic.Business.TranslationManagement.InternalContract.DataClasses
{
    internal class StoryTextRangeData
    {
        [Column("A")]
        public string SceneName { get; set; }
        [Column("B")]
        public string EventName { get; set; }
        [Column("C")]
        public int Index { get; set; }

        [Column("E")]
        public string Speaker { get; set; }

        [Column("I")]
        public string Translation { get; set; }
    }
}
