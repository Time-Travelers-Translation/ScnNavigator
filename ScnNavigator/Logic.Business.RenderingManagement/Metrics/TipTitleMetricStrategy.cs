using Logic.Business.RenderingManagement.Contract.Layouts.DataClasses;
using Logic.Business.RenderingManagement.Contract.Metrics;
using Logic.Business.RenderingManagement.Contract.Metrics.DataClasses;
using Logic.Business.RenderingManagement.Contract.Parsers.DataClasses;

namespace Logic.Business.RenderingManagement.Metrics
{
    internal class TipTitleMetricStrategy: ITipTitleMetricStrategy
    {
        private readonly IInvalidCharacterMetricStrategy _invalidCharacterStrategy;
        private readonly ISpaceMetricStrategy _spaceStrategy;
        private readonly IPunctuationMetricStrategy _punctuationStrategy;

        public TipTitleMetricStrategy(IInvalidCharacterMetricStrategy invalidCharacterStrategy, IPunctuationMetricStrategy punctuationStrategy, 
            ISpaceMetricStrategy spaceStrategy)
        {
            _invalidCharacterStrategy = invalidCharacterStrategy;
            _spaceStrategy = spaceStrategy;
            _punctuationStrategy = punctuationStrategy;
        }

        public IList<MetricDetailData> Validate(TextLayoutData layout, IList<CharacterData> characters)
        {
            var details = new List<MetricDetailData>();
            
            details.AddRange(_invalidCharacterStrategy.Validate(layout, characters));
            details.AddRange(_spaceStrategy.Validate(layout, characters));
            details.AddRange(_punctuationStrategy.Validate(layout, characters));

            return details;
        }
    }
}
