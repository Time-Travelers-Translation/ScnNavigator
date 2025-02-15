using CrossCutting.Abstract.DataClasses;
using CrossCutting.Core.Contract.EventBrokerage;
using ImGui.Forms.Controls.Base;
using ImGui.Forms.Controls.Layouts;
using Logic.Business.TimeTravelersManagement.Contract.Texts;
using Logic.Business.TranslationManagement.Contract;
using Logic.Domain.GraphVizManagement.Contract;
using Logic.Domain.GraphVizManagement.Contract.DataClasses;
using Logic.Domain.Level5Management.Contract.DataClasses.Scene;
using UI.ScnNavigator.Abstract.DataClasses;
using UI.ScnNavigator.Abstract.Enums;
using UI.ScnNavigator.Components.Contract;
using UI.ScnNavigator.Components.Contract.DataClasses;
using UI.ScnNavigator.Components.InternalContract.Mergers;
using UI.ScnNavigator.Dialogs.Contract;

namespace UI.ScnNavigator.Components.Components
{
    public partial class ChapterForm : Component, ISaveableComponent
    {
        private readonly IEventBroker _eventBroker;
        private readonly IDialogFactory _dialogFactory;
        private readonly IStoryTextManager _originalStoryTextManager;
        private readonly IStoryTranslationManager _storyTranslationManager;
        private readonly IDecisionTranslationManager _decisionTranslationManager;
        private readonly IHintTranslationManager _hintTranslationManager;
        private readonly ITitleTranslationManager _titleTranslationManager;
        private readonly IRouteTranslationManager _routeTranslationManager;
        private readonly ISpeakerTranslationManager _speakerTranslationManager;
        private readonly ITranslationSettingsProvider _translationSettingsProvider;
        private readonly IScenePreviewDataMerger _previewDataMerger;

        private readonly IDictionary<string, Node<SceneEntry>> _nodeLookup;
        private readonly HashSet<string> _changedScenes;

        private readonly Component _decisionPreviewForm;
        private readonly Component _badEndPreviewForm;
        private readonly Component _storyForm;

        private Node<SceneEntry>? _selectedNode;

        public ChapterForm(IList<Node<SceneEntry>> nodes, IEventBroker eventBroker,
            IPreviewComponentFactory previewFactory, IGraphViewComponentFactory graphFactory,
            IGraphSyntaxCreator syntaxCreator, IGraphLayoutCreator layoutCreator,
            IOriginalStoryTextManager originalStoryTextManager, IStoryTranslationManager storyTranslationManager,
            IDecisionTranslationManager decisionTranslationManager, IHintTranslationManager hintTranslationManager,
            ITitleTranslationManager titleTranslationManager, IRouteTranslationManager routeTranslationManager,
            ITranslationSettingsProvider translationSettingsProvider, ISpeakerTranslationManager speakerTranslationManager,
            IScenePreviewDataMerger previewDataMerger, IDialogFactory dialogFactory)
        {
            InitializeComponent(nodes, graphFactory, syntaxCreator, layoutCreator);
            
            _originalStoryTextManager = originalStoryTextManager;
            _storyTranslationManager = storyTranslationManager;
            _decisionTranslationManager = decisionTranslationManager;
            _hintTranslationManager = hintTranslationManager;
            _titleTranslationManager = titleTranslationManager;
            _routeTranslationManager = routeTranslationManager;
            _speakerTranslationManager = speakerTranslationManager;
            _translationSettingsProvider = translationSettingsProvider;
            _eventBroker = eventBroker;
            _dialogFactory = dialogFactory;
            _previewDataMerger = previewDataMerger;

            _nodeLookup = nodes.ToDictionary(x => x.Data.Name, y => y);
            _changedScenes = new HashSet<string>();

            _decisionPreviewForm = previewFactory.CreateDecisionPreview();
            _badEndPreviewForm = previewFactory.CreateBadEndPreview();
            _storyForm = previewFactory.CreateScenePreview();

            eventBroker.Subscribe<SelectedGraphNodeChangedMessage>(ChangeGraphNode);
            eventBroker.Subscribe<SceneChangedMessage>(ChangeScene);
            eventBroker.Subscribe<TranslationEnablementChangedMessage>(_ => UpdateTranslationLayout());
            eventBroker.Subscribe<SceneTextChangedMessage>(message => MarkChangedScene(message.SceneName));
            eventBroker.Subscribe<TextChangedMessage>(message => MarkChangedScene(message.Name));
            eventBroker.Subscribe<SceneDecisionChangedMessage>(UpdateDecisionScene);

            eventBroker.Subscribe<ChapterSaveRequestedMessage>(Save);
            eventBroker.Subscribe<SceneSavedMessage>(message => UnmarkChangedScene(message.SceneName));

            eventBroker.Subscribe<ApplicationDisabledMessage>(_ => ToggleForm(false));
            eventBroker.Subscribe<ApplicationEnabledMessage>(_ => ToggleForm(true));
        }

