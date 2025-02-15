using CrossCutting.Core.Contract.Aspects;
using ImGui.Forms.Resources;
using UI.ScnNavigator.Resources.Contract.Enums;
using UI.ScnNavigator.Resources.Contract.Exceptions;

namespace UI.ScnNavigator.Resources.Contract
{
    [MapException(typeof(ScnNavigatorResourcesException))]
    public interface IFontProvider
    {
        void RegisterFont(Font font);
        FontResource GetFont(Font font);
    }
}
