using CrossCutting.Core.Contract.Aspects;
using ImGui.Forms.Controls.Base;
using Logic.Business.TimeTravelersManagement.Contract.DataClasses;
using Logic.Domain.GraphVizManagement.Contract.DataClasses;
using Logic.Domain.Level5Management.Contract.DataClasses.Scene;
using UI.ScnNavigator.Components.Contract.Exceptions;

namespace UI.ScnNavigator.Components.Contract
{
    [MapException(typeof(ScnNavigatorComponentsException))]
    public interface IGraphViewComponentFactory
    {
        Component CreateStoryGraphView(Graph graph, IList<Node<SceneEntry>> nodes);
        Component CreateCallGraphView(Graph graph, IList<Node<TtpCallSectionData>> nodes);
    }
}
