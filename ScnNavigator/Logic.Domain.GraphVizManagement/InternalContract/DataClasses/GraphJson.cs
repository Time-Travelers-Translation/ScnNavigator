namespace Logic.Domain.GraphVizManagement.InternalContract.DataClasses
{
    internal class GraphJson
    {
        public string Name { get; set; }
        public string Bb { get; set; }
        public GraphJsonObject[] Objects { get; set; } = Array.Empty<GraphJsonObject>();
        public GraphJsonEdge[] Edges { get; set; } = Array.Empty<GraphJsonEdge>();
    }
}
