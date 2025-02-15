using System.Numerics;
using CrossCutting.Core.Contract.EventBrokerage;
using ImGui.Forms.Resources;
using Logic.Domain.GraphVizManagement.Contract.DataClasses;
using Logic.Domain.Level5Management.Contract.DataClasses.Scene;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using UI.ScnNavigator.Abstract.DataClasses;
using UI.ScnNavigator.Abstract.Enums;
using UI.ScnNavigator.Components.Contract.DataClasses;
using UI.ScnNavigator.Resources.Contract;

namespace UI.ScnNavigator.Components.Graphs
{
    public class SceneGraphView : GraphView
    {
        private readonly ImageResource? _timeLockImageResource;

        private readonly IDictionary<string, GraphEdge> _nameEdgeLookup;
        private readonly IDictionary<GraphEdge, string> _edgeNameLookup;

        private readonly IDictionary<string, GraphNode> _nameNodeLookup;

        private readonly IDictionary<string, Node<SceneEntry>> _sceneNodeLookup;

        private GraphEdge? _decisionEdge;
        private GraphEdge? _timeoutEdge;
        private GraphEdge? _mainEdge;

        public SceneGraphView(Graph graph, IList<Node<SceneEntry>> nodes, IEventBroker eventBroker, IImageResourceProvider imageResourceProvider) : base(graph, eventBroker)
        {
            Image<Rgba32>? timeLockImage = imageResourceProvider.GetTimeLockResource();
            if (timeLockImage != null)
                _timeLockImageResource = ImageResource.FromImage(timeLockImage);

            _nameEdgeLookup = graph.Edges.ToDictionary(x => x.Id, x => x);
            _edgeNameLookup = graph.Edges.ToDictionary(x => x, x => x.Id);

            _nameNodeLookup = graph.Nodes.ToDictionary(x => x.Label, x => x);

            _sceneNodeLookup = nodes.ToDictionary(x => x.Data.Name, x => x);

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

        protected override uint GetNodeColor(GraphNode node)
        {
            switch (node.Label[0])
            {
                case 'P':
                    return 0xFF80774D;

                case 'C':
                    return 0xFF804D80;

                case 'H':
                    return 0xFF4D804D;

                case 'R':
                    return 0xFF4D7980;

                case 'S':
                    return 0xFF804D5D;

                case 'M':
                    return 0xFF564D80;
            }

            return base.GetNodeColor(node);
        }

        protected override int GetEdgeThickness(GraphEdge edge)
        {
            if (_decisionEdge == edge || _timeoutEdge == edge || _mainEdge == edge)
                return 2;

            return base.GetEdgeThickness(edge);
        }

        protected override void DrawEdge(GraphEdge edge, GraphContext context)
        {
            base.DrawEdge(edge, context);

            if (_timeLockImageResource == null)
                return;

            if (!_edgeNameLookup.TryGetValue(edge, out string? edgeName))
                return;

            string[] edgeScenes = edgeName.Split(':');
            if (edgeScenes.Length != 2)
                return;

            if (!_sceneNodeLookup.TryGetValue(edgeScenes[0], out Node<SceneEntry>? sourceNode))
                return;

            if (!_sceneNodeLookup.TryGetValue(edgeScenes[1], out Node<SceneEntry>? targetNode))
                return;

            for (var i = 0; i < sourceNode.Data.Branches.Length; i++)
            {
                SceneEntryBranch? branch = sourceNode.Data.Branches[i];
                if (branch == null)
                    continue;

                if (branch.Scene != targetNode.Data)
                    continue;

                if (branch.RequiredFlags.Any(f => f.StartsWith("FLAG_UNLOCK", StringComparison.CurrentCultureIgnoreCase)))
                {
                    Vector2 edgeCenter = GetCenter(edge);
                    Vector2 halfSizeImage = new(_timeLockImageResource.Width / 1.25f, _timeLockImageResource.Height / 1.25f);

                    Vector2 topLeft = edgeCenter - halfSizeImage;
                    Vector2 bottomRight = edgeCenter + halfSizeImage;

                    ImGuiNET.ImGui.GetWindowDrawList().AddImage((nint)_timeLockImageResource, TransformPointToGraph(topLeft, context), TransformPointToGraph(bottomRight, context));
                }

                break;
            }
        }

        private Vector2 GetCenter(GraphEdge edge)
        {
            if ((edge.Points.Length & 1) == 1)
            {
                GraphPoint centerPoint = edge.Points[(edge.Points.Length - 1) / 2];
                return new Vector2(centerPoint.X, centerPoint.Y);
            }

            GraphPoint point1 = edge.Points[edge.Points.Length / 2 - 1];
            GraphPoint point2 = edge.Points[edge.Points.Length / 2];

            int minX = Math.Min(point1.X, point2.X);
            int maxX = Math.Max(point1.X, point2.X);
            int minY = Math.Min(point1.Y, point2.Y);
            int maxY = Math.Max(point1.Y, point2.Y);

            return new Vector2(minX + (maxX - minX) / 2, minY + (maxY - minY) / 2);
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

            _decisionEdge = null;
            _mainEdge = null;
            _timeoutEdge = null;

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
