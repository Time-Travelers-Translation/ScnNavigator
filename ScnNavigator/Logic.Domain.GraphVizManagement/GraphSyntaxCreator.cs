using Logic.Domain.GraphVizManagement.Contract;
using Logic.Domain.GraphVizManagement.Contract.DataClasses;

namespace Logic.Domain.GraphVizManagement
{
    internal class GraphSyntaxCreator : IGraphSyntaxCreator
    {
        private const string GraphTemplate_ = """strict digraph{{rankdir="{1}";splines={2};node[shape={3}];{0}}}""";

        public string Create<TData>(IList<Node<TData>> nodes)
        {
            return Create(nodes, new GraphOptions());
        }

        public string Create<TData>(IList<Node<TData>> nodes, GraphOptions options)
        {
            return Create(nodes.Cast<Node>().ToArray(), options);
        }

        public string Create(IList<Node> nodes)
        {
            return Create(nodes, new GraphOptions());
        }

        public string Create(IList<Node> nodes, GraphOptions options)
        {
            string direction = GetGraphDirection(options.Direction);
            string nodeShape = GetNodeShape(options.NodeShape);
            string splineShape = GetSplineShape(options.SplineShape);
            string nodeRelations = GetNodeRelations(nodes);

            return string.Format(GraphTemplate_, nodeRelations, direction, splineShape, nodeShape);
        }

        private string GetNodeRelations(IList<Node> nodes)
        {
            var res = string.Empty;

            // Dump numbered nodes with their label
            var nodeDictionary = new Dictionary<Node, int>();
            for (var i = 0; i < nodes.Count; i++)
            {
                res += $"{i}[label=\"{nodes[i].Text}\"];";
                nodeDictionary[nodes[i]] = i;
            }

            // Print relations with their label
            foreach (Node parentNode in nodeDictionary.Keys)
                foreach (Node childNode in parentNode.Children)
                    res += $"{nodeDictionary[parentNode]}->{nodeDictionary[childNode]}[id=\"{parentNode.Text}:{childNode.Text}\"];";

            return res;
        }

        private string GetGraphDirection(GraphDirection dir)
        {
            switch (dir)
            {
                case GraphDirection.LeftToRight:
                    return "LR";

                case GraphDirection.TopToBottom:
                    return "TB";

                default:
                    throw new InvalidOperationException($"Unknown graph direction {dir}.");
            }
        }

        private string GetNodeShape(NodeShape shape)
        {
            switch (shape)
            {
                case NodeShape.Box:
                    return "box";

                case NodeShape.Circle:
                    return "circle";

                default:
                    throw new InvalidOperationException($"Unknown nodes shape {shape}.");
            }
        }

        private string GetSplineShape(SplineShape shape)
        {
            switch (shape)
            {
                case SplineShape.None:
                    return "none";

                case SplineShape.Line:
                    return "line";

                case SplineShape.PolyLine:
                    return "polyline";

                case SplineShape.Curved:
                    return "curved";

                case SplineShape.Orthogonal:
                    return "ortho";

                case SplineShape.Spline:
                    return "spline";

                default:
                    throw new InvalidOperationException($"Unknown splines shape {shape}.");
            }
        }
    }
}
