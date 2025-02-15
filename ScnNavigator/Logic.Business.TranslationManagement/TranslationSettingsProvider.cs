using CrossCutting.Core.Contract.Settings;
using Logic.Business.TranslationManagement.Contract;

namespace Logic.Business.TranslationManagement
{
    internal class TranslationSettingsProvider : ITranslationSettingsProvider
    {
        private readonly ISettingsProvider _settingsProvider;

        public TranslationSettingsProvider(ISettingsProvider settingsProvider)
        {
            _settingsProvider = settingsProvider;
        }

        public bool IsTranslationEnabled()
        {
            return _settingsProvider.Get("ScnNavigator.Translations.Enabled", false);
        }

        public void SetTranslationEnabled(bool value)
        {
            _settingsProvider.Set("ScnNavigator.Translations.Enabled", value);
        }
    }
}
