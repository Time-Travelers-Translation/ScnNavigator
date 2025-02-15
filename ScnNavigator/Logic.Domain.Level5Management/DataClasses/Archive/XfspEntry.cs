namespace Logic.Domain.Level5Management.DataClasses.Archive
{
    public struct XfspEntry
    {
        public ushort hash;
        public ushort nameOffset;

        public ushort fileOffsetLower;
        public ushort fileSizeLower;
        public byte fileOffsetUpper;
        public byte fileSizeUpper;
    }
}
