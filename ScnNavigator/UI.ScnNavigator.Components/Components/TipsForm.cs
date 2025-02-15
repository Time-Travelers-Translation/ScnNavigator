using CrossCutting.Abstract.DataClasses;
using CrossCutting.Core.Contract.EventBrokerage;
using ImGui.Forms.Controls.Base;
using Logic.Business.TimeTravelersManagement.Contract.DataClasses;
using Logic.Business.TimeTravelersManagement.Contract.Texts;
using Logic.Business.TranslationManagement.Contract;
using Logic.Domain.Level5Management.Contract.ConfigBinary;
using Logic.Domain.Level5Management.Contract.DataClasses.ConfigBinary;
using UI.ScnNavigator.Abstract.DataClasses;
using UI.ScnNavigator.Components.Contract;
using UI.ScnNavigator.Components.Contract.DataClasses;
using UI.ScnNavigator.Components.InternalContract.Mergers;
using UI.ScnNavigator.Dialogs.Contract;
using UI.ScnNavigator.Dialogs.Contract.DataClasses;

namespace UI.ScnNavigator.Components.Components
{
    public partial class TipsForm : Component, ISaveableComponent
    {
        private readonly TipTitlesData _tipTitles;
        private readonly IList<TipTitleEntryData> _tipTitleEntries;

        private readonly IEventBroker _eventBroker;
        private readonly IDialogFactory _dialogFactory;
        private readonly ITipTranslationManager _tipTranslationManager;
        private readonly ITranslationSettingsProvider _translationSettingsProvider;
        private readonly IConfigurationReader<RawConfigurationEntry> _configReader;
        private readonly IEventTextParser _textParser;
        private readonly ITipPreviewDataMerger _previewDataMerger;
        private readonly IOriginalTipTextManager _tipTextManager;

        private readonly HashSet<int> _changedTips;

        private readonly Component _previewForm;

        private int _index;

        public TipsForm(TipTitlesData data, IEventBroker eventBroker, IDialogFactory dialogFactory, IPreviewComponentFactory previewFactory,
            ITipTranslationManager tipTranslationManager, IConfigurationReader<RawConfigurationEntry> configReader, IEventTextParser textParser,
            ITranslationSettingsProvider translationSettingsProvider, ITipPreviewDataMerger previewDataMerger, IOriginalTipTextManager tipTextManager)
        {
            InitializeComponent(data.TipTitles);

            _tipTitles = data;
            _eventBroker = eventBroker;
            _dialogFactory = dialogFactory;
            _tipTranslationManager = tipTranslationManager;
            _translationSettingsProvider = translationSettingsProvider;
            _configReader = configReader;
            _textParser = textParser;
            _previewDataMerger = previewDataMerger;
            _tipTextManager = tipTextManager;

            _previewForm = previewFactory.CreateTipPreview();

            _changedTips = new HashSet<int>();

            _previousTipButton.Clicked += (s, e) => UpdateTipIndex(_index - 1);
            _nextTipButton.Clicked += (s, e) => UpdateTipIndex(_index + 1);
            _tipTitleTreeView.SelectedNodeChanged += UpdateSelectedNode;

            _tipTitleEntries = CreateTitleEntries();

            eventBroker.Subscribe<TipSavedMessage>(UnmarkChangedTip);
            eventBroker.Subscribe<TranslationEnablementChangedMessage>(_ => UpdateTipIndex(_index));
            eventBroker.Subscribe<TipChangedMessage>(MarkChangedTip);
            eventBroker.Subscribe<SelectedTipChangedMessage>(ChangeSelectedTip);
            eventBroker.Subscribe<ApplicationDisabledMessage>(_ => ToggleForm(false));
            eventBroker.Subscribe<ApplicationEnabledMessage>(_ => ToggleForm(true));

            UpdateTipIndex(0);
        }

        private IList<TipTitleEntryData> CreateTitleEntries()
        {
            var titles = new List<TipTitleEntryData>();

            for (var i = 0; i < _tipTitles.TipTitles.Length; i++)
                titles.Add(CreateTitleEntryData(i));

            return titles;
        }

