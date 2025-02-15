namespace Logic.Business.TimeTravelersManagement.Contract.DataClasses
{
    public class TtpCallSectionData
    {
        public int Id { get; set; }
        public TtpCallBlockData[] Blocks { get; set; }
    }
}
