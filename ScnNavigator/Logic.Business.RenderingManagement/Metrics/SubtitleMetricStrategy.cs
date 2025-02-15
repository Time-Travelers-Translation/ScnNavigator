using Logic.Business.RenderingManagement.Contract.Layouts.DataClasses;
using Logic.Business.RenderingManagement.Contract.Metrics;
using Logic.Business.RenderingManagement.Contract.Metrics.DataClasses;
using Logic.Business.RenderingManagement.Contract.Metrics.Enums;
using Logic.Business.RenderingManagement.Contract.Parsers.DataClasses;

namespace Logic.Business.RenderingManagement.Metrics
{
    internal class SubtitleMetricStrategy : BaseMetricStrategy, ISubtitleMetricStrategy
    {
        private readonly ITagMetricStrategy _tagStrategy;
        private readonly IInvalidCharacterMetricStrategy _invalidCharacterStrategy;
        private readonly ISpaceMetricStrategy _spaceStrategy;
        private readonly ILineMetricStrategy _lineStrategy;
        private readonly IPunctuationMetricStrategy _punctuationStrategy;

        public SubtitleMetricStrategy(ITagMetricStrategy tagStrategy, IInvalidCharacterMetricStrategy invalidCharacterStrategy,
            ISpaceMetricStrategy spaceStrategy, ILineMetricStrategy lineStrategy, IPunctuationMetricStrategy punctuationStrategy)
        {
            _tagStrategy = tagStrategy;
            _invalidCharacterStrategy = invalidCharacterStrategy;
            _spaceStrategy = spaceStrategy;
            _lineStrategy = lineStrategy;
            _punctuationStrategy = punctuationStrategy;
        }

        public override IList<MetricDetailData> Validate(TextLayoutData layout, IList<CharacterData> characters)
        {
            var details = new List<MetricDetailData>();

            details.AddRange(_tagStrategy.Validate(layout, characters));
            details.AddRange(_invalidCharacterStrategy.Validate(layout, characters));
            details.AddRange(_spaceStrategy.Validate(layout, characters));
            details.AddRange(_lineStrategy.Validate(layout, characters));
            details.AddRange(_punctuationStrategy.Validate(layout, characters));

            if (layout.Lines.Count <= 3)
                return details;

            for (var i = 3; i < layout.Lines.Count; i++)
                details.Add(CreateMetricDetail(layout.Lines[i], MetricDetailType.TooManyLines));

            return details;
        }
    }
}
