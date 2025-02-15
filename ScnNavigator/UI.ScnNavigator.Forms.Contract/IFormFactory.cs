using CrossCutting.Core.Contract.Aspects;
using ImGui.Forms;
using UI.ScnNavigator.Forms.Contract.Exceptions;

namespace UI.ScnNavigator.Forms.Contract
{
    [MapException(typeof(ScnNavigatorFormsException))]
    public interface IFormFactory
    {
        Form CreateMainForm();
    }
}
