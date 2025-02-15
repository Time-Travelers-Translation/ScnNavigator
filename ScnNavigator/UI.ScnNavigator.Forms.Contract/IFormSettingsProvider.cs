using CrossCutting.Core.Contract.Aspects;
using ImGui.Forms.Models;
using UI.ScnNavigator.Forms.Contract.Exceptions;

namespace UI.ScnNavigator.Forms.Contract
{
    [MapException(typeof(ScnNavigatorFormsException))]
    public interface IFormSettingsProvider
    {
        Theme GetThemeSetting();
        void SetThemeSetting(Theme theme);
    }
}
