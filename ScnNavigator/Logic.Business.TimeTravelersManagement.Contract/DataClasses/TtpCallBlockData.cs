namespace Logic.Business.TimeTravelersManagement.Contract.DataClasses
{
    public class TtpCallBlockData
    {
        public TtpCallWardData[] Wards { get; set; }
        public TtpCallDecisionEntry[] Decisions { get; set; }
    }
}
