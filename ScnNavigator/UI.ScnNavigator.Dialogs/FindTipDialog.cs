using CrossCutting.Core.Contract.EventBrokerage;
using ImGui.Forms.Controls.Layouts;
using ImGui.Forms.Controls;
using ImGui.Forms.Modals;
using UI.ScnNavigator.Abstract.DataClasses;
using UI.ScnNavigator.Dialogs.Contract.DataClasses;
using UI.ScnNavigator.Resources.Contract;

namespace UI.ScnNavigator.Dialogs
{
    internal class FindTipDialog : Modal
    {
        private readonly object _target;

        private readonly IEventBroker _eventBroker;

        private readonly ComboBox<TipTitleEntryData> _comboBox;
        private readonly IList<TipTitleEntryData> _titles;

        public FindTipDialog(object target, IList<TipTitleEntryData> titles, IEventBroker eventBroker, IStringResourceProvider stringProvider)
        {
            _target = target;

            _eventBroker = eventBroker;

            _comboBox = new ComboBox<TipTitleEntryData>();
            _titles = titles;

            Caption = stringProvider.FindTipDialogCaption();
            Content = new StackLayout
            {
                Items =
                {
                    _comboBox
                }
            };
        }

        protected override void ShowInternal()
        {
            InitializeComboBox(_comboBox, _titles);
        }

        private void InitializeComboBox(ComboBox<TipTitleEntryData> comboBox, IList<TipTitleEntryData> titles)
        {
            comboBox.SelectedItemChanged -= ComboBox_SelectedItemChanged;

            comboBox.Items.Clear();
            comboBox.SelectedItem = null;

            foreach (TipTitleEntryData title in titles)
            {
                string text = title.TranslatedTipTitle?.Text ?? title.TipTitle?.Text ?? string.Empty;
                comboBox.Items.Add(new DropDownItem<TipTitleEntryData>(title, text));
            }

            if (comboBox.Items.Count > 0)
                comboBox.SelectedItem = comboBox.Items[0];

            comboBox.SelectedItemChanged += ComboBox_SelectedItemChanged;
        }

        private void ComboBox_SelectedItemChanged(object? sender, EventArgs e)
        {
            if (_comboBox.SelectedItem == null)
                return;

            _eventBroker.Raise(new SelectedTipChangedMessage(this, _comboBox.SelectedItem.Content.Id));

            Close(DialogResult.Ok);
        }
    }
}
