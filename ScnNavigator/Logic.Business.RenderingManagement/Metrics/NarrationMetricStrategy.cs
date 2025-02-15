using Logic.Business.RenderingManagement.Contract.Layouts.DataClasses;
using Logic.Business.RenderingManagement.Contract.Metrics;
using Logic.Business.RenderingManagement.Contract.Metrics.DataClasses;
using Logic.Business.RenderingManagement.Contract.Parsers.DataClasses;

namespace Logic.Business.RenderingManagement.Metrics
{
    internal class NarrationMetricStrategy : BaseMetricStrategy, INarrationMetricStrategy
    {
        private readonly ITagMetricStrategy _tagStrategy;
        private readonly IInvalidCharacterMetricStrategy _invalidCharacterStrategy;
        private readonly ISpaceMetricStrategy _spaceStrategy;
        private readonly ILineMetricStrategy _lineStrategy;
        private readonly IPunctuationMetricStrategy _punctuationStrategy;

        public NarrationMetricStrategy(ITagMetricStrategy tagStrategy, IInvalidCharacterMetricStrategy invalidCharacterStrategy,
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

            return details;
        }
    }
}