        private void ToggleForm(bool enabled)
        {
            _tipTitleTreeView.Enabled = enabled;

            if (!enabled)
            {
                _previousTipButton.Enabled = false;
                _nextTipButton.Enabled = false;
            }
            else
                UpdateArrowButtons();
        }

        private void UnmarkChangedTip(TipSavedMessage message)
        {
            _changedTips.Remove(message.TipIndex);

            if (_changedTips.Count <= 0)
                RaiseFileSaved();
        }

        private void MarkChangedTip(TipChangedMessage message)
        {
            if (message.Sender != _previewForm)
                return;

            _changedTips.Add(message.Id);

            RaiseFileChanged();
        }

        private void ChangeSelectedTip(SelectedTipChangedMessage message)
        {
            if (message.Sender != _tipDialog)
                return;

            SetSelectedNode(message.Id - 1);
            UpdateTipIndex(message.Id - 1);
        }

        public void Save()
        {
            int[] tips = _changedTips.ToArray();
            _tipTranslationManager.UpdateTips(tips).Wait();

            _changedTips.Clear();

            RaiseFileSaved();
        }

        private void UpdateSelectedNode(object sender, EventArgs e)
        {
            int selectedNodeIndex = _tipTitleTreeView.Nodes.IndexOf(_tipTitleTreeView.SelectedNode);
            if (selectedNodeIndex < 0 || selectedNodeIndex >= _tipTitleTreeView.Nodes.Count)
                return;

            UpdateTipIndex(selectedNodeIndex);
        }

        private void UpdateTipIndex(int index)
        {
            _index = Math.Clamp(index, 0, _tipTitles.TipTitles.Length - 1);

            UpdateArrowButtons();

            SetSelectedNode(_index);

            TipTitleData tipTitle = _tipTitles.TipTitles[_index];

            var titleData = new TextData { Text = tipTitle.Text };
            TextData? textData = _tipTextManager.GetText(tipTitle.Id);
            if (textData == null)
                return;

            TextData? translatedTipTitle = null;
            TextData? translatedTipText = null;
            if (_translationSettingsProvider.IsTranslationEnabled())
            {
                translatedTipTitle = _tipTranslationManager.GetTipTitle(_index + 1).Result;
                translatedTipText = _tipTranslationManager.GetTipText(_index + 1).Result;
            }

            TipPreviewData tipData = _previewDataMerger.Merge(titleData, textData, translatedTipTitle, translatedTipText);
            tipData.Id = tipTitle.Id;

            RaiseTipPreviewChanged(tipData);

            _tipPanel.Content = _previewForm;
        }

        private void RaiseTipPreviewChanged(TipPreviewData data)
        {
            _eventBroker.Raise(new PreviewChangedMessage<TipPreviewData>(_previewForm, data, 0));
        }

        private TipTitleEntryData CreateTitleEntryData(int index)
        {
            TipTitleData tipTitle = _tipTitles.TipTitles[index];

            var entryData = new TipTitleEntryData
            {
                Id = tipTitle.Id,
                TipTitle = new TextData { Name = $"Title{tipTitle.Id:000}", Text = tipTitle.Text }
            };

            if (_translationSettingsProvider.IsTranslationEnabled())
            {
                TextData? translatedTipTitle = _tipTranslationManager.GetTipTitle(index + 1).Result;

                entryData.TranslatedTipTitle = translatedTipTitle;
            }

            return entryData;
        }

        private void UpdateArrowButtons()
        {
            _previousTipButton.Enabled = _index > 0;
            _nextTipButton.Enabled = _index < _tipTitles.TipTitles.Length - 1;
        }

        private void SetSelectedNode(int index)
        {
            _tipTitleTreeView.SelectedNodeChanged -= UpdateSelectedNode;
            _tipTitleTreeView.SelectedNode = _tipTitleTreeView.Nodes[index];
            _tipTitleTreeView.SelectedNodeChanged += UpdateSelectedNode;
        }

        private void RaiseFileChanged()
        {
            _eventBroker.Raise(new FileChangedMessage(this));
        }

        private void RaiseFileSaved()
        {
            _eventBroker.Raise(new FileSavedMessage(this));
        }
    }
}
