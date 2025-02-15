using CrossCutting.Abstract.DataClasses;
using CrossCutting.Core.Contract.EventBrokerage;
using ImGui.Forms.Controls.Base;
using ImGui.Forms.Controls.Layouts;
using Logic.Business.TimeTravelersManagement.Contract.DataClasses;
using Logic.Business.TranslationManagement.Contract;
using Logic.Domain.GraphVizManagement.Contract;
using Logic.Domain.GraphVizManagement.Contract.DataClasses;
using UI.ScnNavigator.Abstract.DataClasses;
using UI.ScnNavigator.Abstract.Enums;
using UI.ScnNavigator.Components.Contract;
using UI.ScnNavigator.Components.Contract.DataClasses;
using UI.ScnNavigator.Components.Previews;

namespace UI.ScnNavigator.Components.Components
{
    public partial class TtpCallForm : Component, ISaveableComponent
    {
        private readonly TtpCallData _data;

        private readonly IEventBroker _eventBroker;
        private readonly IPostStoryTranslationManager _postStoryTranslationManager;
        private readonly ITranslationSettingsProvider _translationSettingsProvider;
        private readonly IPreviewComponentFactory _previewFactory;

        private readonly Dictionary<string, Node<TtpCallSectionData>> _textNodeLookup;
        private readonly Dictionary<int, Node<TtpCallSectionData>> _idNodeLookup;

        private Component _decisionPreview;

        public TtpCallForm(TtpCallData data, IEventBroker eventBroker, IPostStoryTranslationManager postStoryTranslationManager,
            IGraphViewComponentFactory graphFactory, IPreviewComponentFactory previewFactory,
            IGraphSyntaxCreator syntaxCreator, IGraphLayoutCreator layoutCreator, ITranslationSettingsProvider translationSettingsProvider)
        {
            IList<Node<TtpCallSectionData>> nodes = CreateNodes(data);

            InitializeComponent(nodes, graphFactory, syntaxCreator, layoutCreator);

            _data = data;

            _eventBroker = eventBroker;
            _postStoryTranslationManager = postStoryTranslationManager;
            _translationSettingsProvider = translationSettingsProvider;
            _previewFactory = previewFactory;

            _textNodeLookup = nodes.ToDictionary(x => x.Text, y => y);
            _idNodeLookup = nodes.ToDictionary(x => x.Data.Id, y => y);

            _decisionPreview = previewFactory.CreateDecisionPreview();

            eventBroker.Subscribe<SelectedGraphNodeChangedMessage>(message => ChangeGraphNode(message.Node));
        }

        public void Save()
        {

        }

        private void ChangeGraphNode(GraphNode node)
        {
            if (!_textNodeLookup.TryGetValue(node.Label, out Node<TtpCallSectionData>? sectionNode))
                return;

            SetTextLayout(sectionNode.Data).Wait();

            RaiseMainBranchChanged(sectionNode);
            if (sectionNode.Data.Blocks[^1].Decisions.Length >= 1)
                RaiseDecisionChanged(sectionNode, sectionNode.Data.Blocks[^1].Decisions[0].NextSectionId);
            RaiseTimeoutDecisionChanged(sectionNode);
        }

        private async Task SetTextLayout(TtpCallSectionData sectionData)
        {
            bool hasDecision = await CreateDecisionPreviewForm(sectionData);
            ScenePreview? storyForm = null;//await CreateScenePreviewForm(sectionData);

            if (!hasDecision && storyForm == null)
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

            if (hasDecision)
                textLayout.Items.Add(_decisionPreview);

            if (storyForm != null)
                textLayout.Items.Add(storyForm);

            if (_mainLayout.Items.Count >= 2)
                _mainLayout.Items[1] = textLayout;
            else
                _mainLayout.Items.Add(textLayout);
        }

