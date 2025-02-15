using System.Numerics;
using ImGui.Forms;
using ImGui.Forms.Controls;
using ImGui.Forms.Controls.Layouts;
using ImGui.Forms.Controls.Menu;
using ImGui.Forms.Localization;
using ImGui.Forms.Models;
using Logic.Business.TranslationManagement.Contract;
using UI.ScnNavigator.Forms.Contract;
using UI.ScnNavigator.Resources.Contract;

namespace UI.ScnNavigator.Forms
{
    public partial class MainForm
    {
        private MenuBarMenu _settingsButton;
        private MenuBarMenu _metricsButton;

        private MenuBarCheckBox _translationEnabledButton;
        private MenuBarRadio _settingsLanguageMenuItem;
        private MenuBarRadio _settingsThemeMenuItem;

        private MenuBarButton _subtitleMetricButton;
        private MenuBarButton _narrationMetricButton;
        private MenuBarButton _tipMetricButton;

        private MainMenuBar _menuBar;
        private TabControl _mainTabControl;

        private IDictionary<MenuBarCheckBox, string> _localeItems;
        private IDictionary<MenuBarCheckBox, Theme> _themeItems;

        private void InitializeComponent(ILocalizer localizer, IStringResourceProvider stringProvider,
            ITranslationSettingsProvider translationSettingsProvider, IFormSettingsProvider formSettingsProvider)
        {
            _translationEnabledButton = new MenuBarCheckBox(stringProvider.MenuSettingsChangeTranslationEnablementCaption())
            {
                Checked = translationSettingsProvider.IsTranslationEnabled()
            };
            _settingsLanguageMenuItem = new MenuBarRadio(stringProvider.MenuSettingsChangeLanguageCaption());
            _settingsThemeMenuItem = new MenuBarRadio(stringProvider.MenuSettingsChangeThemeCaption());

            _subtitleMetricButton = new MenuBarButton(stringProvider.MenuSubtitleMetricsCaption());
            _narrationMetricButton = new MenuBarButton(stringProvider.MenuNarrationMetricsCaption());
            _tipMetricButton = new MenuBarButton(stringProvider.MenuTipsMetricsCaption());

            _settingsButton = new MenuBarMenu(stringProvider.MenuSettingsCaption())
            {
                Items =
                {
                    _translationEnabledButton,
                    _settingsLanguageMenuItem,
                    _settingsThemeMenuItem
                }
            };
            _metricsButton = new MenuBarMenu(stringProvider.MenuMetricsCaption())
            {
                Items =
                {
                    _subtitleMetricButton,
                    _narrationMetricButton,
                    _tipMetricButton
                }
            };

            _menuBar = new MainMenuBar
            {
                Items =
                {
                    _settingsButton,
                    _metricsButton
                }
            };
            _mainTabControl = new TabControl();

            _localeItems = new Dictionary<MenuBarCheckBox, string>();
            _themeItems = new Dictionary<MenuBarCheckBox, Theme>();

            InitializeLanguages(localizer);
            InitializeThemes(formSettingsProvider, stringProvider);

            AllowDragDrop = true;
            Size = new Vector2(1500, 800);

            MenuBar = _menuBar;
            Content = new StackLayout
            {
                Alignment = Alignment.Vertical,
                ItemSpacing = 5,
                Items =
                {
                    _mainTabControl
                }
            };
        }

        private void InitializeLanguages(ILocalizer localizer)
        {
            foreach (string locale in localizer.GetLocales())
            {
                var localeItem = new MenuBarCheckBox(localizer.GetLanguageName(locale))
                {
                    Checked = localizer.CurrentLocale == locale
                };

                _settingsLanguageMenuItem.CheckItems.Add(localeItem);
                _localeItems[localeItem] = locale;
            }
        }

        private void InitializeThemes(IFormSettingsProvider formSettingsProvider, IStringResourceProvider stringProvider)
        {
            Theme themeSetting = formSettingsProvider.GetThemeSetting();
            Style.ChangeTheme(themeSetting);

            foreach (Theme theme in Enum.GetValues(typeof(Theme)))
            {
                var themeItem = new MenuBarCheckBox(stringProvider.MenuSettingsThemeCaption(theme))
                {
                    Checked = themeSetting == theme
                };

                _settingsThemeMenuItem.CheckItems.Add(themeItem);
                _themeItems[themeItem] = theme;
            }
        }
    }
}
