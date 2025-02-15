namespace Logic.Domain.Level5Management.Contract.DataClasses.Scene
{
    public class SceneEntryDecisionData : SceneEntryData
    {
        public byte Seconds { get; set; }
        public string[] Decisions { get; set; }
    }
}
