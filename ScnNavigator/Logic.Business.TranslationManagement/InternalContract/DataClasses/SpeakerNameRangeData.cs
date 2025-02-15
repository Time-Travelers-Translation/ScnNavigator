using Logic.Domain.GoogleSheetsManagement.Contract.Aspects;

namespace Logic.Business.TranslationManagement.InternalContract.DataClasses
{
    internal class SpeakerNameRangeData
    {
        [Column("A")]
        public string OriginalName { get; set; }
        [Column("C")]
        public string TranslatedName { get; set; }
    }
}
