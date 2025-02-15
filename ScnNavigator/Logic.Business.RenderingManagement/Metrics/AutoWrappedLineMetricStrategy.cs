using Logic.Business.RenderingManagement.Contract.Metrics;
using Logic.Business.RenderingManagement.Contract.Layouts.DataClasses;
using Logic.Business.RenderingManagement.Contract.Metrics.DataClasses;
using Logic.Business.RenderingManagement.Contract.Parsers.DataClasses;
using Logic.Business.RenderingManagement.Contract.Metrics.Enums;

namespace Logic.Business.RenderingManagement.Metrics
{
    internal class AutoWrappedLineMetricStrategy : BaseMetricStrategy, IAutoWrappedLineMetricStrategy
    {
        public override IList<MetricDetailData> Validate(TextLayoutData layout, IList<CharacterData> characters)
        {
            var result = new List<MetricDetailData>();

            for (var i = 0; i < layout.Lines.Count; i++)
            {
                TextLayoutLineData layoutLine = layout.Lines[i];

                if (layout.Lines[^1] == layoutLine || layoutLine.Characters[^1].Character is LineBreakCharacterData)
                    continue;

                result.Add(CreateMetricDetail(layout.Lines[i + 1], MetricDetailType.AutoWrappedLine));
            }

            return result;
        }
    }
}
