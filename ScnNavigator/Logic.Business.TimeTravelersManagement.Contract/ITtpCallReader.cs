using Logic.Business.TimeTravelersManagement.Contract.DataClasses;
using Logic.Domain.Level5Management.Contract.Enums.ConfigBinary;

namespace Logic.Business.TimeTravelersManagement.Contract
{
    public interface ITtpCallReader
    {
        TtpCallData Read(Stream input);
        TtpCallData Read(Stream input, StringEncoding encoding);
    }
}
