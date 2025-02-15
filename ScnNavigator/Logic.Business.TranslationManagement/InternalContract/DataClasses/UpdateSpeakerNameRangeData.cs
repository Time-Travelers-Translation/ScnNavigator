using Logic.Domain.GoogleSheetsManagement.Contract.Aspects;

namespace Logic.Business.TranslationManagement.InternalContract.DataClasses
{
    internal class UpdateSpeakerNameRangeData
    {
        [Column("C")]
        public string TranslatedName { get; set; }
    }
}
