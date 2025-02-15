using ImGui.Forms.Controls.Base;
using ImGui.Forms.Controls.Layouts;
using ImGui.Forms.Modals;
using ImGui.Forms.Models;
using ImGui.Forms.Models.IO;
using Logic.Domain.GraphVizManagement.Contract;
using Logic.Domain.GraphVizManagement.Contract.DataClasses;
using Logic.Domain.Level5Management.Contract.DataClasses.Scene;
using UI.ScnNavigator.Components.Contract;
using Veldrid;

namespace UI.ScnNavigator.Components.Components
{
    public partial class ChapterForm
    {
        private static readonly KeyCommand FindSceneCommand = new(ModifierKeys.Control, Key.F);

        private bool _isFindDialogOpen;

        private Component _graphView;

        private StackLayout _mainLayout;

        private void InitializeComponent(IList<Node<SceneEntry>> nodes, IGraphViewComponentFactory graphFactory,
            IGraphSyntaxCreator syntaxCreator, IGraphLayoutCreator layoutCreator)
        {
            Graph graph = CreateGraph(nodes, syntaxCreator, layoutCreator);
            _graphView = graphFactory.CreateStoryGraphView(graph, nodes);

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
            if (FindSceneCommand.IsPressed())
                OpenFindSceneDialog();

            _mainLayout.Update(contentRect);
        }

        private async void OpenFindSceneDialog()
        {
            if (_isFindDialogOpen)
                return;

            _isFindDialogOpen = true;

            IList<SceneEntry> scenes = _nodeLookup.Values.Select(n => n.Data).ToArray();
            Modal dialog = _dialogFactory.CreateFindSceneDialog(_graphView, scenes);

            await dialog.ShowAsync();

            _isFindDialogOpen = false;
        }

        private Graph CreateGraph(IList<Node<SceneEntry>> nodes, IGraphSyntaxCreator syntaxCreator, IGraphLayoutCreator layoutCreator)
        {
            string syntax = syntaxCreator.Create(nodes, new GraphOptions { SplineShape = SplineShape.Orthogonal });
            return layoutCreator.Create(syntax);
        }
    }
}