        private void ToggleForm(bool enabled)
        {
            _graphView.Enabled = enabled;
        }

        private void Save(ChapterSaveRequestedMessage saveRequestMessage)
        {
            if (saveRequestMessage.Sender != this)
                return;

            Save();
        }

        public void Save()
        {
            string[] sceneNames = _changedScenes.ToArray();

            _storyTranslationManager.UpdateStoryText(sceneNames).Wait();
            _decisionTranslationManager.UpdateDecisionText(sceneNames).Wait();
            _titleTranslationManager.UpdateSceneTitle(sceneNames).Wait();
            _hintTranslationManager.UpdateSceneHint(sceneNames).Wait();

            _changedScenes.Clear();

            RaiseChapterSaved();
        }

        private void ChangeGraphNode(SelectedGraphNodeChangedMessage nodeChangeMessage)
        {
            if (!_nodeLookup.TryGetValue(nodeChangeMessage.Node.Label, out _selectedNode))
                return;

            ChangeScene(_selectedNode.Data).Wait();
        }

        private void ChangeScene(SceneChangedMessage sceneChangeMessage)
        {
            if (sceneChangeMessage.Target != _graphView)
                return;

            if (!_nodeLookup.TryGetValue(sceneChangeMessage.Scene, out _selectedNode))
                return;

            ChangeScene(_selectedNode.Data).Wait();
        }

        private async Task ChangeScene(SceneEntry entry)
        {
            await SetTextLayout(entry);

            RaiseMainBranchChanged();
            if (entry.Branches.Length >= 1)
                RaiseBranchChanged(entry.Branches[0]?.Scene.Name);
            RaiseTimeoutDecisionChanged();
        }

        private async void UpdateTranslationLayout()
        {
            if (_selectedNode == null)
                return;

            await SetTextLayout(_selectedNode.Data);
        }

        private async Task SetTextLayout(SceneEntry scene)
        {
            bool hasSecondaryPreview = await CreateSimplePreviewForm(scene);
            bool hasStory = await UpdateScenePreviewForm(scene);

            if (!hasSecondaryPreview && !hasStory)
            {
                if (_mainLayout.Items.Count >= 2)
                    _mainLayout.Items.RemoveAt(1);

                return;
            }

            var textLayout = new StackLayout
            {
                Alignment = Alignment.Horizontal,
                ItemSpacing = 5
            };

            if (hasSecondaryPreview)
            {
                if (IsDecision(scene))
                    textLayout.Items.Add(_decisionPreviewForm);
                else if (IsBadEnd(scene))
                    textLayout.Items.Add(_badEndPreviewForm);
            }

            if (hasStory)
                textLayout.Items.Add(_storyForm);

            if (_mainLayout.Items.Count >= 2)
                _mainLayout.Items[1] = textLayout;
            else
                _mainLayout.Items.Add(textLayout);
        }

        private async Task<bool> CreateSimplePreviewForm(SceneEntry scene)
        {
            switch (scene.Data)
            {
                case SceneEntryDecisionData decisionData:
                    return await UpdateDecisionPreviewForm(scene.Name, decisionData, scene.Branches);

                case SceneEntryBadEndData badEndData:
                    return await UpdateBadEndPreviewForm(scene.Name, badEndData);

                default:
                    return false;
            }
        }

        private bool IsDecision(SceneEntry scene) => scene.Data is SceneEntryDecisionData;

        private bool IsBadEnd(SceneEntry scene) => scene.Data is SceneEntryBadEndData;

        private async Task<bool> UpdateScenePreviewForm(SceneEntry scene)
        {
            if (scene.Data is SceneEntryBadEndData)
                return false;

            string sceneName = scene.Name;

            TextData? title = _originalStoryTextManager.GetTitleText(sceneName);
            StoryTextData[] texts = _originalStoryTextManager.GetStoryTexts(sceneName);

            if (title == null && texts.Length <= 0)
                return false;

            TextData? translatedTitle = null;
            StoryTextData[]? translatedTexts = null;
            TextData? translatedRoute = null;

            if (_translationSettingsProvider.IsTranslationEnabled())
            {
                translatedTitle = await _titleTranslationManager.GetSceneTitle(sceneName);
                translatedTexts = await _storyTranslationManager.GetStoryTexts(sceneName);
                translatedRoute = await _routeTranslationManager.GetRouteName(sceneName);
            }

            ScenePreviewData previewData = _previewDataMerger.Merge(title, texts, translatedTitle, translatedTexts, translatedRoute);
            previewData.SceneName = sceneName;

            if (_translationSettingsProvider.IsTranslationEnabled())
            {
                previewData.TranslatedSpeakers = new TextData?[previewData.Texts.Length];

                for (var i = 0; i < previewData.Texts.Length; i++)
                {
                    if (previewData.Texts[i]?.Speaker == null)
                        continue;

                    previewData.TranslatedSpeakers[i] = await _speakerTranslationManager.GetSpeaker(previewData.Texts[i].Speaker!);
                }
            }

            RaiseScenePreviewChanged(previewData, 0);
            return true;
        }

