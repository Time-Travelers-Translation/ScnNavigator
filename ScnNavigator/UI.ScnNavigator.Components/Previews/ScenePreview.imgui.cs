using ImGui.Forms.Controls;
using ImGui.Forms.Controls.Layouts;
using ImGui.Forms.Controls.Text;
using ImGui.Forms.Controls.Text.Editor;
using ImGui.Forms.Models;
using ImGuiNET;
using Veldrid;

namespace UI.ScnNavigator.Components.Previews
{
    public partial class ScenePreview
    {
        private ZoomablePictureBox _textPreview;

        private TextBox _sceneNameTextBox;
        private TextBox _speakerNameTextBox;
        private ArrowButton _previousTextButton;
        private ArrowButton _nextTextButton;
        private TextBox _indexTextBox;

        private TextEditor _originalStoryTextBox;
        private TextEditor _translationTextBox;

        private StackLayout _titleLayout;
        private StackLayout _textLayout;
        private StackLayout _mainLayout;

        private void InitializeComponent()
        {
            _textPreview = new ZoomablePictureBox { ShowBorder = true };
            _textPreview.Zoom(0.125f);

            _sceneNameTextBox = new TextBox { Width = SizeValue.Relative(1f), IsReadOnly = true };
            _speakerNameTextBox = new TextBox { Width = SizeValue.Absolute(100), IsReadOnly = true };
            _previousTextButton = new ArrowButton { Direction = ImGuiDir.Left, Enabled = false };
            _nextTextButton = new ArrowButton { Direction = ImGuiDir.Right, Enabled = false };
            _indexTextBox = new TextBox { Width = SizeValue.Absolute(10), IsReadOnly = true };

            _originalStoryTextBox = new TextEditor { IsReadOnly = true, IsShowingLineNumbers = false };
            _translationTextBox = new TextEditor { IsShowingLineNumbers = false };

            _titleLayout = new StackLayout
            {
                Alignment = Alignment.Horizontal,
                ItemSpacing = 5,
                Size = Size.WidthAlign,
                Items =
                {
                    _sceneNameTextBox,
                    _previousTextButton,
                    _nextTextButton,
                    new StackItem(_speakerNameTextBox){Size = Size.WidthAlign,HorizontalAlignment = HorizontalAlignment.Right},
                    new StackItem(_indexTextBox){Size = Size.Content,HorizontalAlignment = HorizontalAlignment.Right}
                }
            };

            _textLayout = new StackLayout
            {
                Alignment = Alignment.Vertical,
                ItemSpacing = 5,
                Size = new Size(SizeValue.Parent, SizeValue.Relative(.3f)),
                Items =
                {
                    _titleLayout,
                    _originalStoryTextBox
                }
            };

            _mainLayout = new StackLayout
            {
                Alignment = Alignment.Horizontal,
                ItemSpacing = 5,
                Items =
                {
                    _textLayout,
                    _textPreview
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
    }
}
