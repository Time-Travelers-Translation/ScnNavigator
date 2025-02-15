using Logic.Business.RenderingManagement.Contract.Layouts.DataClasses;
using Logic.Business.RenderingManagement.Contract.Metrics;
using Logic.Business.RenderingManagement.Contract.Metrics.DataClasses;
using Logic.Business.RenderingManagement.Contract.Metrics.Enums;
using Logic.Business.RenderingManagement.Contract.Parsers.DataClasses;

namespace Logic.Business.RenderingManagement.Metrics
{
    internal class SpaceMetricStrategy : BaseMetricStrategy, ISpaceMetricStrategy
    {
        public override IList<MetricDetailData> Validate(TextLayoutData layout, IList<CharacterData> characters)
        {
            var result = new List<MetricDetailData>();

            foreach (TextLayoutLineData layoutLine in layout.Lines)
            {
                TextLayoutCharacterData? startCharacter = null;
                TextLayoutCharacterData? endCharacter = null;

                var spaceCount = 0;

                var isStartOfLine = true;
                foreach (TextLayoutCharacterData layoutCharacter in layoutLine.Characters)
                {
                    if (layoutCharacter.Character is not FontCharacterData fontCharacter)
                        continue;

                    if (layoutCharacter.BoundingBox.Width <= 0)
                        continue;

                    if (fontCharacter.Character is not ' ')
                    {
                        // Detected a single space before the visible line
                        if (spaceCount > 0 && isStartOfLine)
                            result.Add(CreateMetricDetail(startCharacter!, endCharacter!, MetricDetailType.MisplacedSpace));

                        // Detected multiple spaces before or in the line, not at the end
                        if (spaceCount > 1)
                            result.Add(CreateMetricDetail(startCharacter!, endCharacter!, MetricDetailType.ContinuousSpaces));

                        startCharacter = null;
                        endCharacter = null;

                        spaceCount = 0;

                        isStartOfLine = false;
                        continue;
                    }

                    if (spaceCount <= 0)
                        startCharacter = layoutCharacter;
                    endCharacter = layoutCharacter;

                    spaceCount++;
                }

                if (spaceCount <= 0)
                    continue;

                result.Add(CreateMetricDetail(startCharacter!, endCharacter!,
                    spaceCount == 1 ? MetricDetailType.MisplacedSpace : MetricDetailType.ContinuousSpaces));
            }

            return result;
        }
    }
}
