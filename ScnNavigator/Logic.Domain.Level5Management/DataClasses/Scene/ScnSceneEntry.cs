namespace Logic.Domain.Level5Management.DataClasses.Scene
{
    public struct ScnSceneEntry
    {
        public ScnSceneEntryHeader header;
        public object? data;
        public ScnSceneEntryBranchEntry[] branches;
    }
}
