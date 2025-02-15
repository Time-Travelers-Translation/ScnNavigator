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
    internal partial class HelpsForm
    {
        private static readonly KeyCommand SaveCommand = new(ModifierKeys.Control, Key.S);

        private TreeView<HelpTitleData> _helpTitleTreeView;

        private ArrowButton _previousHelpButton;
        private ArrowButton _nextHelpButton;

        private Panel _helpPanel;
        private StackLayout _helpControlLayout;
        private StackLayout _helpLayout;
        private StackLayout _mainLayout;

        private void InitializeComponent(HelpTitleData[] helpTitles)
        {
            _helpTitleTreeView = new TreeView<HelpTitleData>();

            _previousHelpButton = new ArrowButton { Direction = ImGuiDir.Up };
            _nextHelpButton = new ArrowButton { Direction = ImGuiDir.Down };

            _helpControlLayout = new StackLayout
            {
                Size = Size.WidthAlign,
                Alignment = Alignment.Horizontal,
                ItemSpacing = 5,
                Items =
                {
                    _previousHelpButton,
                    _nextHelpButton
                }
            };

            _helpPanel = new Panel();

            _helpLayout = new StackLayout
            {
                Size = new Size(SizeValue.Absolute(200), SizeValue.Parent),
                Alignment = Alignment.Vertical,
                ItemSpacing = 5,
                Items =
                {
                    _helpControlLayout,
                    _helpTitleTreeView
                }
            };

            _mainLayout = new StackLayout
            {
                Alignment = Alignment.Horizontal,
                ItemSpacing = 5,
                Items =
                {
                    _helpLayout,
                    _helpPanel
                }
            };

            InitializeHelpTitles(helpTitles);
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
            _helpTitleTreeView.SetTabInactive();
        }

        private void InitializeHelpTitles(HelpTitleData[] helpTitles)
        {
            foreach (HelpTitleData helpTitle in helpTitles)
                _helpTitleTreeView.Nodes.Add(new TreeNode<HelpTitleData> { Text = $"HELP{helpTitle.Id + 1:000}", Data = helpTitle });
        }
    }
}
