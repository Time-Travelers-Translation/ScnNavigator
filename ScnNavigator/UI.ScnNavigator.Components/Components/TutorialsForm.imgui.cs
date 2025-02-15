using ImGui.Forms;
using ImGui.Forms.Controls;
using ImGui.Forms.Controls.Layouts;
using ImGui.Forms.Controls.Tree;
using ImGui.Forms.Models;
using ImGui.Forms.Models.IO;
using ImGuiNET;
using UI.ScnNavigator.Components.Contract.DataClasses;
using Veldrid;

namespace UI.ScnNavigator.Components.Components
{
    internal partial class TutorialsForm
    {
        private static readonly KeyCommand SaveCommand = new(ModifierKeys.Control, Key.S);

        private TreeView<TutorialTitleData> _tutorialTitleTreeView;

        private ArrowButton _previousTutorialButton;
        private ArrowButton _nextTutorialButton;

        private Panel _tutorialPanel;
        private StackLayout _tutorialControlLayout;
        private StackLayout _tutorialLayout;
        private StackLayout _mainLayout;

        private void InitializeComponent(TutorialTitleData[] tutorialTitles)
        {
            _tutorialTitleTreeView = new TreeView<TutorialTitleData>();

            _previousTutorialButton = new ArrowButton { Direction = ImGuiDir.Up };
            _nextTutorialButton = new ArrowButton { Direction = ImGuiDir.Down };

            _tutorialControlLayout = new StackLayout
            {
                Size = Size.WidthAlign,
                Alignment = Alignment.Horizontal,
                ItemSpacing = 5,
                Items =
                {
                    _previousTutorialButton,
                    _nextTutorialButton
                }
            };

            _tutorialPanel = new Panel();

            _tutorialLayout = new StackLayout
            {
                Size = new Size(SizeValue.Absolute(200), SizeValue.Parent),
                Alignment = Alignment.Vertical,
                ItemSpacing = 5,
                Items =
                {
                    _tutorialControlLayout,
                    _tutorialTitleTreeView
                }
            };

            _mainLayout = new StackLayout
            {
                Alignment = Alignment.Horizontal,
                ItemSpacing = 5,
                Items =
                {
                    _tutorialLayout,
                    _tutorialPanel
                }
            };

            InitializeTutorialTitles(tutorialTitles);
        }

        public override Size GetSize()
        {
            return Size.Parent;
        }

        protected override void UpdateInternal(Rectangle contentRect)
        {
            _mainLayout.Update(contentRect);

            if (SaveCommand.IsPressed() && !Application.Instance.MainForm.HasOpenModals())
                Save();
        }

        protected override void SetTabInactiveCore()
        {
            _tutorialTitleTreeView.SetTabInactive();
        }

        private void InitializeTutorialTitles(TutorialTitleData[] tutorialTitles)
        {
            foreach (TutorialTitleData tutorialTitle in tutorialTitles)
                _tutorialTitleTreeView.Nodes.Add(new TreeNode<TutorialTitleData> { Text = $"TUTO{tutorialTitle.Id + 1:000}", Data = tutorialTitle });
        }
    }
}
