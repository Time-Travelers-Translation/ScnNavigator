using Logic.Domain.Level5Management.Contract.Enums.ConfigBinary;

namespace Logic.Business.TimeTravelersManagement.Contract.DataClasses
{
    public class TtpCallData
    {
        public string Name { get; set; }
        public TtpCallSectionData[] Sections { get; set; }

        public StringEncoding Encoding { get; set; }
    }
}
