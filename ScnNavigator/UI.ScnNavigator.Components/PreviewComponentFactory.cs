using CrossCutting.Core.Contract.DependencyInjection;
using ImGui.Forms.Controls.Base;
using UI.ScnNavigator.Components.Contract;
using UI.ScnNavigator.Components.Previews;

namespace UI.ScnNavigator.Components
{
    internal class PreviewComponentFactory : IPreviewComponentFactory
    {
        private readonly ICoCoKernel _kernel;

        public PreviewComponentFactory(ICoCoKernel kernel)
        {
            _kernel = kernel;
        }

        public Component CreateHelpPreview()
        {
            return _kernel.Get<HelpPreview>();
        }

        public Component CreateTutorialPreview()
        {
            return _kernel.Get<TutorialPreview>();
        }

        public Component CreateOutlinePreview()
        {
            return _kernel.Get<OutlinePreview>();
        }

        public Component CreateScenePreview()
        {
            return _kernel.Get<ScenePreview>();
        }

        public Component CreateMetricSubtitleScenePreview()
        {
            return _kernel.Get<MetricSubtitleScenePreview>();
        }

        public Component CreateMetricNarrationScenePreview()
        {
            return _kernel.Get<MetricNarrationScenePreview>();
        }

        public Component CreateTipPreview()
        {
            return _kernel.Get<TipPreview>();
        }

        public Component CreateMetricTipPreview()
        {
            return _kernel.Get<MetricTipPreview>();
        }

        public Component CreateStaffRollPreview()
        {
            return _kernel.Get<StaffRollPreview>();
        }

        public Component CreateDecisionPreview()
        {
            return _kernel.Get<DecisionPreview>();
        }

        public Component CreateBadEndPreview()
        {
            return _kernel.Get<BadEndPreview>();
        }
    }
}
