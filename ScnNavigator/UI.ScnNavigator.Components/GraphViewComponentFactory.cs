using CrossCutting.Core.Contract.DependencyInjection;
using CrossCutting.Core.Contract.DependencyInjection.DataClasses;
using ImGui.Forms.Controls.Base;
using Logic.Business.TimeTravelersManagement.Contract.DataClasses;
using Logic.Domain.GraphVizManagement.Contract.DataClasses;
using Logic.Domain.Level5Management.Contract.DataClasses.Scene;
using UI.ScnNavigator.Components.Contract;
using UI.ScnNavigator.Components.Graphs;

namespace UI.ScnNavigator.Components
{
    internal class GraphViewComponentFactory: IGraphViewComponentFactory
    {
        private readonly ICoCoKernel _kernel;

        public GraphViewComponentFactory(ICoCoKernel kernel)
        {
            _kernel = kernel;
        }

        public Component CreateStoryGraphView(Graph graph, IList<Node<SceneEntry>> nodes)
        {
            return _kernel.Get<SceneGraphView>(
                new ConstructorParameter("graph", graph),
                new ConstructorParameter("nodes", nodes));
        }
    }
}
