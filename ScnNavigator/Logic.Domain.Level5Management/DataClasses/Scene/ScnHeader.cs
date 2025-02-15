namespace Logic.Domain.Level5Management.DataClasses.Scene
{
    public struct ScnHeader
    {
        public string magic;
        public int unk1;
        public int unk2;
        public short unk3;
        public short unk4;
        public short unk5;
        public short unk6;
        public int sceneEntryCount;
        public int selectionCount;
        public int quickTimeCount;
        public int flagCount;
        public int counterCount;
        public int sceneNameOffset;
        public int decisionTextOffset;
        public int badEndTextOffset;
        public int hintTextOffset;
    }
}
