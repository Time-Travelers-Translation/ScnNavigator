using CrossCutting.Core.Contract.DependencyInjection;
using CrossCutting.Core.Contract.DependencyInjection.DataClasses;
using ImGui.Forms.Controls.Base;
using Logic.Business.TimeTravelersManagement.Contract.DataClasses;
using Logic.Domain.GraphVizManagement.Contract.DataClasses;
using Logic.Domain.Level5Management.Contract.DataClasses.Scene;
using UI.ScnNavigator.Components.Components;
using UI.ScnNavigator.Components.Contract;
using UI.ScnNavigator.Components.Contract.DataClasses;

namespace UI.ScnNavigator.Components
{
    internal class ComponentFactory : IComponentFactory
    {
        private readonly ICoCoKernel _kernel;

        public ComponentFactory(ICoCoKernel kernel)
        {
            _kernel = kernel;
        }

        public Component CreateNavigatorForm(SceneNavigator navigator)
        {
            return _kernel.Get<NavigatorForm>(
                new ConstructorParameter("sceneNavigator", navigator));
        }

        public Component CreateTipsForm(TipTitlesData tipTitles)
        {
            return _kernel.Get<TipsForm>(
                new ConstructorParameter("data", tipTitles));
        }

        public Component CreateHelpsForm(HelpTitlesData helpTitles)
        {
            return _kernel.Get<HelpsForm>(
                new ConstructorParameter("data", helpTitles));
        }

        public Component CreateTutorialsForm(TutorialTitlesData tutorialTitles)
        {
            return _kernel.Get<TutorialsForm>(
                new ConstructorParameter("data", tutorialTitles));
        }

        public Component CreateChapterForm(IList<Node<SceneEntry>> nodes)
        {
            return _kernel.Get<ChapterForm>(
                new ConstructorParameter("nodes", nodes));
        }

        public Component CreateOutlineForm(OutlineData outlines)
        {
            return _kernel.Get<OutlinesForm>(
                new ConstructorParameter("data", outlines));
        }

        public Component CreateStaffrollForm(StaffrollData staffroll)
        {
            return _kernel.Get<StaffrollForm>(
                new ConstructorParameter("data", staffroll));
        }
    }
}
