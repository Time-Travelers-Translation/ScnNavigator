using CrossCutting.Abstract.DataClasses;
using CrossCutting.Core.Contract.EventBrokerage;
using ImGui.Forms;
using ImGui.Forms.Controls.Base;
using ImGui.Forms.Controls.Tree;
using ImGui.Forms.Localization;
using Logic.Business.RenderingManagement.Contract;
using Logic.Business.RenderingManagement.Contract.Layouts;
using Logic.Business.RenderingManagement.Contract.Layouts.DataClasses;
using Logic.Business.RenderingManagement.Contract.Metrics;
using Logic.Business.RenderingManagement.Contract.Metrics.DataClasses;
using Logic.Business.RenderingManagement.Contract.Parsers;
using Logic.Business.RenderingManagement.Contract.Parsers.DataClasses;
using Logic.Business.TimeTravelersManagement.Contract.Enums;
using Logic.Business.TimeTravelersManagement.Contract.Texts;
using Logic.Business.TranslationManagement.Contract;
using SixLabors.ImageSharp;
using UI.ScnNavigator.Abstract.DataClasses;
using UI.ScnNavigator.Components.Contract;
using UI.ScnNavigator.Components.Contract.DataClasses;
using UI.ScnNavigator.Components.Contract.Texts;
using UI.ScnNavigator.Components.InternalContract.Mergers;
using UI.ScnNavigator.Resources.Contract;

namespace UI.ScnNavigator.Dialogs
{
    internal class SubtitleMetricDialog : BaseMetricDialog<SubtitleMetricData>
    {
        private static readonly Size BoundingBox = new(400, 240);

        private readonly IEventBroker _eventBroker;
        private readonly IOriginalStoryTextManager _originalStoryTextManager;
        private readonly IStoryTranslationManager _storyTranslationManager;
        private readonly ISpeakerTranslationManager _speakerTranslationManager;
        private readonly IScenePreviewDataMerger _previewDataMerger;
        private readonly ISubtitleMetricStrategy _metricStrategy;
        private readonly ISceneTextNormalizer _textNormalizer;
        private readonly ICharacterParser _subtitleParser;
        private readonly ITextLayoutCreator? _layoutProvider;
        private readonly ITextLayoutCreator? _introLayoutProvider;

        private readonly Dictionary<string, TreeNode<SubtitleMetricData>> _eventNodeLookup = new();
        private readonly HashSet<string> _changedScenes = new();
        private readonly HashSet<string> _changedEvents = new();

        public SubtitleMetricDialog(IEventBroker eventBroker, IPreviewComponentFactory previewFactory, IStringResourceProvider stringProvider,
            IOriginalStoryTextManager originalStoryTextManager, IStoryTranslationManager storyTranslationManager, ISpeakerTranslationManager speakerTranslationManager,
            IScenePreviewDataMerger previewDataMerger, ISubtitleMetricStrategy metricStrategy, ISceneTextNormalizer textNormalizer,
            ICharacterParserProvider parserProvider, ITextLayoutCreatorProvider layoutProvider)
            : base(eventBroker, previewFactory, stringProvider)
        {
            _eventBroker = eventBroker;
            _originalStoryTextManager = originalStoryTextManager;
            _storyTranslationManager = storyTranslationManager;
            _speakerTranslationManager = speakerTranslationManager;
            _previewDataMerger = previewDataMerger;
            _metricStrategy = metricStrategy;
            _textNormalizer = textNormalizer;
            _subtitleParser = parserProvider.GetSubtitleParser();
            _layoutProvider = layoutProvider.GetSubtitleLayoutCreator(AssetPreference.Patch);
            _introLayoutProvider = layoutProvider.GetIntroSubtitleLayoutCreator(AssetPreference.Patch);

            eventBroker.Subscribe<SceneTextChangedMessage>(UpdateChangedScene);
            eventBroker.Subscribe<SpeakerChangedMessage>(UpdateChangedSpeaker);

            LoadTexts();
        }

        protected override async Task<bool> ShouldCancelClose()
        {
            if (_changedScenes.Count <= 0 && _changedEvents.Count <= 0)
                return false;

            return await base.ShouldCancelClose();
        }

        protected override void Save()
        {
            string[] sceneNames = _changedScenes.ToArray();
            string[] eventNames = _changedEvents.ToArray();

            _storyTranslationManager.UpdateStoryText(sceneNames).Wait();

            foreach (string sceneName in sceneNames)
                RaiseSceneSaved(sceneName);

            foreach (string eventName in eventNames)
                ToggleNodeChanged(eventName, false);

            _changedScenes.Clear();
            _changedEvents.Clear();
        }

        protected override LocalizedString GetTitle(IStringResourceProvider stringProvider)
            => stringProvider.SubtitleMetricDialogCaption();

        protected override Component CreatePreview(IPreviewComponentFactory previewFactory)
            => previewFactory.CreateMetricSubtitleScenePreview();

