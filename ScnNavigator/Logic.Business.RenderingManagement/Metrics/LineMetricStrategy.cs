using Logic.Business.RenderingManagement.Contract.Layouts.DataClasses;
using Logic.Business.RenderingManagement.Contract.Metrics;
using Logic.Business.RenderingManagement.Contract.Metrics.DataClasses;
using Logic.Business.RenderingManagement.Contract.Parsers.DataClasses;

namespace Logic.Business.RenderingManagement.Metrics
{
    internal class LineMetricStrategy : BaseMetricStrategy, ILineMetricStrategy
    {
        private readonly IEmptyLineMetricStrategy _emptyLineStrategy;
        private readonly IAutoWrappedLineMetricStrategy _autoWrapLineStrategy;

        public LineMetricStrategy(IEmptyLineMetricStrategy emptyLineStrategy, IAutoWrappedLineMetricStrategy autoWrapLineStrategy)
        {
            _emptyLineStrategy = emptyLineStrategy;
            _autoWrapLineStrategy = autoWrapLineStrategy;
        }

        public override IList<MetricDetailData> Validate(TextLayoutData layout, IList<CharacterData> characters)
        {
            var result = new List<MetricDetailData>();

            result.AddRange(_emptyLineStrategy.Validate(layout, characters));
            result.AddRange(_autoWrapLineStrategy.Validate(layout, characters));

            return result;
        }
    }
}
