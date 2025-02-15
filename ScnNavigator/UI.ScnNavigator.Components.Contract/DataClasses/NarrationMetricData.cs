namespace UI.ScnNavigator.Components.Contract.DataClasses
{
    public class NarrationMetricData : MetricData
    {
        public ScenePreviewData PreviewData { get; set; }
        public string EventName { get; set; }
        public int Index { get; set; }
    }
}
