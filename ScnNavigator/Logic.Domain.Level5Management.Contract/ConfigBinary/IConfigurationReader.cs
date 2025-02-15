using System.IO;
using CrossCutting.Core.Contract.Aspects;
using Logic.Domain.Level5Management.Contract.DataClasses.ConfigBinary;
using Logic.Domain.Level5Management.Contract.Enums.ConfigBinary;
using Logic.Domain.Level5Management.Contract.Exceptions.ConfigBinary;

namespace Logic.Domain.Level5Management.Contract.ConfigBinary
{
    [MapException(typeof(ConfigurationReaderException))]
    public interface IConfigurationReader<TConfigEntry>
    {
        Configuration<TConfigEntry> Read(Stream input);
        Configuration<TConfigEntry> Read(Stream input, StringEncoding encoding);
    }
}
