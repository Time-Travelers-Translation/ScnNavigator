using CrossCutting.Core.Contract.DependencyInjection;
using ImGui.Forms;
using UI.ScnNavigator.Forms.Contract;

namespace UI.ScnNavigator.Forms
{
    internal class FormFactory : IFormFactory
    {
        private readonly ICoCoKernel _kernel;

        public FormFactory(ICoCoKernel kernel)
        {
            _kernel = kernel;
        }

        public Form CreateMainForm()
        {
            return _kernel.Get<MainForm>();
        }
    }
}
