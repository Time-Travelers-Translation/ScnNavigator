namespace Logic.Domain.Level5Management.Contract.DataClasses.Scene
{
    public class SceneEntry
    {
        public short Id { get; set; }
        public string Name { get; set; }
        public SceneEntryData Data { get; set; }
        public SceneEntryBranch?[] Branches { get; set; }
    }
}
