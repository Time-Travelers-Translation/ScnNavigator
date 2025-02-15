using ImGui.Forms.Controls;
using ImGui.Forms.Controls.Layouts;
using ImGui.Forms.Controls.Text.Editor;
using ImGui.Forms.Models;
using ImGuiNET;
using UI.ScnNavigator.Components.Contract.DataClasses;
using Veldrid;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace UI.ScnNavigator.Components.Previews
{
    public abstract partial class BasePreview<TTextData>
    {
        private ZoomablePictureBox _textPreview;

        private ArrowButton _previousTextButton;
        private ArrowButton _nextTextButton;

        private TextEditor _originalTextBox;
        private TextEditor _translationTextBox;

        private StackLayout _titleLayout;
        private StackLayout _textLayout;
        private StackLayout _mainLayout;

        private void InitializeComponent()
        {
            _textPreview = new ZoomablePictureBox { ShowBorder = true };
            _textPreview.Zoom(0.125f);

            _previousTextButton = new ArrowButton { Direction = ImGuiDir.Left, Enabled = false };
            _nextTextButton = new ArrowButton { Direction = ImGuiDir.Right, Enabled = false };

            _originalTextBox = new TextEditor { IsReadOnly = true, IsShowingLineNumbers = false };
            _translationTextBox = new TextEditor { IsShowingLineNumbers = false };

            _titleLayout = new StackLayout
            {
                Alignment = Alignment.Horizontal,
                ItemSpacing = 5,
                Size = Size.WidthAlign,
                Items =
                {
                    _previousTextButton,
                    _nextTextButton
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
                    _originalTextBox
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
