namespace UI.ScnNavigator.Components.Contract.DataClasses
{
    public class TipMetricData : MetricData
    {
        public TipPreviewData PreviewData { get; set; }
        public int TipIndex { get; set; }
        public bool IsTitle { get; set; }
    }
}
