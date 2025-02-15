using CrossCutting.Core.Contract.Aspects;
using ImGui.Forms.Controls.Base;
using Logic.Domain.GraphVizManagement.Contract.DataClasses;
using Logic.Domain.Level5Management.Contract.DataClasses.Scene;
using UI.ScnNavigator.Components.Contract.Exceptions;

namespace UI.ScnNavigator.Components.Contract
{
    [MapException(typeof(ScnNavigatorComponentsException))]
    public interface IGraphViewComponentFactory
    {
        Component CreateStoryGraphView(Graph graph, IList<Node<SceneEntry>> nodes);
    }
}
