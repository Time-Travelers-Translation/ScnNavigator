using Logic.Business.TimeTravelersManagement.Contract.DataClasses;
using Logic.Domain.Level5Management.Contract.DataClasses.ConfigBinary;

namespace Logic.Business.TimeTravelersManagement.Contract.Texts
{
    public interface IEventTextParser
    {
        EventTextConfiguration Parse(Configuration<RawConfigurationEntry> config);
    }
}
