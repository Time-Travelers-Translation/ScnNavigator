using CrossCutting.Core.Contract.Settings;
using ImGui.Forms;
using ImGui.Forms.Models;
using UI.ScnNavigator.Forms.Contract;

namespace UI.ScnNavigator.Forms
{
    internal class FormSettingsProvider : IFormSettingsProvider
    {
        private readonly ISettingsProvider _settingsProvider;

        public FormSettingsProvider(ISettingsProvider settingsProvider)
        {
            _settingsProvider = settingsProvider;
        }

        public Theme GetThemeSetting()
        {
            return _settingsProvider.Get("ScnNavigator.Settings.Theme", Style.Theme);
        }

        public void SetThemeSetting(Theme theme)
        {
            _settingsProvider.Set("ScnNavigator.Settings.Theme", theme);
        }
    }
}
