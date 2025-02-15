using CrossCutting.Core.Contract.Bootstrapping;
using CrossCutting.Core.Contract.Configuration;
using CrossCutting.Core.Contract.DependencyInjection;
using CrossCutting.Core.Contract.DependencyInjection.DataClasses;
using CrossCutting.Core.Contract.EventBrokerage;
using Logic.Business.TranslationManagement.Contract;

namespace Logic.Business.TranslationManagement
{
    public class TranslationManagementActivator : IComponentActivator
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
            kernel.Register<IDecisionTranslationManager, DecisionTranslationManager>(ActivationScope.Unique);
            kernel.Register<IHintTranslationManager, HintTranslationManager>(ActivationScope.Unique);
            kernel.Register<IOutlineTranslationManager, OutlineTranslationManager>(ActivationScope.Unique);
            kernel.Register<IPostStoryTranslationManager, PostStoryTranslationManager>(ActivationScope.Unique);
            kernel.Register<IRouteTranslationManager, RouteTranslationManager>(ActivationScope.Unique);
            kernel.Register<ISpeakerTranslationManager, SpeakerTranslationManager>(ActivationScope.Unique);
            kernel.Register<IStaffrollTranslationManager, StaffrollTranslationManager>(ActivationScope.Unique);
            kernel.Register<IStoryTranslationManager, StoryTranslationManager>(ActivationScope.Unique);
            kernel.Register<ITipTranslationManager, TipTranslationManager>(ActivationScope.Unique);
            kernel.Register<IHelpTranslationManager, HelpTranslationManager>(ActivationScope.Unique);
            kernel.Register<ITutorialTranslationManager, TutorialTranslationManager>(ActivationScope.Unique);
            kernel.Register<ITitleTranslationManager, TitleTranslationManager>(ActivationScope.Unique);

            kernel.Register<ITranslationSettingsProvider, TranslationSettingsProvider>(ActivationScope.Unique);

            kernel.RegisterConfiguration<TranslationManagementConfiguration>();
        }

        public void AddMessageSubscriptions(IEventBroker broker)
        {
        }

        public void Configure(IConfigurator configurator)
        {
        }
    }
}
