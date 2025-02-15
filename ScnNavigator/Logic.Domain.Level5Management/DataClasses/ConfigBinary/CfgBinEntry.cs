namespace Logic.Domain.Level5Management.DataClasses.ConfigBinary
{
    internal struct CfgBinEntry
    {
        public uint crc32;
        public byte entryCount;
        public byte[] entryTypes;
        public int[] entryValues;
    }
}
