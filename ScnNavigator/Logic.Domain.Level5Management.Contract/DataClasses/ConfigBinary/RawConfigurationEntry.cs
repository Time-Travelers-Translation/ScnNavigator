namespace Logic.Domain.Level5Management.Contract.DataClasses.ConfigBinary
{
    public class RawConfigurationEntry
    {
        public uint Hash { get; set; }
        public ConfigurationEntryValue[] Values { get; set; }
    }
}
