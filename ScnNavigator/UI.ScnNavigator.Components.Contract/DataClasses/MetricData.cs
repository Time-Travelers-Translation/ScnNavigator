using Logic.Business.RenderingManagement.Contract.Metrics.DataClasses;

namespace UI.ScnNavigator.Components.Contract.DataClasses
{
    public abstract class MetricData
    {
        public IList<MetricDetailData> Metrics { get; set; }
    }
}