        private async Task<bool> UpdateDecisionPreviewForm(string sceneName, SceneEntryDecisionData decisionData, SceneEntryBranch?[] branches)
        {
            TextData[]? translatedTexts = null;
            if (_translationSettingsProvider.IsTranslationEnabled())
                translatedTexts = await _decisionTranslationManager.GetDecisions(sceneName);

            var decisionTexts = new TextData[decisionData.Decisions.Length];
            for (var i = 0; i < decisionTexts.Length; i++)
            {
                decisionTexts[i] = new TextData
                {
                    Text = decisionData.Decisions[i]
                };

                if (i < branches.Length)
                    decisionTexts[i].Name = branches[i]?.Scene.Name ?? sceneName;
            }

            var previewData = new DecisionPreviewData
            {
                Name = sceneName,
                Texts = decisionTexts,
                TranslatedTexts = translatedTexts
            };
            RaiseDecisionPreviewChanged(previewData);

            return true;
        }

        private void RaiseDecisionPreviewChanged(DecisionPreviewData data)
        {
            _eventBroker.Raise(new PreviewChangedMessage<DecisionPreviewData>(_decisionPreviewForm, data, 0));
        }

        private async Task<bool> UpdateBadEndPreviewForm(string sceneName, SceneEntryBadEndData badEndData)
        {
            TextData? translatedHint = null;
            if (_translationSettingsProvider.IsTranslationEnabled())
                translatedHint = await _hintTranslationManager.GetSceneHint(sceneName);

            TextData? translatedTitle = null;
            if (_translationSettingsProvider.IsTranslationEnabled())
                translatedTitle = await _titleTranslationManager.GetSceneTitle(sceneName);

            var previewData = new BadEndPreviewData
            {
                Name = sceneName,
                HintText = new TextData { Name = sceneName, Text = badEndData.Hint },
                TitleText = new TextData { Name = sceneName, Text = badEndData.Title },
                TranslatedHintText = translatedHint,
                TranslatedTitleText = translatedTitle
            };
            RaiseBadEndPreviewChanged(previewData);

            return true;
        }

        private void RaiseBadEndPreviewChanged(BadEndPreviewData data)
        {
            _eventBroker.Raise(new PreviewChangedMessage<BadEndPreviewData>(_badEndPreviewForm, data, 0));
        }
        
        private void MarkChangedScene(string sceneName)
        {
            if (!_nodeLookup.ContainsKey(sceneName))
                return;

            _changedScenes.Add(sceneName);

            RaiseChapterChanged();
        }

        private void UnmarkChangedScene(string sceneName)
        {
            _changedScenes.Remove(sceneName);

            if (_changedScenes.Count <= 0)
                RaiseChapterSaved();
        }

        private void UpdateDecisionScene(SceneDecisionChangedMessage message)
        {
            if (message.Sender != _decisionPreviewForm)
                return;

            RaiseBranchChanged(message.Scene);
        }

        private void RaiseChapterChanged()
        {
            _eventBroker.Raise(new ChapterChangedMessage(this));
        }

        private void RaiseChapterSaved()
        {
            _eventBroker.Raise(new ChapterSavedMessage(this));
        }

        private void RaiseScenePreviewChanged(ScenePreviewData previewData, int index)
        {
            _eventBroker.Raise(new PreviewChangedMessage<ScenePreviewData>(_storyForm, previewData, index));
        }

        private void RaiseMainBranchChanged()
        {
            if (_selectedNode == null)
                return;

            if (_selectedNode.Data.Branches.Length <= 0 || _selectedNode.Data.Data is SceneEntryDecisionData)
                return;

            _eventBroker.Raise(new BranchChangedMessage(_graphView, _selectedNode.Data.Name, _selectedNode.Data.Branches[^1]?.Scene.Name, BranchType.Main));
        }

        private void RaiseBranchChanged(string? targetScene)
        {
            if (_selectedNode == null)
                return;

            if (targetScene == null)
                return;

            if (_selectedNode.Data.Data is not SceneEntryDecisionData)
                return;

            if (_selectedNode.Data.Branches.Any(b => b?.Scene.Name.Equals(targetScene, StringComparison.Ordinal) ?? false))
                _eventBroker.Raise(new BranchChangedMessage(_graphView, _selectedNode.Data.Name, targetScene, BranchType.Decision));
        }

        private void RaiseTimeoutDecisionChanged()
        {
            if (_selectedNode == null)
                return;

            if (_selectedNode.Data.Data is not SceneEntryDecisionData decisionData || _selectedNode.Data.Branches.Length == decisionData.Decisions.Length)
                return;

            _eventBroker.Raise(new BranchChangedMessage(_graphView, _selectedNode.Data.Name, _selectedNode.Data.Branches[^1]?.Scene.Name, BranchType.Timeout));
        }
    }
}
