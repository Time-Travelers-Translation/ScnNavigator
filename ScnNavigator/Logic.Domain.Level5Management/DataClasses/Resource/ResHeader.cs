namespace Logic.Domain.Level5Management.DataClasses.Resource
{
    internal struct ResHeader
    {
        public string magic;

        public short stringTablesOffset;
        public short stringTablesCount;  // always 1

        public short imageTablesOffset;
        public short imageTablesCount;    // always 3

        public short layoutDataTablesOffset;
        public short layoutDataTablesCount;    // always 7
    }
}