        protected override async Task InitializeTexts()
        {
            for (var i = 1; i <= 7; i++)
            {
                IDictionary<string, StoryTextData[]> translatedTexts = await _storyTranslationManager.GetStoryTexts(i);

                foreach (string sceneName in translatedTexts.Keys)
                {
                    StoryTextData[] texts = _originalStoryTextManager.GetStoryTexts(sceneName);
                    ScenePreviewData previewData = await CreatePreviewData(sceneName, texts, translatedTexts[sceneName]);

                    for (var j = 0; j < previewData.Texts.Length; j++)
                    {
                        if (previewData.TranslatedTexts?[j]?.Text == "<remove>")
                            continue;

                        SubtitleMetricData? metricItem = CreateMetricItem(previewData, j);
                        if (metricItem == null || metricItem.Metrics.Count <= 0)
                            continue;

                        var metricNode = new TreeNode<SubtitleMetricData>
                        {
                            Data = metricItem,
                            Text = metricItem.EventName
                        };

                        _eventNodeLookup[metricItem.EventName] = metricNode;

                        AddNode(metricNode);
                    }
                }
            }

            await base.InitializeTexts();
        }

        protected override void RaisePreviewChanged(object target, SubtitleMetricData data)
        {
            _eventBroker.Raise(new PreviewChangedMessage<ScenePreviewData>(target, data.PreviewData, data.Index));
        }

        private void RaiseSceneSaved(string sceneName)
        {
            _eventBroker.Raise(new SceneSavedMessage(sceneName));
        }

        private void UpdateChangedScene(SceneTextChangedMessage message)
        {
            UpdateChangedScene(message.SceneName, message.EventName);
        }

        private void UpdateChangedSpeaker(SpeakerChangedMessage message)
        {
            UpdateChangedScene(message.SceneName, message.EventName);
        }

        void UpdateChangedScene(string sceneName, string? eventName)
        {
            if (eventName == null || !_eventNodeLookup.TryGetValue(eventName, out TreeNode<SubtitleMetricData>? metricNode))
                return;

            MarkChangedScene(sceneName, eventName);
            ToggleNodeChanged(metricNode, true);

            IList<MetricDetailData>? newMetricDetails = CreateMetrics(metricNode.Data.PreviewData, metricNode.Data.Index);
            if (newMetricDetails is not { Count: > 0 })
                newMetricDetails = Array.Empty<MetricDetailData>();

            metricNode.Data.Metrics = newMetricDetails;

            UpdateMetricDetails(metricNode.Data.Metrics);
            RaiseMetricPreviewChanged(metricNode.Data);
        }

        private void ToggleNodeChanged(string? eventName, bool isChanged)
        {
            if (eventName == null || !_eventNodeLookup.TryGetValue(eventName, out TreeNode<SubtitleMetricData>? node))
                return;

            ToggleNodeChanged(node, isChanged);
        }

        private void ToggleNodeChanged(TreeNode<SubtitleMetricData> node, bool isChanged)
        {
            node.TextColor = isChanged ? Color.Orange : new ThemedColor(Color.Black, Color.White);
        }

        private void MarkChangedScene(string sceneName, string eventName)
        {
            _changedScenes.Add(sceneName);
            _changedEvents.Add(eventName);
        }

        private SubtitleMetricData? CreateMetricItem(ScenePreviewData previewData, int index)
        {
            IList<MetricDetailData>? metricDetails = CreateMetrics(previewData, index);
            if (metricDetails is not { Count: > 0 })
                return null;

            string? eventName = previewData.Texts[index]?.Name ?? previewData.TranslatedTexts?[index]?.Name;
            if (eventName == null)
                return null;

            return new SubtitleMetricData
            {
                PreviewData = previewData,
                EventName = eventName,
                Index = index,
                Metrics = metricDetails
            };
        }

        private IList<MetricDetailData>? CreateMetrics(ScenePreviewData previewData, int index)
        {
            string? normalizedSubtitle = _textNormalizer.Normalize(previewData.Texts[index], previewData.TranslatedTexts?[index], previewData.TranslatedSpeakers?[index]);
            if (normalizedSubtitle == null)
                return null;

            if (!IsSubtitle(normalizedSubtitle))
                return null;

            ITextLayoutCreator? layoutCreator = GetLayoutCreator(previewData.SceneName);
            if (layoutCreator == null)
                return null;

            IList<CharacterData> subtitleCharacters = _subtitleParser.Parse(normalizedSubtitle);
            TextLayoutData subtitleLayout = layoutCreator.Create(subtitleCharacters, BoundingBox);

            return _metricStrategy.Validate(subtitleLayout, subtitleCharacters);
        }

        private async Task<ScenePreviewData> CreatePreviewData(string sceneName, StoryTextData[] texts, StoryTextData[] translatedTexts)
        {
            ScenePreviewData previewData = _previewDataMerger.Merge(null, texts, null, translatedTexts, null);

            previewData.SceneName = sceneName;
            previewData.TranslatedSpeakers = new TextData?[previewData.Texts.Length];

            for (var j = 0; j < previewData.Texts.Length; j++)
            {
                if (previewData.Texts[j]?.Speaker == null)
                    continue;

                previewData.TranslatedSpeakers[j] = await _speakerTranslationManager.GetSpeaker(previewData.Texts[j]!.Speaker!);
            }

            return previewData;
        }

        private bool IsSubtitle(string? text)
        {
            if (text == null)
                return false;

            return text.Contains('「') || text.Contains('“');
        }

        private ITextLayoutCreator? GetLayoutCreator(string sceneName)
        {
            return sceneName.StartsWith("A01")
                ? _introLayoutProvider
                : _layoutProvider;
        }
    }
}
