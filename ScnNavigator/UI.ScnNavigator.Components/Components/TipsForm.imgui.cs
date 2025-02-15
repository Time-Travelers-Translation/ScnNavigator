using ImGui.Forms;
using ImGui.Forms.Controls;
using ImGui.Forms.Controls.Layouts;
using ImGui.Forms.Controls.Tree;
using ImGui.Forms.Modals;
using ImGui.Forms.Models;
using ImGui.Forms.Models.IO;
using ImGuiNET;
using UI.ScnNavigator.Components.Contract.DataClasses;
using UI.ScnNavigator.Dialogs.Contract.DataClasses;
using Veldrid;

namespace UI.ScnNavigator.Components.Components
{
    public partial class TipsForm
    {
        private static readonly KeyCommand SaveCommand = new(ModifierKeys.Control, Key.S);
        private static readonly KeyCommand FindTipCommand = new(ModifierKeys.Control, Key.F);

        private bool _isTipDialogOpen;
        private Modal? _tipDialog;

        private TreeView<TipTitleData> _tipTitleTreeView;

        private ArrowButton _previousTipButton;
        private ArrowButton _nextTipButton;

        private Panel _tipPanel;
        private StackLayout _tipControlLayout;
        private StackLayout _tipLayout;
        private StackLayout _mainLayout;

        private void InitializeComponent(TipTitleData[] tipTitles)
        {
            _tipTitleTreeView = new TreeView<TipTitleData>();

            _previousTipButton = new ArrowButton { Direction = ImGuiDir.Up };
            _nextTipButton = new ArrowButton { Direction = ImGuiDir.Down };

            _tipControlLayout = new StackLayout
            {
                Size = Size.WidthAlign,
                Alignment = Alignment.Horizontal,
                ItemSpacing = 5,
                Items =
                {
                    _previousTipButton,
                    _nextTipButton
                }
            };

            _tipPanel = new Panel();

            _tipLayout = new StackLayout
            {
                Size = new Size(SizeValue.Absolute(200), SizeValue.Parent),
                Alignment = Alignment.Vertical,
                ItemSpacing = 5,
                Items =
                {
                    _tipControlLayout,
                    _tipTitleTreeView
                }
            };

            _mainLayout = new StackLayout
            {
                Alignment = Alignment.Horizontal,
                ItemSpacing = 5,
                Items =
                {
                    _tipLayout,
                    _tipPanel
                }
            };

            InitializeTipTitles(tipTitles);
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

            if (FindTipCommand.IsPressed() && !Application.Instance.MainForm.HasOpenModals())
                OpenFindTipDialog();
        }

        private async void OpenFindTipDialog()
        {
            if (_isTipDialogOpen)
                return;

            _isTipDialogOpen = true;

            _tipDialog ??= _dialogFactory.CreateFindTipDialog(this, _tipTitleEntries);
            await _tipDialog.ShowAsync();

            _isTipDialogOpen = false;
        }

        protected override void SetTabInactiveCore()
        {
            _tipTitleTreeView.SetTabInactive();
        }

        private void InitializeTipTitles(TipTitleData[] tipTitles)
        {
            foreach (TipTitleData tipTitle in tipTitles)
                _tipTitleTreeView.Nodes.Add(new TreeNode<TipTitleData> { Text = $"TIP{tipTitle.Id:000}", Data = tipTitle });
        }
    }
}
