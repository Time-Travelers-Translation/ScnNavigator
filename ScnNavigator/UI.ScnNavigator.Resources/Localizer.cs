﻿using System.Text.Json;
using CrossCutting.Core.Contract.Settings;
using ImGui.Forms.Localization;

namespace UI.ScnNavigator.Resources
{
    internal class Localizer : BaseLocalizer
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            ReadCommentHandling = JsonCommentHandling.Skip
        };

        private const string NameValue_ = "Name";

        private readonly ScnNavigatorResourcesConfiguration _config;
        private readonly ISettingsProvider _settingsProvider;

        protected override string UndefinedValue => "<undefined>";
        protected override string DefaultLocale => "en";

        public Localizer(ScnNavigatorResourcesConfiguration config, ISettingsProvider settingsProvider)
        {
            _config = config;
            _settingsProvider = settingsProvider;

            Initialize();
        }

        protected override IList<LanguageInfo> InitializeLocalizations()
        {
            string? applicationDirectory = Path.GetDirectoryName(Environment.ProcessPath);
            if (string.IsNullOrEmpty(applicationDirectory))
                return Array.Empty<LanguageInfo>();

            string localeDirectory = Path.Combine(applicationDirectory, _config.LocalizationPath);
            if (!Directory.Exists(localeDirectory))
                return Array.Empty<LanguageInfo>();

            string[] localeFiles = Directory.GetFiles(localeDirectory);

            var result = new List<LanguageInfo>();
            foreach (string localeFile in localeFiles)
            {
                // Read text from stream
                string json = File.ReadAllText(localeFile);

                // Deserialize JSON
                var entries = JsonSerializer.Deserialize<IDictionary<string, string>>(json, JsonOptions);
                if (entries == null || !entries.TryGetValue(NameValue_, out string? name))
                    continue;

                string locale = Path.GetFileNameWithoutExtension(localeFile);
                result.Add(new LanguageInfo(locale, name, entries));
            }

            return result;
        }

        protected override string InitializeLocale()
        {
            return _settingsProvider.Get("ScnNavigator.Settings.Locale", string.Empty);
        }

        protected override void SetCurrentLocale(string locale)
        {
            base.SetCurrentLocale(locale);

            _settingsProvider.Set("ScnNavigator.Settings.Locale", locale);
        }
    }
}
