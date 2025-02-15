namespace Logic.Domain.Level5Management.Contract.DataClasses.Script
{
    public class StoryboardInstruction
    {
        public StoryboardOperation Operation { get; set; }
        public StoryboardValue[] Arguments { get; set; }
    }
}
