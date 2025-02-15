using CrossCutting.Core.Contract.EventBrokerage;
using Logic.Business.TimeTravelersManagement.Contract.DataClasses;
using Logic.Domain.GraphVizManagement.Contract.DataClasses;
using UI.ScnNavigator.Abstract.DataClasses;
using UI.ScnNavigator.Abstract.Enums;

namespace UI.ScnNavigator.Components.Graphs
{
    public class CallGraphView : GraphView
    {
        private readonly IDictionary<string, GraphEdge> _nameEdgeLookup;

        private readonly IDictionary<string, GraphNode> _nameNodeLookup;

        private GraphEdge? _decisionEdge;
        private GraphEdge? _timeoutEdge;
        private GraphEdge? _mainEdge;

        public CallGraphView(Graph graph, IList<Node<TtpCallSectionData>> nodes, IEventBroker eventBroker) : base(graph, eventBroker)
        {
            _nameEdgeLookup = graph.Edges.ToDictionary(x => x.Id, x => x);

            _nameNodeLookup = graph.Nodes.ToDictionary(x => x.Name, x => x);

            eventBroker.Subscribe<SceneChangedMessage>(ChangeSelectedScene);
            eventBroker.Subscribe<BranchChangedMessage>(ChangeSelectedBranches);
        }

        protected override uint GetEdgeColor(GraphEdge edge)
        {
            if (_decisionEdge == edge)
                return 0xC0ECA23D;

            if (_timeoutEdge == edge)
                return 0xC0E4467B;

            if (_mainEdge == edge)
                return 0xC0343FAD;

            return base.GetEdgeColor(edge);
        }

        protected override int GetEdgeThickness(GraphEdge edge)
        {
            if (_decisionEdge == edge || _timeoutEdge == edge || _mainEdge == edge)
                return 2;

            return base.GetEdgeThickness(edge);
        }

        protected override void OnSelectedGraphNodeChanged(GraphNode node)
        {
            _decisionEdge = null;
            _mainEdge = null;
            _timeoutEdge = null;

            base.OnSelectedGraphNodeChanged(node);
        }

        private void ChangeSelectedScene(SceneChangedMessage sceneMessage)
        {
            if (sceneMessage.Target != this)
                return;

            if (!_nameNodeLookup.TryGetValue(sceneMessage.Scene, out GraphNode? node))
                return;

            SetSelectedNode(node);
        }

        private void ChangeSelectedBranches(BranchChangedMessage branchMessage)
        {
            if (branchMessage.Target != this)
                return;

            switch (branchMessage.BranchType)
            {
                case BranchType.Decision:
                    ChangeBranch(branchMessage.SourceScene, branchMessage.TargetScene, edge => _decisionEdge = edge);
                    break;

                case BranchType.Timeout:
                    ChangeBranch(branchMessage.SourceScene, branchMessage.TargetScene, edge => _timeoutEdge = edge);
                    break;

                case BranchType.Main:
                    ChangeBranch(branchMessage.SourceScene, branchMessage.TargetScene, edge => _mainEdge = edge);
                    break;
            }
        }

        private void ChangeBranch(string? sourceScene, string? targetScene, Action<GraphEdge?> setBranchAction)
        {
            if (string.IsNullOrEmpty(sourceScene) || string.IsNullOrEmpty(targetScene))
            {
                setBranchAction(null);
                return;
            }

            var edgeLabel = $"{sourceScene}:{targetScene}";
            if (!_nameEdgeLookup.TryGetValue(edgeLabel, out GraphEdge? foundEdge))
                return;

            setBranchAction(foundEdge);
        }
    }
}
