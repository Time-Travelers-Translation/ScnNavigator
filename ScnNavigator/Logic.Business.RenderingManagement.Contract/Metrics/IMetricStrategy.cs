using Logic.Business.RenderingManagement.Contract.Layouts.DataClasses;
using Logic.Business.RenderingManagement.Contract.Metrics.DataClasses;
using Logic.Business.RenderingManagement.Contract.Parsers.DataClasses;

namespace Logic.Business.RenderingManagement.Contract.Metrics
{
    public interface IMetricStrategy
    {
        IList<MetricDetailData> Validate(TextLayoutData layout, IList<CharacterData> characters);
    }
}
