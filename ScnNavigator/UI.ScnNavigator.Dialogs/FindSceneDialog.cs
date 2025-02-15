using CrossCutting.Core.Contract.EventBrokerage;
using ImGui.Forms.Controls;
using ImGui.Forms.Controls.Layouts;
using ImGui.Forms.Modals;
using Logic.Domain.Level5Management.Contract.DataClasses.Scene;
using UI.ScnNavigator.Abstract.DataClasses;
using UI.ScnNavigator.Resources.Contract;

namespace UI.ScnNavigator.Dialogs
{
    internal class FindSceneDialog : Modal
    {
        private readonly object _target;

        private readonly IEventBroker _eventBroker;

        private readonly ComboBox<SceneEntry> _comboBox;

        public FindSceneDialog(object target, IList<SceneEntry> scenes, IEventBroker eventBroker, IStringResourceProvider stringProvider)
        {
            _target = target;

            _eventBroker = eventBroker;

            _comboBox = new ComboBox<SceneEntry>();

            InitializeComboBox(_comboBox, scenes);

            Caption = stringProvider.FindSceneDialogCaption();
            Content = new StackLayout
            {
                Items =
                {
                    _comboBox
                }
            };
        }

        private void InitializeComboBox(ComboBox<SceneEntry> comboBox, IList<SceneEntry> scenes)
        {
            foreach (SceneEntry scene in scenes.OrderBy(s => s.Name))
                comboBox.Items.Add(new DropDownItem<SceneEntry>(scene, scene.Name));

            if (comboBox.Items.Count > 0)
                comboBox.SelectedItem = comboBox.Items[0];

            comboBox.SelectedItemChanged += ComboBox_SelectedItemChanged;
        }

        private void ComboBox_SelectedItemChanged(object? sender, EventArgs e)
        {
            if (_comboBox.SelectedItem == null)
                return;

            _eventBroker.Raise(new SceneChangedMessage(_target, _comboBox.SelectedItem.Content.Name));

            Close(DialogResult.Ok);
        }
    }
}