        private async Task<bool> CreateDecisionPreviewForm(TtpCallSectionData sectionData)
        {
            if (sectionData.Blocks[^1].Decisions.Length <= 0)
                return false;

            if (!_idNodeLookup.TryGetValue(sectionData.Id, out Node<TtpCallSectionData>? sectionNode))
                return false;

            TextData[]? translatedTexts = null;
            if (_translationSettingsProvider.IsTranslationEnabled())
                translatedTexts = await _postStoryTranslationManager.GetPostDecisions(_data.Name, sectionData.Id);

            var decisionTexts = new List<TextData>();
            foreach (TtpCallDecisionEntry decisionData in sectionData.Blocks[^1].Decisions)
            {
                if (decisionData.Text == "……")
                    continue;

                if (!_idNodeLookup.TryGetValue(decisionData.NextSectionId, out Node<TtpCallSectionData>? decisionNode))
                    continue;

                decisionTexts.Add(new TextData
                {
                    Name = decisionNode.Text,
                    Text = decisionData.Text
                });
            }

            RaiseDecisionPreviewChanged(new DecisionPreviewData
            {
                Name = sectionNode.Text,
                Texts = decisionTexts.ToArray(),
                TranslatedTexts = translatedTexts
            });
            return true;
        }

        private void RaiseDecisionPreviewChanged(DecisionPreviewData previewData)
        {
            _eventBroker.Raise(new PreviewChangedMessage<DecisionPreviewData>(_decisionPreview, previewData, 0));
        }

        private void RaiseMainBranchChanged(Node<TtpCallSectionData> sourceNode)
        {
            TtpCallBlockData lastBlock = sourceNode.Data.Blocks[^1];
            if (lastBlock.Decisions.Length > 0 || lastBlock.Wards.Length <= 0)
                return;

            TtpCallWardData lastWard = lastBlock.Wards[^1];
            if (!_idNodeLookup.TryGetValue(lastWard.NextSectionId, out Node<TtpCallSectionData>? targetNode))
                return;

            _eventBroker.Raise(new BranchChangedMessage(_graphView, sourceNode.Text, targetNode.Text, BranchType.Main));
        }

        private void RaiseDecisionChanged(Node<TtpCallSectionData> sourceNode, int? targetSectionId)
        {
            if (targetSectionId == null)
                return;

            TtpCallBlockData lastBlock = sourceNode.Data.Blocks[^1];
            if (lastBlock.Decisions.Length <= 0)
                return;

            if (!_idNodeLookup.TryGetValue(targetSectionId.Value, out Node<TtpCallSectionData>? targetNode))
                return;

            _eventBroker.Raise(new BranchChangedMessage(_graphView, sourceNode.Text, targetNode.Text, BranchType.Decision));
        }

        private void RaiseTimeoutDecisionChanged(Node<TtpCallSectionData> sourceNode)
        {
            TtpCallBlockData lastBlock = sourceNode.Data.Blocks[^1];
            if (lastBlock.Decisions.Length <= 0)
                return;

            TtpCallDecisionEntry? timeoutDecisionData = lastBlock.Decisions.FirstOrDefault(x => x.Text == "……");
            if (timeoutDecisionData == null)
                return;

            if (!_idNodeLookup.TryGetValue(timeoutDecisionData.NextSectionId, out Node<TtpCallSectionData>? targetNode))
                return;

            _eventBroker.Raise(new BranchChangedMessage(_graphView, sourceNode.Text, targetNode.Text, BranchType.Timeout));
        }

        private IList<Node<TtpCallSectionData>> CreateNodes(TtpCallData data)
        {
            IDictionary<int, Node<TtpCallSectionData>> lookup =
                data.Sections.ToDictionary(x => x.Id, y => new Node<TtpCallSectionData>(y)
                {
                    Text = $"{data.Name}_{y.Id:0000}"
                });

            foreach (TtpCallSectionData sectionData in data.Sections)
            {
                if (!lookup.TryGetValue(sectionData.Id, out Node<TtpCallSectionData>? node))
                    continue;

                foreach (TtpCallDecisionEntry decisionData in sectionData.Blocks[^1].Decisions)
                {
                    if (!lookup.TryGetValue(decisionData.NextSectionId, out Node<TtpCallSectionData>? decisionNode))
                        continue;

                    node.AddChild(decisionNode);
                }

                if (sectionData.Blocks[^1].Wards.Length <= 0)
                    continue;

                TtpCallWardData lastWard = sectionData.Blocks[^1].Wards[^1];
                if (!lookup.TryGetValue(lastWard.NextSectionId, out Node<TtpCallSectionData>? nextSectionNode))
                    continue;

                node.AddChild(nextSectionNode);
            }

            return lookup.Values.ToArray();
        }
    }
}
