namespace Logic.Domain.Level5Management.DataClasses.Script
{
    public struct StbHeader
    {
        public string magic;
        public int mainLogicOffset;
        public int firstLogicOffset;
        public int logicTableOffset;
        public int logicTableCount;
    }
}
