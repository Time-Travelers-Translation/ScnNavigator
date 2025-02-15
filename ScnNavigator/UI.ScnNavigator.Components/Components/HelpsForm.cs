using CrossCutting.Abstract.DataClasses;
using CrossCutting.Core.Contract.EventBrokerage;
using ImGui.Forms.Controls.Base;
using Logic.Business.TranslationManagement.Contract;
using Logic.Domain.Level5Management.Contract.ConfigBinary;
using UI.ScnNavigator.Abstract.DataClasses;
using UI.ScnNavigator.Components.Contract;
using UI.ScnNavigator.Components.Contract.DataClasses;
using Logic.Business.TimeTravelersManagement.Contract.DataClasses;
using UI.ScnNavigator.Components.InternalContract.Mergers;
using Logic.Business.TimeTravelersManagement.Contract.Texts;
using Logic.Domain.Level5Management.Contract.DataClasses.ConfigBinary;
using Logic.Domain.Level5Management.Contract.Enums.ConfigBinary;

namespace UI.ScnNavigator.Components.Components
{
    internal partial class HelpsForm : Component, ISaveableComponent
    {
        private readonly HelpTitlesData _helpTitles;

        private readonly IEventBroker _eventBroker;
        private readonly IHelpTranslationManager _helpTranslationManager;
        private readonly IHelpPreviewDataMerger _previewDataMerger;
        private readonly ITranslationSettingsProvider _translationSettingsProvider;
        private readonly IPreviewComponentFactory _previewFactory;
        private readonly IConfigurationReader<RawConfigurationEntry> _configReader;
        private readonly IEventTextParser _textParser;

        private readonly HashSet<int> _changedHelps;

        private readonly Component _helpPreview;

        private int _index;

        public HelpsForm(HelpTitlesData data, IEventBroker eventBroker, IPreviewComponentFactory previewFactory, IHelpTranslationManager helpTranslationManager,
            IConfigurationReader<RawConfigurationEntry> configReader, IEventTextParser textParser, ITranslationSettingsProvider translationSettingsProvider,
            IHelpPreviewDataMerger previewDataMerger)
        {
            InitializeComponent(data.HelpTitles);

            _helpTitles = data;
            _eventBroker = eventBroker;
            _helpTranslationManager = helpTranslationManager;
            _previewDataMerger = previewDataMerger;
            _translationSettingsProvider = translationSettingsProvider;
            _previewFactory = previewFactory;
            _configReader = configReader;
            _textParser = textParser;

            _helpPreview = previewFactory.CreateHelpPreview();

            _changedHelps = new HashSet<int>();

            _previousHelpButton.Clicked += (s, e) => UpdateHelpIndex(_index - 1);
            _nextHelpButton.Clicked += (s, e) => UpdateHelpIndex(_index + 1);
            _helpTitleTreeView.SelectedNodeChanged += UpdateSelectedNode;

            eventBroker.Subscribe<TranslationEnablementChangedMessage>(_ => UpdateHelpIndex(_index));
            eventBroker.Subscribe<HelpChangedMessage>(MarkChangedHelp);
            eventBroker.Subscribe<ApplicationDisabledMessage>(_ => ToggleForm(false));
            eventBroker.Subscribe<ApplicationEnabledMessage>(_ => ToggleForm(true));

            UpdateHelpIndex(0);
        }

        private void ToggleForm(bool enabled)
        {
            _helpTitleTreeView.Enabled = enabled;

            if (!enabled)
            {
                _previousHelpButton.Enabled = false;
                _nextHelpButton.Enabled = false;
            }
            else
                UpdateArrowButtons();
        }

        private void MarkChangedHelp(HelpChangedMessage message)
        {
            if (message.Sender != _helpPreview)
                return;

            _changedHelps.Add(message.Id);

            RaiseFileChanged();
        }

        public void Save()
        {
            int[] helps = _changedHelps.ToArray();
            _helpTranslationManager.UpdateHelps(helps).Wait();

            _changedHelps.Clear();

            RaiseFileSaved();
        }

        private void UpdateSelectedNode(object sender, EventArgs e)
        {
            int selectedNodeIndex = _helpTitleTreeView.Nodes.IndexOf(_helpTitleTreeView.SelectedNode);
            if (selectedNodeIndex < 0 || selectedNodeIndex >= _helpTitleTreeView.Nodes.Count)
                return;

            UpdateHelpIndex(selectedNodeIndex);
        }

        private void UpdateHelpIndex(int index)
        {
            _index = Math.Clamp(index, 0, _helpTitles.HelpTitles.Length - 1);

            UpdateArrowButtons();

            SetSelectedNode(_index);

            HelpTitleData helpTitle = _helpTitles.HelpTitles[_index];
            string helpPath = Path.Combine(_helpTitles.HelpPath, $"HELP{helpTitle.Id + 1:000}_ja.cfg.bin");

            if (!TryLoadRawConfiguration(helpPath, out EventTextConfiguration? helpConfig) || helpConfig!.Texts.Length <= 0)
            {
                _helpPanel.Content = null;
                return;
            }

            TextData? translatedHelpTitle = null;
            TextData[]? translatedHelpText = null;
            if (_translationSettingsProvider.IsTranslationEnabled())
            {
                translatedHelpTitle = _helpTranslationManager.GetHelpTitle(_index).Result;
                translatedHelpText = _helpTranslationManager.GetHelpTexts(_index).Result;
            }

            HelpPreviewData helpData = _previewDataMerger.Merge(helpTitle.Text, helpConfig.Texts, translatedHelpTitle, translatedHelpText);
            helpData.Id = helpTitle.Id;

            RaiseHelpPreviewChanged(helpData);
            _helpPanel.Content = _helpPreview;
        }

        private void RaiseHelpPreviewChanged(HelpPreviewData data)
        {
            _eventBroker.Raise(new PreviewChangedMessage<HelpPreviewData>(_helpPreview, data, 0));
        }

        private void UpdateArrowButtons()
        {
            _previousHelpButton.Enabled = _index > 0;
            _nextHelpButton.Enabled = _index < _helpTitles.HelpTitles.Length - 1;
        }

        private bool TryLoadRawConfiguration(string filePath, out EventTextConfiguration? tipConfig)
        {
            tipConfig = null;

            try
            {
                using Stream fileStream = File.OpenRead(filePath);

                Configuration<RawConfigurationEntry> config = _configReader.Read(fileStream, StringEncoding.Sjis);
                tipConfig = _textParser.Parse(config);
            }
            catch
            {
                return false;
            }

            return true;
        }

        private void SetSelectedNode(int index)
        {
            _helpTitleTreeView.SelectedNodeChanged -= UpdateSelectedNode;
            _helpTitleTreeView.SelectedNode = _helpTitleTreeView.Nodes[index];
            _helpTitleTreeView.SelectedNodeChanged += UpdateSelectedNode;
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
