using Logic.Business.RenderingManagement.Contract.Layouts.DataClasses;
using Logic.Business.RenderingManagement.Contract.Metrics;
using Logic.Business.RenderingManagement.Contract.Metrics.DataClasses;
using Logic.Business.RenderingManagement.Contract.Metrics.Enums;
using Logic.Business.RenderingManagement.Contract.Parsers.DataClasses;

namespace Logic.Business.RenderingManagement.Metrics
{
    internal class InvalidCharacterMetricStrategy : BaseMetricStrategy, IInvalidCharacterMetricStrategy
    {
        public override IList<MetricDetailData> Validate(TextLayoutData layout, IList<CharacterData> characters)
        {
            var result = new List<MetricDetailData>();

            foreach (TextLayoutLineData layoutLine in layout.Lines)
            {
                foreach (TextLayoutCharacterData layoutCharacter in layoutLine.Characters)
                {
                    if (!layoutCharacter.Character.IsVisible)
                        continue;

                    ushort character;
                    switch (layoutCharacter.Character)
                    {
                        case SubtitleFontCharacterData subtitleCharacter:
                            character = subtitleCharacter.Character;
                            break;

                        case FontCharacterData fontCharacter:
                            character = fontCharacter.Character;
                            break;

                        default:
                            continue;
                    }

                    if (character is '「' or '」' or '“' or '”' or ';' or '’' or '‘')
                        result.Add(CreateMetricDetail(layoutCharacter, MetricDetailType.InvalidCharacter));
                }
            }

            return result;
        }
    }
}
