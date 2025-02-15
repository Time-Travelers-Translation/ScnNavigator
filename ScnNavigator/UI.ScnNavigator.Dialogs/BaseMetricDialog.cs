using ImGui.Forms.Controls.Layouts;
using ImGui.Forms.Controls.Tree;
using ImGui.Forms.Controls;
using ImGui.Forms.Controls.Base;
using ImGui.Forms.Modals;
using ImGui.Forms.Models;
using ImGui.Forms.Models.IO;
using UI.ScnNavigator.Components.Contract;
using UI.ScnNavigator.Components.Contract.DataClasses;
using UI.ScnNavigator.Resources.Contract;
using Veldrid;
using Logic.Business.RenderingManagement.Contract.Metrics.Enums;
using System.Collections.Concurrent;
using CrossCutting.Core.Contract.EventBrokerage;
using ImGui.Forms.Localization;
using UI.ScnNavigator.Abstract.DataClasses;
using Logic.Business.RenderingManagement.Contract.Metrics.DataClasses;

namespace UI.ScnNavigator.Dialogs
{
    internal abstract class BaseMetricDialog<TData> : Modal
        where TData : MetricData
    {
        private static readonly KeyCommand SaveCommand = new(ModifierKeys.Control, Key.S);

        private readonly IEventBroker _eventBroker;
        private readonly IStringResourceProvider _stringProvider;

        private readonly ConcurrentQueue<TreeNode<TData>> _addedNodes = new();
        private readonly List<TreeNode<TData>> _totalNodes = new();
        private readonly CancellationTokenSource _cancellationTokenSource = new();

        private CheckBox _showWarnsCheckBox;
        private TreeView<TData> _metricsTree;
        private Label _loadingLabel;
        private ImGui.Forms.Controls.Lists.List<Label> _metricList;
        private Component _preview;

        private StackLayout _listLayout;
        private StackLayout _previewLayout;
        private StackLayout _mainLayout;

        public BaseMetricDialog(IEventBroker eventBroker, IPreviewComponentFactory previewFactory, IStringResourceProvider stringProvider)
        {
            _eventBroker = eventBroker;
            _stringProvider = stringProvider;

            InitializeLayout(previewFactory, stringProvider);

            eventBroker.Subscribe<ApplicationDisabledMessage>(_ => UpdateForm(false));
            eventBroker.Subscribe<ApplicationEnabledMessage>(_ => UpdateForm(true));
        }
        
        protected abstract LocalizedString GetTitle(IStringResourceProvider stringProvider);
        
        protected abstract Component CreatePreview(IPreviewComponentFactory previewFactory);

        protected void LoadTexts()
        {
            Task.Run(InitializeTexts, _cancellationTokenSource.Token);
        }

        protected virtual Task InitializeTexts()
        {
            _loadingLabel.Text = string.Empty;
            _showWarnsCheckBox.Enabled = true;

            return Task.CompletedTask;
        }

        protected abstract void Save();

        protected override void UpdateInternal(Rectangle contentRect)
        {
            if (SaveCommand.IsPressed())
                Save();

            while (_addedNodes.TryDequeue(out TreeNode<TData>? queuedNode))
            {
                _totalNodes.Add(queuedNode);
                if (!_showWarnsCheckBox.Checked)
                {
                    if (queuedNode.Data.Metrics.Any(m => m.Level != MetricDetailLevel.Warn))
                        _metricsTree.Nodes.Add(queuedNode);
                }
                else
                    _metricsTree.Nodes.Add(queuedNode);
            }

            base.UpdateInternal(contentRect);
        }

        protected override Task CloseInternal()
        {
            _cancellationTokenSource.Cancel(false);

            return base.CloseInternal();
        }

        protected override async Task<bool> ShouldCancelClose()
        {
            RaiseApplicationDisabled();

            DialogResult result = await GetUnsavedChangesResult();

            if (result == DialogResult.Cancel)
            {
                RaiseApplicationEnabled();

                return true;
            }

            if (result == DialogResult.Yes)
                Save();

            RaiseApplicationEnabled();

            return false;
        }

        protected void AddNode(TreeNode<TData> node)
        {
            _addedNodes.Enqueue(node);
        }

        private void InitializeLayout(IPreviewComponentFactory previewFactory, IStringResourceProvider stringProvider)
        {
            _preview = CreatePreview(previewFactory);

            _metricList = new ImGui.Forms.Controls.Lists.List<Label>
            {
                Size = new ImGui.Forms.Models.Size(SizeValue.Parent, SizeValue.Content),
                Alignment = Alignment.Horizontal,
                ItemSpacing = 5
            };

            _showWarnsCheckBox = new CheckBox { Text = stringProvider.MetricDialogShowWarningsCaption(), Enabled = false };
            _metricsTree = new TreeView<TData>();
            _loadingLabel = new Label { Text = stringProvider.MetricDialogLoadingCaption() };

            _listLayout = new StackLayout
            {
                Alignment = Alignment.Vertical,
                ItemSpacing = 5,
                Items =
                {
                    _showWarnsCheckBox,
                    new StackItem(_metricsTree) { Size = ImGui.Forms.Models.Size.Parent },
                    _loadingLabel
                }
            };
            _previewLayout = new StackLayout
            {
                Alignment = Alignment.Vertical,
                ItemSpacing = 5,
                Items =
                {
                    _metricList
                }
            };

            _mainLayout = new StackLayout
            {
                Alignment = Alignment.Horizontal,
                ItemSpacing = 5,
                Items =
                {
                    new StackItem(_listLayout) { Size = new ImGui.Forms.Models.Size(.2f, SizeValue.Parent) },
                    _previewLayout
                }
            };

            Content = _mainLayout;
            Caption = GetTitle(stringProvider);
            Size = new ImGui.Forms.Models.Size(SizeValue.Relative(.75f), SizeValue.Relative(.75f));

            _showWarnsCheckBox.CheckChanged += ShowWarnsCheckBox_CheckChanged;
            _metricsTree.SelectedNodeChanged += MetricsTree_SelectedNodeChanged;
        }

        private async Task<DialogResult> GetUnsavedChangesResult()
        {
            LocalizedString caption = _stringProvider.FileCloseUnsavedChangesCaption();
            LocalizedString text = _stringProvider.FileCloseUnsavedChangesText();
            return await MessageBox.ShowYesNoCancelAsync(caption, text);
        }

        #region Updates

        private void UpdateForm(bool toggle)
        {
            _metricsTree.Enabled = toggle;
        }

        protected void UpdatePreview(TData metricItem)
        {
            if (_previewLayout.Items.Count <= 1)
                _previewLayout.Items.Add(_preview);

            UpdateMetricDetails(metricItem.Metrics);

            RaisePreviewChanged(_preview, metricItem);
            RaiseMetricPreviewChanged(metricItem);
        }

        protected void UpdateMetricDetails(IList<MetricDetailData> metricDetails)
        {
            _metricList.Items.Clear();

            foreach (MetricDetailData detail in metricDetails)
                _metricList.Items.Add(new Label { Text = $"{detail.Type}" });
        }

        #endregion

        #region Events

        private void MetricsTree_SelectedNodeChanged(object? sender, EventArgs e)
        {
            if (_metricsTree.SelectedNode == null)
                return;

            UpdatePreview(_metricsTree.SelectedNode.Data);
        }

        private void ShowWarnsCheckBox_CheckChanged(object? sender, EventArgs e)
        {
            _metricsTree.Nodes.Clear();

            foreach (TreeNode<TData> node in _totalNodes)
            {
                if (!_showWarnsCheckBox.Checked)
                {
                    if (node.Data.Metrics.Any(m => m.Level != MetricDetailLevel.Warn))
                        _metricsTree.Nodes.Add(node);
                }
                else
                    _metricsTree.Nodes.Add(node);
            }

            RaiseShowMetricWarnings(_showWarnsCheckBox.Checked);
        }

        #endregion

        #region Raise messages

        protected abstract void RaisePreviewChanged(object target, TData data);

        private void RaiseShowMetricWarnings(bool showWarnings)
        {
            _eventBroker.Raise(new MetricShowWarningsMessage(showWarnings));
        }

        protected void RaiseMetricPreviewChanged(TData metrics)
        {
            _eventBroker.Raise(new MetricPreviewChangedMessage<TData>(_preview, metrics));
        }

        private void RaiseApplicationDisabled()
        {
            _eventBroker.Raise(new ApplicationDisabledMessage());
        }

        private void RaiseApplicationEnabled()
        {
            _eventBroker.Raise(new ApplicationEnabledMessage());
        }

        #endregion
    }
}
