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
    internal partial class TutorialsForm : Component, ISaveableComponent
    {
        private readonly TutorialTitlesData _tutorialTitles;

        private readonly IEventBroker _eventBroker;
        private readonly ITutorialTranslationManager _tutorialTranslationManager;
        private readonly ITutorialPreviewDataMerger _previewDataMerger;
        private readonly ITranslationSettingsProvider _translationSettingsProvider;
        private readonly IPreviewComponentFactory _previewFactory;
        private readonly IConfigurationReader<RawConfigurationEntry> _configReader;
        private readonly IEventTextParser _textParser;

        private readonly HashSet<int> _changedTutorials;

        private readonly Component _previewForm;

        private int _index;

        public TutorialsForm(TutorialTitlesData data, IEventBroker eventBroker, IPreviewComponentFactory previewFactory, ITutorialTranslationManager tutorialTranslationManager,
            IConfigurationReader<RawConfigurationEntry> configReader, IEventTextParser textParser, ITranslationSettingsProvider translationSettingsProvider,
            ITutorialPreviewDataMerger previewDataMerger)
        {
            InitializeComponent(data.TutorialTitles);

            _tutorialTitles = data;
            _eventBroker = eventBroker;
            _tutorialTranslationManager = tutorialTranslationManager;
            _previewDataMerger = previewDataMerger;
            _translationSettingsProvider = translationSettingsProvider;
            _previewFactory = previewFactory;
            _configReader = configReader;
            _textParser = textParser;

            _previewForm = _previewFactory.CreateTutorialPreview();

            _changedTutorials = new HashSet<int>();

            _previousTutorialButton.Clicked += (s, e) => UpdateTutorialIndex(_index - 1);
            _nextTutorialButton.Clicked += (s, e) => UpdateTutorialIndex(_index + 1);
            _tutorialTitleTreeView.SelectedNodeChanged += UpdateSelectedNode;

            eventBroker.Subscribe<TranslationEnablementChangedMessage>(_ => UpdateTutorialIndex(_index));
            eventBroker.Subscribe<TutorialChangedMessage>(MarkChangedTutorial);
            eventBroker.Subscribe<ApplicationDisabledMessage>(_ => ToggleForm(false));
            eventBroker.Subscribe<ApplicationEnabledMessage>(_ => ToggleForm(true));

            UpdateTutorialIndex(0);
        }

        private void ToggleForm(bool enabled)
        {
            _tutorialTitleTreeView.Enabled = enabled;

            if (!enabled)
            {
                _previousTutorialButton.Enabled = false;
                _nextTutorialButton.Enabled = false;
            }
            else
                UpdateArrowButtons();
        }

        private void MarkChangedTutorial(TutorialChangedMessage message)
        {
            if (message.Sender != _previewForm)
                return;

            _changedTutorials.Add(message.Id);

            RaiseFileChanged();
        }

        public void Save()
        {
            int[] tutorials = _changedTutorials.ToArray();
            _tutorialTranslationManager.UpdateTutorials(tutorials).Wait();

            _changedTutorials.Clear();

            RaiseFileSaved();
        }

        private void UpdateSelectedNode(object sender, EventArgs e)
        {
            int selectedNodeIndex = _tutorialTitleTreeView.Nodes.IndexOf(_tutorialTitleTreeView.SelectedNode);
            if (selectedNodeIndex < 0 || selectedNodeIndex >= _tutorialTitleTreeView.Nodes.Count)
                return;

            UpdateTutorialIndex(selectedNodeIndex);
        }

        private void UpdateTutorialIndex(int index)
        {
            _index = Math.Clamp(index, 0, _tutorialTitles.TutorialTitles.Length - 1);

            UpdateArrowButtons();

            SetSelectedNode(_index);

            TutorialTitleData tutorialTitle = _tutorialTitles.TutorialTitles[_index];
            string helpPath = Path.Combine(_tutorialTitles.TutorialPath, $"TUTO{tutorialTitle.Id + 1:000}_ja.cfg.bin");

            if (!TryLoadRawConfiguration(helpPath, out EventTextConfiguration? tutorialConfig) || tutorialConfig!.Texts.Length <= 0)
            {
                _tutorialPanel.Content = null;
                return;
            }

            TextData? translatedTutorialTitle = null;
            TextData[]? translatedTutorialText = null;
            if (_translationSettingsProvider.IsTranslationEnabled())
            {
                translatedTutorialTitle = _tutorialTranslationManager.GetTutorialTitle(_index).Result;
                translatedTutorialText = _tutorialTranslationManager.GetTutorialTexts(_index).Result;
            }

            TutorialPreviewData tutorialData = _previewDataMerger.Merge(tutorialTitle.Text, tutorialConfig.Texts, translatedTutorialTitle, translatedTutorialText);
            tutorialData.Id = tutorialTitle.Id;

            RaiseTutorialPreviewChanged(tutorialData);

            _tutorialPanel.Content = _previewForm;
        }

        private void RaiseTutorialPreviewChanged(TutorialPreviewData tutorialData)
        {
            _eventBroker.Raise(new PreviewChangedMessage<TutorialPreviewData>(_previewForm, tutorialData, 0));
        }

        private void UpdateArrowButtons()
        {
            _previousTutorialButton.Enabled = _index > 0;
            _nextTutorialButton.Enabled = _index < _tutorialTitles.TutorialTitles.Length - 1;
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
            _tutorialTitleTreeView.SelectedNodeChanged -= UpdateSelectedNode;
            _tutorialTitleTreeView.SelectedNode = _tutorialTitleTreeView.Nodes[index];
            _tutorialTitleTreeView.SelectedNodeChanged += UpdateSelectedNode;
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
