using ImGui.Forms.Modals;
using Logic.Domain.Level5Management.Contract.DataClasses.Scene;
using UI.ScnNavigator.Dialogs.Contract.DataClasses;

namespace UI.ScnNavigator.Dialogs.Contract
{
    public interface IDialogFactory
    {
        Modal CreateFindSceneDialog(object target, IList<SceneEntry> scenes);
        Modal CreateFindTipDialog(object target, IList<TipTitleEntryData> titles);
        Modal CreateSubtitleMetricDialog();
        Modal CreateNarrationMetricDialog();
        Modal CreateTipMetricDialog();
    }
}
