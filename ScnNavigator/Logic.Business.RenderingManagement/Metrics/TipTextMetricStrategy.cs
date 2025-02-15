using Logic.Business.RenderingManagement.Contract.Metrics;
using Logic.Business.RenderingManagement.Contract.Layouts.DataClasses;
using Logic.Business.RenderingManagement.Contract.Metrics.DataClasses;
using Logic.Business.RenderingManagement.Contract.Parsers.DataClasses;

namespace Logic.Business.RenderingManagement.Metrics
{
    internal class TipTextMetricStrategy : ITipTextMetricStrategy
    {
        private readonly ITagMetricStrategy _tagStrategy;
        private readonly IInvalidCharacterMetricStrategy _invalidCharacterStrategy;
        private readonly ISpaceMetricStrategy _spaceStrategy;
        private readonly IAutoWrappedLineMetricStrategy _autoWrapLineStrategy;
        private readonly IPunctuationMetricStrategy _punctuationStrategy;

        public TipTextMetricStrategy(ITagMetricStrategy tagStrategy, IInvalidCharacterMetricStrategy invalidCharacterStrategy,
            ISpaceMetricStrategy spaceStrategy, IAutoWrappedLineMetricStrategy autoWrapLineStrategy, IPunctuationMetricStrategy punctuationStrategy)
        {
            _tagStrategy = tagStrategy;
            _invalidCharacterStrategy = invalidCharacterStrategy;
            _spaceStrategy = spaceStrategy;
            _autoWrapLineStrategy = autoWrapLineStrategy;
            _punctuationStrategy = punctuationStrategy;
        }

        public IList<MetricDetailData> Validate(TextLayoutData layout, IList<CharacterData> characters)
        {
            var details = new List<MetricDetailData>();

            details.AddRange(_tagStrategy.Validate(layout, characters));
            details.AddRange(_invalidCharacterStrategy.Validate(layout, characters));
            details.AddRange(_spaceStrategy.Validate(layout, characters));
            details.AddRange(_autoWrapLineStrategy.Validate(layout, characters));
            details.AddRange(_punctuationStrategy.Validate(layout, characters));

            return details;
        }
    }
}
