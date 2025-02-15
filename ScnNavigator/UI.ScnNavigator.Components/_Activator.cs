using CrossCutting.Core.Contract.Bootstrapping;
using CrossCutting.Core.Contract.Configuration;
using CrossCutting.Core.Contract.DependencyInjection;
using CrossCutting.Core.Contract.DependencyInjection.DataClasses;
using CrossCutting.Core.Contract.EventBrokerage;
using UI.ScnNavigator.Components.Components;
using UI.ScnNavigator.Components.Contract;
using UI.ScnNavigator.Components.Contract.Texts;
using UI.ScnNavigator.Components.Graphs;
using UI.ScnNavigator.Components.InternalContract.Mergers;
using UI.ScnNavigator.Components.Mergers;
using UI.ScnNavigator.Components.Previews;
using UI.ScnNavigator.Components.Texts;

namespace UI.ScnNavigator.Components
{
    public class ScnNavigatorComponentsActivator : IComponentActivator
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
            kernel.Register<ISceneTextNormalizer, SceneTextNormalizer>(ActivationScope.Unique);
            kernel.Register<ITextCharacterReplacer, TextCharacterReplacer>(ActivationScope.Unique);

            kernel.Register<IComponentFactory, ComponentFactory>(ActivationScope.Unique);
            kernel.Register<IPreviewComponentFactory, PreviewComponentFactory>(ActivationScope.Unique);
            kernel.Register<IGraphViewComponentFactory, GraphViewComponentFactory>(ActivationScope.Unique);

            kernel.Register<IHelpPreviewDataMerger, HelpPreviewDataMerger>(ActivationScope.Unique);
            kernel.Register<ITutorialPreviewDataMerger, TutorialPreviewDataMerger>(ActivationScope.Unique);
            kernel.Register<IStaffrollPreviewDataMerger, StaffrollPreviewDataMerger>(ActivationScope.Unique);
            kernel.Register<IScenePreviewDataMerger, ScenePreviewDataMerger>(ActivationScope.Unique);
            kernel.Register<ITipPreviewDataMerger, TipPreviewDataMerger>(ActivationScope.Unique);

            kernel.RegisterToSelf<NavigatorForm>();
            kernel.RegisterToSelf<ChapterForm>();
            kernel.RegisterToSelf<TipsForm>();
            kernel.RegisterToSelf<HelpsForm>();
            kernel.RegisterToSelf<TutorialsForm>();
            kernel.RegisterToSelf<OutlinesForm>();
            kernel.RegisterToSelf<StaffrollForm>();

            kernel.RegisterToSelf<BadEndPreview>();
            kernel.RegisterToSelf<DecisionPreview>();
            kernel.RegisterToSelf<OutlinePreview>();
            kernel.RegisterToSelf<ScenePreview>();
            kernel.RegisterToSelf<StaffRollPreview>();
            kernel.RegisterToSelf<TipPreview>();
            kernel.RegisterToSelf<HelpPreview>();
            kernel.RegisterToSelf<TutorialPreview>();

            kernel.RegisterToSelf<MetricSubtitleScenePreview>();
            kernel.RegisterToSelf<MetricNarrationScenePreview>();
            kernel.RegisterToSelf<MetricTipPreview>();

            kernel.RegisterToSelf<SceneGraphView>();

            kernel.RegisterConfiguration<ScnNavigatorComponentsConfiguration>();
        }

        public void AddMessageSubscriptions(IEventBroker broker)
        {
        }

        public void Configure(IConfigurator configurator)
        {
        }
    }
}
