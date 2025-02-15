namespace Logic.Business.TimeTravelersManagement.InternalContract
{
    internal struct TtpCallHeader
    {
        public string magic;
        public int sectionCount;
        public int sectionOffset;
        public string name;
    }
}
