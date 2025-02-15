using CrossCutting.Core.Contract.EventBrokerage;
using Logic.Business.RenderingManagement.Contract;
using Logic.Business.RenderingManagement.Contract.Metrics.DataClasses;
using Logic.Business.RenderingManagement.Contract.Metrics.Enums;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using UI.ScnNavigator.Abstract.DataClasses;
using UI.ScnNavigator.Components.Contract.DataClasses;
using UI.ScnNavigator.Components.Contract.Texts;
using UI.ScnNavigator.Resources.Contract;

namespace UI.ScnNavigator.Components.Previews
{
    internal class MetricNarrationScenePreview : ScenePreview
    {
        private NarrationMetricData? _metricData;
        private bool _showWarnings;

        public MetricNarrationScenePreview(ITextRendererProvider rendererProvider, IScreenResourceProvider screenProvider,
            IEventBroker eventBroker, ISceneTextNormalizer textNormalizer, ITextCharacterReplacer characterReplacer)
            : base(rendererProvider, screenProvider, characterReplacer, eventBroker, textNormalizer)
        {
            eventBroker.Subscribe<MetricShowWarningsMessage>(message => UpdateShowWarnings(message.ShowWarnings));
            eventBroker.Subscribe<MetricPreviewChangedMessage<NarrationMetricData>>(ChangeMetricDetails);
        }

        protected override Image<Rgba32>? CreatePreview()
        {
            Image<Rgba32>? preview = base.CreatePreview();
            if (preview == null || _metricData == null || Index != _metricData.Index)
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

        private void ChangeMetricDetails(MetricPreviewChangedMessage<NarrationMetricData> message)
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
