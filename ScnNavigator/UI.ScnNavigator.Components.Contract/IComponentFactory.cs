using CrossCutting.Core.Contract.Aspects;
using ImGui.Forms.Controls.Base;
using Logic.Business.TimeTravelersManagement.Contract.DataClasses;
using Logic.Domain.GraphVizManagement.Contract.DataClasses;
using Logic.Domain.Level5Management.Contract.DataClasses.Scene;
using UI.ScnNavigator.Components.Contract.DataClasses;
using UI.ScnNavigator.Components.Contract.Exceptions;

namespace UI.ScnNavigator.Components.Contract
{
    [MapException(typeof(ScnNavigatorComponentsException))]
    public interface IComponentFactory
    {
        Component CreateNavigatorForm(SceneNavigator navigator);
        Component CreateChapterForm(IList<Node<SceneEntry>> nodes);
        Component CreateTipsForm(TipTitlesData tipTitles);
        Component CreateHelpsForm(HelpTitlesData helpTitles);
        Component CreateTutorialsForm(TutorialTitlesData tutorialTitles);
        Component CreateOutlineForm(OutlineData outlines);
        Component CreateStaffrollForm(StaffrollData staffroll);
        Component CreateTtpCallForm(TtpCallData callData);
    }
}
