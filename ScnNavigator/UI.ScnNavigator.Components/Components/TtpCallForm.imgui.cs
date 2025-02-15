using ImGui.Forms.Controls.Base;
using ImGui.Forms.Controls.Layouts;
using ImGui.Forms.Models;
using Logic.Business.TimeTravelersManagement.Contract.DataClasses;
using Logic.Domain.GraphVizManagement.Contract;
using Logic.Domain.GraphVizManagement.Contract.DataClasses;
using UI.ScnNavigator.Components.Contract;
using Veldrid;

namespace UI.ScnNavigator.Components.Components
{
    public partial class TtpCallForm
    {
        private Component _graphView;

        private StackLayout _mainLayout;

        private void InitializeComponent(IList<Node<TtpCallSectionData>> nodes, IGraphViewComponentFactory graphFactory,
            IGraphSyntaxCreator syntaxCreator, IGraphLayoutCreator layoutCreator)
        {
            Graph graph = CreateGraph(nodes, syntaxCreator, layoutCreator);
            _graphView = graphFactory.CreateCallGraphView(graph, nodes);

            _mainLayout = new StackLayout
            {
                Alignment = Alignment.Vertical,
                ItemSpacing = 5,
                Items =
                {
                    _graphView
                }
            };
        }

        public override Size GetSize()
        {
            return Size.Parent;
        }

        protected override void UpdateInternal(Rectangle contentRect)
        {
            _mainLayout.Update(contentRect);
        }

        private Graph CreateGraph(IList<Node<TtpCallSectionData>> nodes, IGraphSyntaxCreator syntaxCreator, IGraphLayoutCreator layoutCreator)
        {
            string syntax = syntaxCreator.Create(nodes, new GraphOptions { SplineShape = SplineShape.Orthogonal });
            return layoutCreator.Create(syntax);
        }
    }
}
