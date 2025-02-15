using CrossCutting.Core.Contract.DependencyInjection;
using CrossCutting.Core.Contract.DependencyInjection.DataClasses;
using ImGui.Forms.Modals;
using Logic.Domain.Level5Management.Contract.DataClasses.Scene;
using UI.ScnNavigator.Dialogs.Contract;
using UI.ScnNavigator.Dialogs.Contract.DataClasses;

namespace UI.ScnNavigator.Dialogs
{
    internal class DialogFactory : IDialogFactory
    {
        private readonly ICoCoKernel _kernel;

        public DialogFactory(ICoCoKernel kernel)
        {
            _kernel = kernel;
        }

        public Modal CreateFindSceneDialog(object target, IList<SceneEntry> scenes)
        {
            return _kernel.Get<FindSceneDialog>(
                new ConstructorParameter("target", target),
                new ConstructorParameter("scenes", scenes));
        }

        public Modal CreateFindTipDialog(object target, IList<TipTitleEntryData> titles)
        {
            return _kernel.Get<FindTipDialog>(
                new ConstructorParameter("target", target),
                new ConstructorParameter("titles", titles));
        }

        public Modal CreateSubtitleMetricDialog()
        {
            return _kernel.Get<SubtitleMetricDialog>();
        }

        public Modal CreateNarrationMetricDialog()
        {
            return _kernel.Get<NarrationMetricDialog>();
        }

        public Modal CreateTipMetricDialog()
        {
            return _kernel.Get<TipMetricDialog>();
        }
    }
}
