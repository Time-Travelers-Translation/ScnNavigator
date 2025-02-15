using System.Diagnostics;
using Logic.Domain.GraphVizManagement.InternalContract;

namespace Logic.Domain.GraphVizManagement
{
    internal class GraphVizExecutor : IGraphVizExecutor
    {
        private readonly IGraphVizProvider _provider;

        public GraphVizExecutor(IGraphVizProvider provider)
        {
            _provider = provider;
        }

        public string Execute(string command)
        {
            string graphVizExecutable = _provider.GetGraphVizExecutable();

            string output = RetrieveGraph(graphVizExecutable, command);

            return output;
        }

        private string RetrieveGraph(string graphVizExecutable, string command)
        {
            var processStartInfo = new ProcessStartInfo(graphVizExecutable, command)
            {
                RedirectStandardOutput = true
            };

            Process? process = Process.Start(processStartInfo);
            if (process == null)
                return string.Empty;

            process.Start();

            string output = process.StandardOutput.ReadToEnd();

            process.WaitForExit();
            process.Dispose();

            return output;
        }
    }
}
