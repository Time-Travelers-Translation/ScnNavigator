using CrossCutting.Abstract.DataClasses;
using CrossCutting.Core.Contract.EventBrokerage;
using ImGui.Forms;
using ImGui.Forms.Controls.Base;
using ImGui.Forms.Controls.Tree;
using ImGui.Forms.Localization;
using Logic.Business.RenderingManagement.Contract;
using Logic.Business.RenderingManagement.Contract.Layouts.DataClasses;
using Logic.Business.RenderingManagement.Contract.Layouts;
using Logic.Business.RenderingManagement.Contract.Metrics;
using Logic.Business.RenderingManagement.Contract.Metrics.DataClasses;
using Logic.Business.RenderingManagement.Contract.Parsers;
using Logic.Business.RenderingManagement.Contract.Parsers.DataClasses;
using Logic.Business.TimeTravelersManagement.Contract.Texts;
using Logic.Business.TranslationManagement.Contract;
using SixLabors.ImageSharp;
using UI.ScnNavigator.Abstract.DataClasses;
using UI.ScnNavigator.Components.Contract;
using UI.ScnNavigator.Components.Contract.DataClasses;
using UI.ScnNavigator.Components.InternalContract.Mergers;
using UI.ScnNavigator.Resources.Contract;
using Color = SixLabors.ImageSharp.Color;
using Logic.Business.TimeTravelersManagement.Contract.Enums;

namespace UI.ScnNavigator.Dialogs
{
    internal class TipMetricDialog : BaseMetricDialog<TipMetricData>
    {
        private static readonly Size BoundingBox = new(400, 240);

        private readonly ITipTranslationManager _tipTranslationManager;
        private readonly ITipPreviewDataMerger _previewDataMerger;
        private readonly ITextLayoutCreator? _titleLayoutCreator;
        private readonly ITextLayoutCreator? _textLayoutCreator;
        private readonly ICharacterParser _titleParser;
        private readonly ICharacterParser _textParser;
        private readonly ITipTitleMetricStrategy _titleMetricStrategy;
        private readonly ITipTextMetricStrategy _textMetricStrategy;
        private readonly IOriginalTipTextManager _tipTextManager;
        private readonly IEventBroker _eventBroker;

        private readonly Dictionary<int, TreeNode<TipMetricData>> _tipNodeLookup = new();
        private readonly HashSet<int> _changedTips = new();

        public TipMetricDialog(IEventBroker eventBroker, IPreviewComponentFactory previewFactory, IStringResourceProvider stringProvider,
            ITipTranslationManager tipTranslationManager, ITipPreviewDataMerger previewDataMerger, ITextLayoutCreatorProvider layoutCreatorProvider,
            ICharacterParserProvider parserProvider, ITipTitleMetricStrategy tipTitleMetricStrategy, ITipTextMetricStrategy tipTextMetricStrategy,
            IOriginalTipTextManager tipTextManager)
            : base(eventBroker, previewFactory, stringProvider)
        {
            _tipTranslationManager = tipTranslationManager;
            _previewDataMerger = previewDataMerger;
            _titleLayoutCreator = layoutCreatorProvider.GetTipTitleLayoutCreator(AssetPreference.Patch);
            _textLayoutCreator = layoutCreatorProvider.GetTipTextLayoutCreator(AssetPreference.Patch, AssetPreference.PatchOrOriginal);
            _titleParser = parserProvider.GetDefaultParser();
            _textParser = parserProvider.GetTipParser();
            _titleMetricStrategy = tipTitleMetricStrategy;
            _textMetricStrategy = tipTextMetricStrategy;
            _tipTextManager = tipTextManager;
            _eventBroker = eventBroker;

            eventBroker.Subscribe<TipChangedMessage>(UpdateChangedTip);

            LoadTexts();
        }

        protected override async Task<bool> ShouldCancelClose()
        {
            if (_changedTips.Count <= 0)
                return false;

            return await base.ShouldCancelClose();
        }

        protected override LocalizedString GetTitle(IStringResourceProvider stringProvider)
            => stringProvider.TipMetricDialogCaption();

        protected override Component CreatePreview(IPreviewComponentFactory previewFactory)
            => previewFactory.CreateMetricTipPreview();

        protected override void Save()
        {
            int[] tipIndexes = _changedTips.ToArray();

            _tipTranslationManager.UpdateTips(tipIndexes).Wait();

            foreach (int tipIndex in _changedTips)
                RaiseTipSaved(tipIndex);

            foreach (int tipIndex in tipIndexes)
            {
                ToggleNodeChanged(tipIndex, true, false);
                ToggleNodeChanged(tipIndex, false, false);
            }

            _changedTips.Clear();
        }

