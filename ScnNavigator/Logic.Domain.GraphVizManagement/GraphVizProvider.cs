using Logic.Domain.GraphVizManagement.InternalContract;
using System.IO.Compression;

namespace Logic.Domain.GraphVizManagement
{
    internal class GraphVizProvider : IGraphVizProvider
    {
        private const string GraphVizResource_ = "Logic.Domain.GraphVizManagement.Resources.graphviz_dot.zip";
        
        private string? _graphVizPath;

        public string GetGraphVizExecutable()
        {
            string graphVizPath = GetGraphVizDirectory();
            return Path.Combine(graphVizPath, "dot.exe");
        }

        public string GetGraphVizDirectory()
        {
            if (!string.IsNullOrEmpty(_graphVizPath))
                return _graphVizPath;

            return _graphVizPath = ExtractGraphViz();
        }

        private string ExtractGraphViz()
        {
            using Stream graphVizStream = GetGraphVizStream();
            using var graphVizArchive = new ZipArchive(graphVizStream, ZipArchiveMode.Read);

            DirectoryInfo tempPath = Directory.CreateTempSubdirectory();
            graphVizArchive.ExtractToDirectory(tempPath.FullName);

            return tempPath.FullName;
        }

        private Stream GetGraphVizStream()
        {
            Stream? resourceStream = GetType().Assembly.GetManifestResourceStream(GraphVizResource_);
            if (resourceStream == null)
                throw new InvalidOperationException($"Resource {GraphVizResource_} not found.");

            return resourceStream;
        }
    }
}
