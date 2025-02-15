using CrossCutting.Core.Contract.EventBrokerage;
using ImGui.Forms;
using ImGui.Forms.Controls;
using ImGui.Forms.Controls.Base;
using ImGui.Forms.Models;
using ImGui.Forms.Models.IO;
using UI.ScnNavigator.Abstract.DataClasses;
using UI.ScnNavigator.Components.Contract;
using UI.ScnNavigator.Components.Contract.DataClasses;
using Veldrid;

namespace UI.ScnNavigator.Components.Components
{
    internal partial class StaffrollForm
    {
        private static readonly KeyCommand SaveCommand = new(ModifierKeys.Control, Key.S);

        private Component _previewForm;

        private Panel _content;

        private void InitializeComponent(StaffRollPreviewData data, IPreviewComponentFactory previewFactory, IEventBroker eventBroker)
        {
            _previewForm = previewFactory.CreateStaffRollPreview();
            eventBroker.Raise(new PreviewChangedMessage<StaffRollPreviewData>(_previewForm, data, 0));

            _content = new Panel
            {
                Content = _previewForm
            };
        }

        public override Size GetSize()
        {
            return Size.Parent;
        }

        protected override void UpdateInternal(Rectangle contentRect)
        {
            _content.Update(contentRect);

            if (SaveCommand.IsPressed() && !Application.Instance.MainForm.HasOpenModals())
                Save();
        }
    }
}
