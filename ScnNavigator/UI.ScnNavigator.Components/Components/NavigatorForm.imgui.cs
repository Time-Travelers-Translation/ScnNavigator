using ImGui.Forms;
using ImGui.Forms.Controls;
using ImGui.Forms.Models;
using ImGui.Forms.Models.IO;
using Veldrid;

namespace UI.ScnNavigator.Components.Components
{
    public partial class NavigatorForm
    {
        private static readonly KeyCommand SaveCommand = new(ModifierKeys.Control, Key.S);

        private TabControl _chapterTabControl;

        private void InitializeComponent()
        {
            _chapterTabControl = new TabControl();
        }

        public override Size GetSize()
        {
            return Size.Parent;
        }

        protected override void UpdateInternal(Rectangle contentRect)
        {
            _chapterTabControl.Update(contentRect);

            if (SaveCommand.IsPressed() && !Application.Instance.MainForm.HasOpenModals())
                Save();
        }
    }
}
