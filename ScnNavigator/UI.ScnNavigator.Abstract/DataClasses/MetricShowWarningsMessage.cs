namespace UI.ScnNavigator.Abstract.DataClasses
{
    public class MetricShowWarningsMessage
    {
        public bool ShowWarnings { get; }

        public MetricShowWarningsMessage(bool showWarnings)
        {
            ShowWarnings = showWarnings;
        }
    }
}
