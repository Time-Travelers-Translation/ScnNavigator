using System.IO;
using CrossCutting.Core.Contract.Aspects;
using Logic.Domain.Level5Management.Contract.DataClasses.ConfigBinary;
using Logic.Domain.Level5Management.Contract.Exceptions.ConfigBinary;

namespace Logic.Domain.Level5Management.Contract.ConfigBinary
{
    [MapException(typeof(ConfigurationWriterException))]
    public interface IConfigurationWriter<TConfigEntry>
    {
        Stream Write(Configuration<TConfigEntry> config);
    }
}
