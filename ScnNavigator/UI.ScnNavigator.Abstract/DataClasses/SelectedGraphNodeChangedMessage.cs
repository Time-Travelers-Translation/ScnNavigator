using Logic.Domain.GraphVizManagement.Contract.DataClasses;

namespace UI.ScnNavigator.Abstract.DataClasses
{
    public class SelectedGraphNodeChangedMessage
    {
        public GraphNode Node { get; }

        public SelectedGraphNodeChangedMessage(GraphNode node)
        {
            Node = node;
        }
    }
}