        protected override async Task InitializeTexts()
        {
            for (var i = 1; i <= 447; i++)
            {
                // Create preview data
                TextData? title = _tipTextManager.GetTitle(i);
                TextData? text = _tipTextManager.GetText(i);

                if (title == null || text == null)
                    continue;

                TextData? translatedTitle = await _tipTranslationManager.GetTipTitle(i);
                TextData? translatedText = await _tipTranslationManager.GetTipText(i);

                if (translatedTitle == null || translatedText == null)
                    continue;

                TipPreviewData previewData = _previewDataMerger.Merge(title, text, translatedTitle, translatedText);
                previewData.Id = i;

                // Measure TIP Title
                TipMetricData? titleMetricItem = CreateMetricItem(previewData, true);
                if (titleMetricItem is { Metrics.Count: > 0 })
                {
                    var titleMetricNode = new TreeNode<TipMetricData>
                    {
                        Data = titleMetricItem,
                        Text = $"TIP{i:000}"
                    };

                    _tipNodeLookup[GetLookupId(i, true)] = titleMetricNode;
                    AddNode(titleMetricNode);
                }

                // Measure TIP Text
                TipMetricData? textMetricItem = CreateMetricItem(previewData, false);
                if (textMetricItem is { Metrics.Count: > 0 })
                {
                    var textMetricNode = new TreeNode<TipMetricData>
                    {
                        Data = textMetricItem,
                        Text = $"TIP{i:000}"
                    };

                    _tipNodeLookup[GetLookupId(i, false)] = textMetricNode;
                    AddNode(textMetricNode);
                }
            }

            await base.InitializeTexts();
        }

        private TipMetricData? CreateMetricItem(TipPreviewData previewData, bool isTitle)
        {
            IList<MetricDetailData>? metricDetails = CreateMetrics(previewData, isTitle);
            if (metricDetails is not { Count: > 0 })
                return null;

            return new TipMetricData
            {
                PreviewData = previewData,
                TipIndex = previewData.Id,
                IsTitle = isTitle,
                Metrics = metricDetails
            };
        }

        private IList<MetricDetailData>? CreateMetrics(TipPreviewData previewData, bool isTitle)
        {
            ITextLayoutCreator? layoutCreator = GetLayoutCreator(isTitle);
            if (layoutCreator == null)
                return null;

            ICharacterParser textParser = GetParser(isTitle);
            string text = GetText(previewData, isTitle);

            IList<CharacterData> subtitleCharacters = textParser.Parse(text);
            TextLayoutData subtitleLayout = layoutCreator.Create(subtitleCharacters, BoundingBox);

            IMetricStrategy metricStrategy = GetMetricStrategy(isTitle);
            return metricStrategy.Validate(subtitleLayout, subtitleCharacters);
        }

        protected override void RaisePreviewChanged(object target, TipMetricData data)
        {
            _eventBroker.Raise(new PreviewChangedMessage<TipPreviewData>(target, data.PreviewData, data.IsTitle ? 0 : 1));
        }

        private void UpdateChangedTip(TipChangedMessage message)
        {
            if (!_tipNodeLookup.TryGetValue(GetLookupId(message.Id, message.IsTitle), out TreeNode<TipMetricData>? metricNode))
                return;

            MarkChangedTip(message.Id);
            ToggleNodeChanged(message.Id, message.IsTitle, true);

            IList<MetricDetailData>? newMetricDetails = CreateMetrics(metricNode.Data.PreviewData, metricNode.Data.IsTitle);
            if (newMetricDetails is not { Count: > 0 })
                newMetricDetails = Array.Empty<MetricDetailData>();

            metricNode.Data.Metrics = newMetricDetails;

            UpdateMetricDetails(metricNode.Data.Metrics);
            RaiseMetricPreviewChanged(metricNode.Data);
        }

        private void MarkChangedTip(int tipIndex)
        {
            _changedTips.Add(tipIndex);
        }

        private void ToggleNodeChanged(int tipIndex, bool isTitle, bool isChanged)
        {
            if (!_tipNodeLookup.TryGetValue(GetLookupId(tipIndex, isTitle), out TreeNode<TipMetricData>? node))
                return;

            ToggleNodeChanged(node, isChanged);
        }

        private void ToggleNodeChanged(TreeNode<TipMetricData> node, bool isChanged)
        {
            node.TextColor = isChanged ? Color.Orange : new ThemedColor(Color.Black, Color.White);
        }

        private ITextLayoutCreator? GetLayoutCreator(bool isTitle)
        {
            return isTitle ? _titleLayoutCreator : _textLayoutCreator;
        }

        private ICharacterParser GetParser(bool isTitle)
        {
            return isTitle ? _titleParser : _textParser;
        }

        private IMetricStrategy GetMetricStrategy(bool isTitle)
        {
            return isTitle ? _titleMetricStrategy : _textMetricStrategy;
        }

        private string GetText(TipPreviewData previewData, bool isTitle)
        {
            return isTitle
                ? previewData.TranslatedTipTitle?.Text ?? previewData.TipTitle.Text ?? string.Empty
                : previewData.TranslatedTipText?.Text ?? previewData.TipText.Text ?? string.Empty;
        }

        private int GetLookupId(int tipIndex, bool isTitle)
        {
            return tipIndex * 2 + (isTitle ? 0 : 1);
        }

        private void RaiseTipSaved(int tipIndex)
        {
            _eventBroker.Raise(new TipSavedMessage(tipIndex));
        }
    }
}
