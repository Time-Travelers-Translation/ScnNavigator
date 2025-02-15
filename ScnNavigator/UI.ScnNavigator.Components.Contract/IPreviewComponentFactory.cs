using CrossCutting.Core.Contract.Aspects;
using ImGui.Forms.Controls.Base;
using UI.ScnNavigator.Components.Contract.Exceptions;

namespace UI.ScnNavigator.Components.Contract
{
    [MapException(typeof(ScnNavigatorComponentsException))]
    public interface IPreviewComponentFactory
    {
        Component CreateScenePreview();
        Component CreateMetricSubtitleScenePreview();
        Component CreateMetricNarrationScenePreview();
        Component CreateDecisionPreview();
        Component CreateBadEndPreview();
        Component CreateTipPreview();
        Component CreateMetricTipPreview();
        Component CreateHelpPreview();
        Component CreateTutorialPreview();
        Component CreateOutlinePreview();
        Component CreateStaffRollPreview();
    }
}
