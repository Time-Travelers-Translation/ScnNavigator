namespace Logic.Business.TimeTravelersManagement.Contract.DataClasses
{
    public class TtpCallWardData
    {
        public string? Text { get; set; }
        public TtpCallVoiceData? Voice { get; set; }
        public ushort Flags2 { get; set; }
        public ushort Flags3 { get; set; }
        public short NextSectionId { get; set; }
        public ushort Flags4 { get; set; }
    }
}
