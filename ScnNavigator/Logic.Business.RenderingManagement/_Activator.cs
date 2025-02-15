using CrossCutting.Core.Contract.Bootstrapping;
using CrossCutting.Core.Contract.Configuration;
using CrossCutting.Core.Contract.DependencyInjection;
using CrossCutting.Core.Contract.DependencyInjection.DataClasses;
using CrossCutting.Core.Contract.EventBrokerage;
using Logic.Business.RenderingManagement.Contract;
using Logic.Business.RenderingManagement.Contract.Layouts;
using Logic.Business.RenderingManagement.Contract.Metrics;
using Logic.Business.RenderingManagement.Contract.Parsers;
using Logic.Business.RenderingManagement.Contract.Renderers;
using Logic.Business.RenderingManagement.InternalContract.DataClasses;
using Logic.Business.RenderingManagement.InternalContract.Layouts;
using Logic.Business.RenderingManagement.InternalContract.Parsers;
using Logic.Business.RenderingManagement.InternalContract.Renderers;
using Logic.Business.RenderingManagement.Layouts;
using Logic.Business.RenderingManagement.Metrics;
using Logic.Business.RenderingManagement.Parsers;
using Logic.Business.RenderingManagement.Renderers;

namespace Logic.Business.RenderingManagement
{
    public class RenderingManagementActivator : IComponentActivator
    {
        public void Activating()
        {
        }

        public void Activated()
        {
        }

        public void Deactivating()
        {
        }

        public void Deactivated()
        {
        }

        public void Register(ICoCoKernel kernel)
        {
            kernel.Register<ICharacterParserProvider, CharacterParserProvider>(ActivationScope.Unique);
            kernel.Register<ITextRendererProvider, TextRendererProvider>(ActivationScope.Unique);
            kernel.Register<ITextLayoutCreatorProvider, TextLayoutCreatorProvider>(ActivationScope.Unique);

            kernel.Register<ICharacterParser, CharacterParser<CharacterContext>>(ActivationScope.Unique);
            kernel.Register<INarrationCharacterParser, NarrationCharacterParser>(ActivationScope.Unique);
            kernel.Register<ISubtitleCharacterParser, SubtitleCharacterParser>(ActivationScope.Unique);
            kernel.Register<ITipCharacterParser, TipCharacterParser>(ActivationScope.Unique);

            kernel.Register<ITextLayoutCreator, TextLayoutCreator>();
            kernel.Register<ITipTextLayoutCreator, TipTextLayoutCreator>();
            kernel.Register<IHintTextLayoutCreator, HintTextLayoutCreator>();
            kernel.Register<ISubtitleTextLayoutCreator, SubtitleTextLayoutCreator>();

            kernel.Register<ITextRenderer, TextRenderer>();
            kernel.Register<IDecisionTextRenderer, DecisionTextRenderer>(ActivationScope.Unique);
            kernel.Register<ITipTextRenderer, TipTextRenderer>();

            kernel.Register<ITagMetricStrategy, TagMetricStrategy>(ActivationScope.Unique);
            kernel.Register<IInvalidCharacterMetricStrategy, InvalidCharacterMetricStrategy>(ActivationScope.Unique);
            kernel.Register<ISpaceMetricStrategy, SpaceMetricStrategy>(ActivationScope.Unique);
            kernel.Register<IEmptyLineMetricStrategy, EmptyLineMetricStrategy>(ActivationScope.Unique);
            kernel.Register<IAutoWrappedLineMetricStrategy, AutoWrappedLineMetricStrategy>(ActivationScope.Unique);
            kernel.Register<ILineMetricStrategy, LineMetricStrategy>(ActivationScope.Unique);
            kernel.Register<IPunctuationMetricStrategy, PunctuationMetricStrategy>(ActivationScope.Unique);

            kernel.Register<ISubtitleMetricStrategy, SubtitleMetricStrategy>(ActivationScope.Unique);
            kernel.Register<INarrationMetricStrategy, NarrationMetricStrategy>(ActivationScope.Unique);
            kernel.Register<ITipTextMetricStrategy, TipTextMetricStrategy>(ActivationScope.Unique);
            kernel.Register<ITipTitleMetricStrategy, TipTitleMetricStrategy>(ActivationScope.Unique);

            kernel.RegisterConfiguration<RenderingManagementConfiguration>();
        }

        public void AddMessageSubscriptions(IEventBroker broker)
        {
        }

        public void Configure(IConfigurator configurator)
        {
        }
    }
}
