using UI.ScnNavigator.Components.Contract.DataClasses;

namespace UI.ScnNavigator.Abstract.DataClasses
{
    public class MetricPreviewChangedMessage<TData>
        where TData : MetricData
    {
        public object Target { get; }
        public TData Metrics { get; }

        public MetricPreviewChangedMessage(object target, TData metrics)
        {
            Target = target;
            Metrics = metrics;
        }
    }
}
