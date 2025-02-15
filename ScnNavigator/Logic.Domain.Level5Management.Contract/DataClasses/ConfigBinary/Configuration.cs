using Logic.Domain.Level5Management.Contract.Enums.ConfigBinary;

namespace Logic.Domain.Level5Management.Contract.DataClasses.ConfigBinary
{
    public class Configuration<TConfigEntry>
    {
        public TConfigEntry[] Entries { get; set; }
        public StringEncoding Encoding { get; set; }
    }
}
