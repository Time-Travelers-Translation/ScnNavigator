using System.Globalization;
using Logic.Domain.GraphVizManagement.Contract;
using CrossCutting.Core.Contract.Serialization;
using Logic.Domain.GraphVizManagement.Contract.DataClasses;
using Logic.Domain.GraphVizManagement.InternalContract;
using Logic.Domain.GraphVizManagement.InternalContract.DataClasses;

namespace Logic.Domain.GraphVizManagement
{
    public class GraphLayoutCreator : IGraphLayoutCreator
    {
        private readonly ISerializer _serializer;
        private readonly IGraphVizExecutor _executor;

        public GraphLayoutCreator(ISerializer serializer, IGraphVizExecutor executor)
        {
            _serializer = serializer;
            _executor = executor;
        }

        public Graph Create(string graphSyntax)
        {
            string tempGraphFilePath = Path.GetTempFileName();
            File.WriteAllText(tempGraphFilePath, graphSyntax);

            string result = _executor.Execute($"-Tjson0 {tempGraphFilePath}");

            TryDeleteFile(tempGraphFilePath);

            var graph = _serializer.Deserialize<GraphJson>(result);
            if (graph == null)
                throw new InvalidOperationException("Graph layout could not be retrieved.");

            return CreateGraph(graph);
        }

        private void TryDeleteFile(string filePath)
        {
            try
            {
                File.Delete(filePath);
            }
            catch
            {
                ;
            }
        }

        private Graph CreateGraph(GraphJson json)
        {
            GraphSize size = CreateGraphSize(json.Bb);

            var nodes = new GraphNode[json.Objects.Length];
            for (var i = 0; i < nodes.Length; i++)
                nodes[i] = CreateGraphNode(json.Objects[i]);

            var edges = new GraphEdge[json.Edges.Length];
            for (var i = 0; i < edges.Length; i++)
                edges[i] = CreateGraphEdge(json.Edges[i]);

            return new Graph
            {
                Name = json.Name,
                Size = size,
                Nodes = nodes,
                Edges = edges
            };
        }

        private GraphSize CreateGraphSize(string size)
        {
            string[] coordinates = size.Split(',');
            if (coordinates.Length != 4)
                return new GraphSize();

            var intCoordinates = new int[coordinates.Length];
            for (var i = 0; i < intCoordinates.Length; i++)
                intCoordinates[i] = ParseCoordinate(coordinates[i]);

            return new GraphSize
            {
                Width = intCoordinates[2],
                Height = intCoordinates[3]
            };
        }

        private GraphNode CreateGraphNode(GraphJsonObject obj)
        {
            int width = ParseDimension(obj.Width);
            int height = ParseDimension(obj.Height);

            return new GraphNode
            {
                Name = obj.Name,
                Label = obj.Label,
                Width = width,
                Height = height,
                Position = CreateNodePosition(obj.Pos, width, height)
            };
        }

        private GraphEdge CreateGraphEdge(GraphJsonEdge edge)
        {
            if (!edge.Pos.StartsWith("e,"))
            {
                return new GraphEdge
                {
                    Id = edge.Id,
                    Points = Array.Empty<GraphPoint>(),

                    EdgePos = edge.Pos
                };
            }

            string[] points = edge.Pos[2..].Split(' ');
            var edgePoints = new GraphPoint[points.Length];

            edgePoints[0] = CreateGraphPoint(points[0]);
            for (var i = 1; i < points.Length; i++)
                edgePoints[i] = CreateGraphPoint(points[^i]);

            return new GraphEdge
            {
                Id = edge.Id,
                Points = edgePoints,

                EdgePos = edge.Pos
            };
        }

        private GraphPoint CreateNodePosition(string point, int width, int height)
        {
            GraphPoint nodePosition = CreateGraphPoint(point);

            nodePosition.X -= width / 2;
            nodePosition.Y -= height / 2;

            return nodePosition;
        }

        private GraphPoint CreateGraphPoint(string point)
        {
            string[] coordinates = point.Split(',');
            if (coordinates.Length < 2)
                return new GraphPoint();

            return new GraphPoint
            {
                X = ParseCoordinate(coordinates[0]),
                Y = ParseCoordinate(coordinates[1])
            };
        }

        private int ParseDimension(string value)
        {
            float parsedValue = ParseFloat(value);
            return (int)Math.Floor(parsedValue * 96);
        }

        private int ParseCoordinate(string point)
        {
            float parsedValue = ParseFloat(point);
            return (int)Math.Floor(parsedValue / 72 * 96);
        }

        private float ParseFloat(string value)
        {
            if (!float.TryParse(value, CultureInfo.GetCultureInfo("en-GB"), out float floatValue))
                floatValue = 0f;

            return floatValue;
        }
    }
}
