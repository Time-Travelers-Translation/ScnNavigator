using UI.ScnNavigator.Abstract.Enums;

namespace UI.ScnNavigator.Abstract.DataClasses
{
    public class BranchChangedMessage
    {
        public object Target { get; }
        public string? SourceScene { get; }
        public string? TargetScene { get; }
        public BranchType BranchType { get; }

        public BranchChangedMessage(object target, string? sourceScene, string? targetScene, BranchType branchType)
        {
            Target = target;
            SourceScene = sourceScene;
            TargetScene = targetScene;
            BranchType = branchType;
        }
    }
}
