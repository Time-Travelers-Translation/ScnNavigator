using Logic.Business.RenderingManagement.Contract.Layouts.DataClasses;
using Logic.Business.RenderingManagement.Contract.Metrics;
using Logic.Business.RenderingManagement.Contract.Metrics.DataClasses;
using Logic.Business.RenderingManagement.Contract.Metrics.Enums;
using Logic.Business.RenderingManagement.Contract.Parsers.DataClasses;

namespace Logic.Business.RenderingManagement.Metrics
{
    internal class EmptyLineMetricStrategy : BaseMetricStrategy, IEmptyLineMetricStrategy
    {
        public override IList<MetricDetailData> Validate(TextLayoutData layout, IList<CharacterData> characters)
        {
            var result = new List<MetricDetailData>();

            foreach (TextLayoutLineData layoutLine in layout.Lines)
            {
                if (layoutLine.Characters.Count > 1) 
                    continue;

                if (layoutLine.Characters is [{ Character: not LineBreakCharacterData }])
                    continue;

                result.Add(CreateMetricDetail(layoutLine, MetricDetailType.EmptyLine));
            }

            return result;
        }
    }
}
