using CrossCutting.Core.Contract.EventBrokerage;
using Logic.Business.RenderingManagement.Contract;
using Logic.Business.RenderingManagement.Contract.Metrics.DataClasses;
using Logic.Business.RenderingManagement.Contract.Metrics.Enums;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Processing;
using UI.ScnNavigator.Abstract.DataClasses;
using UI.ScnNavigator.Components.Contract.DataClasses;
using UI.ScnNavigator.Components.Contract.Texts;
using UI.ScnNavigator.Resources.Contract;

namespace UI.ScnNavigator.Components.Previews
{
    internal class MetricTipPreview : TipPreview
    {
        private TipMetricData? _metricData;
        private bool _showWarnings;

        public MetricTipPreview(IEventBroker eventBroker, ITextCharacterReplacer characterReplacer,
            IScreenResourceProvider screenProvider, ITextRendererProvider rendererProvider)
            : base(eventBroker, characterReplacer, screenProvider, rendererProvider)
        {
            eventBroker.Subscribe<MetricShowWarningsMessage>(message => UpdateShowWarnings(message.ShowWarnings));
            eventBroker.Subscribe<MetricPreviewChangedMessage<TipMetricData>>(ChangeMetricDetails);
        }

        protected override Image<Rgba32> RenderPreview()
        {
            Image<Rgba32> preview = base.RenderPreview();
            if (_metricData == null || Index != (_metricData.IsTitle ? 0 : 1))
                return preview;

            foreach (MetricDetailData detail in _metricData.Metrics)
            {
                if (!_showWarnings && detail.Level == MetricDetailLevel.Warn)
                    continue;

                switch (detail.Level)
                {
                    case MetricDetailLevel.Error:
                        preview.Mutate(x => x
                            .Fill(Color.Red.WithAlpha(.5f), detail.BoundingBox)
                            .Draw(Color.Red, 1, detail.BoundingBox));
                        break;

                    case MetricDetailLevel.Warn:
                        preview.Mutate(x => x
                            .Fill(Color.Gold.WithAlpha(.5f), detail.BoundingBox)
                            .Draw(Color.Gold, 1, detail.BoundingBox));
                        break;
                }
            }

            return preview;
        }

        private void ChangeMetricDetails(MetricPreviewChangedMessage<TipMetricData> message)
        {
            if (message.Target != this)
                return;

            _metricData = message.Metrics;

            UpdatePreview();
        }

        private void UpdateShowWarnings(bool showWarnings)
        {
            _showWarnings = showWarnings;

            UpdatePreview();
        }
    }
}
