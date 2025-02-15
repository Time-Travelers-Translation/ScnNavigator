using Logic.Business.RenderingManagement.Contract.Layouts.DataClasses;
using Logic.Business.RenderingManagement.Contract.Metrics;
using Logic.Business.RenderingManagement.Contract.Metrics.DataClasses;
using Logic.Business.RenderingManagement.Contract.Metrics.Enums;
using Logic.Business.RenderingManagement.Contract.Parsers.DataClasses;

namespace Logic.Business.RenderingManagement.Metrics
{
    internal class TagMetricStrategy : BaseMetricStrategy, ITagMetricStrategy
    {
        public override IList<MetricDetailData> Validate(TextLayoutData layout, IList<CharacterData> characters)
        {
            var result = new List<MetricDetailData>();

            foreach (TextLayoutLineData layoutLine in layout.Lines)
            {
                foreach (TextLayoutCharacterData layoutCharacter in layoutLine.Characters)
                {
                    if (layoutCharacter.Character is not FontCharacterData fontCharacter)
                        continue;

                    switch (fontCharacter.Character)
                    {
                        case '<':
                            result.Add(CreateMetricDetail(layoutCharacter, MetricDetailType.InvalidOpenTag));
                            break;

                        case '>':
                            result.Add(CreateMetricDetail(layoutCharacter, MetricDetailType.InvalidCloseTag));
                            break;
                    }
                }
            }

            return result;
        }
    }
}
