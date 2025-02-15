using CrossCutting.Core.Contract.EventBrokerage;
using ImGui.Forms;
using ImGui.Forms.Controls;
using ImGui.Forms.Controls.Base;
using ImGui.Forms.Controls.Menu;
using ImGui.Forms.Localization;
using ImGui.Forms.Modals;
using ImGui.Forms.Models;
using Logic.Business.TimeTravelersManagement.Contract;
using Logic.Business.TimeTravelersManagement.Contract.DataClasses;
using Logic.Business.TimeTravelersManagement.Contract.Texts;
using Logic.Business.TranslationManagement.Contract;
using Logic.Domain.Level5Management.Contract.ConfigBinary;
using Logic.Domain.Level5Management.Contract.DataClasses.ConfigBinary;
using Logic.Domain.Level5Management.Contract.DataClasses.Scene;
using Logic.Domain.Level5Management.Contract.Enums.ConfigBinary;
using Logic.Domain.Level5Management.Contract.Scene;
using UI.ScnNavigator.Abstract.DataClasses;
using UI.ScnNavigator.Components.Contract;
using UI.ScnNavigator.Components.Contract.DataClasses;
using UI.ScnNavigator.Dialogs.Contract;
using UI.ScnNavigator.Forms.Contract;
using UI.ScnNavigator.Resources.Contract;

namespace UI.ScnNavigator.Forms
{
    public partial class MainForm : Form
    {
        private readonly IFormSettingsProvider _formSettingsProvider;

        private readonly IComponentFactory _componentFactory;

        private readonly ILocalizer _localizer;
        private readonly IStringResourceProvider _stringProvider;

        private readonly IScnReader _scnReader;
        private readonly ITtpCallReader _ttpCallReader;
        private readonly IConfigurationReader<RawConfigurationEntry> _configReader;
        private readonly IEventTextParser _textParser;
        private readonly ITranslationSettingsProvider _translationSettingsProvider;
        private readonly IEventBroker _eventBroker;

        private readonly IDictionary<string, TabPage> _pathToTabPageDict;
        private readonly IDictionary<TabPage, string> _tabPageToPathDict;

        private readonly IDictionary<object, TabPage> _componentPageLookup;
        private readonly IDictionary<TabPage, object> _pageComponentLookup;

        private bool _enabled = true;

        public MainForm(IEventBroker eventBroker, IComponentFactory componentFactory, IDialogFactory dialogFactory,
            IStringResourceProvider stringProvider, ILocalizer localizer, 
            IScnReader scnReader, ITtpCallReader ttpCallReader, IConfigurationReader<RawConfigurationEntry> configReader,
            IEventTextParser textParser, ITranslationSettingsProvider translationSettingsProvider, IFormSettingsProvider formSettingsProvider)
        {
            InitializeComponent(localizer, stringProvider, translationSettingsProvider, formSettingsProvider);

            _formSettingsProvider = formSettingsProvider;

            _componentFactory = componentFactory;

            _localizer = localizer;
            _stringProvider = stringProvider;

            _scnReader = scnReader;
            _ttpCallReader = ttpCallReader;
            _configReader = configReader;
            _textParser = textParser;
            _translationSettingsProvider = translationSettingsProvider;
            _eventBroker = eventBroker;

            _pathToTabPageDict = new Dictionary<string, TabPage>();
            _tabPageToPathDict = new Dictionary<TabPage, string>();

            _componentPageLookup = new Dictionary<object, TabPage>();
            _pageComponentLookup = new Dictionary<TabPage, object>();

            _settingsLanguageMenuItem.SelectedItemChanged +=
                (s, e) => ChangeLocale(_settingsLanguageMenuItem.SelectedItem);
            _settingsThemeMenuItem.SelectedItemChanged +=
                (s, e) => ChangeTheme(_settingsThemeMenuItem.SelectedItem);

            _subtitleMetricButton.Clicked +=
                async (s, e) => await dialogFactory.CreateSubtitleMetricDialog().ShowAsync();
            _narrationMetricButton.Clicked +=
                async (s, e) => await dialogFactory.CreateNarrationMetricDialog().ShowAsync();
            _tipMetricButton.Clicked +=
                async (s, e) => await dialogFactory.CreateTipMetricDialog().ShowAsync();

            DragDrop += (s, e) => OpenFile(e[0].File);
            Closing += MainForm_Closing;

            _translationEnabledButton.CheckChanged += (s, e) => ChangeTranslationEnablement();
            _mainTabControl.PageRemoving += async (s, e) => e.Cancel = !await CanCloseTabPage(e.Page);
            _mainTabControl.PageRemoved += (s, e) => CloseTabPage(e.Page);

            eventBroker.Subscribe<FileChangedMessage>(message => MarkChangedForm(message.Sender, true));
            eventBroker.Subscribe<FileSavedMessage>(message => MarkChangedForm(message.Sender, false));
            eventBroker.Subscribe<ApplicationDisabledMessage>(_ => ToggleForm(false));
            eventBroker.Subscribe<ApplicationEnabledMessage>(_ => ToggleForm(true));
        }

        private void ToggleForm(bool enabled)
        {
            _enabled = enabled;

            _settingsButton.Enabled = enabled;

            _translationEnabledButton.Enabled = enabled;
            _settingsLanguageMenuItem.Enabled = enabled;
            _settingsThemeMenuItem.Enabled = enabled;

            foreach (MenuBarCheckBox langBtn in _settingsLanguageMenuItem.CheckItems)
                langBtn.Enabled = enabled;

            foreach (MenuBarCheckBox langBtn in _settingsThemeMenuItem.CheckItems)
                langBtn.Enabled = enabled;

            _mainTabControl.Enabled = enabled;
        }

        private async Task MainForm_Closing(object sender, ClosingEventArgs e)
        {
            if (_pageComponentLookup.Keys.All(x => !x.HasChanges))
                return;

            RaiseApplicationDisabled();

            DialogResult result = await GetUnsavedChangesResult();

            if (result == DialogResult.Cancel)
            {
                e.Cancel = true;
                RaiseApplicationEnabled();

                return;
            }

            if (result == DialogResult.Yes)
            {
                foreach (TabPage key in _pageComponentLookup.Keys)
                {
                    if (!key.HasChanges)
                        continue;

                    if (_pageComponentLookup[key] is ISaveableComponent saveable)
                        saveable.Save();
                }
            }
        }

        private void ChangeTranslationEnablement()
        {
            _translationSettingsProvider.SetTranslationEnabled(_translationEnabledButton.Checked);

            RaiseTranslationEnablementChanged();
        }

        private async void OpenFile(string filePath)
        {
            if (!_enabled)
                return;

            if (_pathToTabPageDict.TryGetValue(filePath, out TabPage? page))
            {
                _mainTabControl.SelectedPage = page;
                return;
            }

            if (!File.Exists(filePath))
                return;

            Component? form = CreateFormatForm(filePath);
            if (form == null)
                return;

            page = new TabPage(form)
            {
                Title = Path.GetFileName(filePath)
            };

            _mainTabControl.AddPage(page);
            _mainTabControl.SelectedPage = page;

            _pathToTabPageDict[filePath] = page;
            _tabPageToPathDict[page] = filePath;

            _componentPageLookup[form] = page;
            _pageComponentLookup[page] = form;
        }

        private Component? CreateFormatForm(string filePath)
        {
            switch (Path.GetExtension(filePath))
            {
                case ".scn":
                    return LoadNavigator(filePath);

                case ".bin":
                    if (Path.GetFileName(filePath).StartsWith("OUTLINE_"))
                        return LoadOutline(filePath);

                    if (Path.GetFileName(filePath).StartsWith("Tip_List"))
                        return LoadTips(filePath);

                    if (Path.GetFileName(filePath).StartsWith("Help_List"))
                        return LoadHelps(filePath);

                    if (Path.GetFileName(filePath).StartsWith("Tuto_List"))
                        return LoadTutorials(filePath);

                    if (Path.GetFileName(filePath).StartsWith("staffroll"))
                        return LoadStaffRoll(filePath);

                    if (Path.GetFileName(filePath).StartsWith("sp_") || Path.GetFileName(filePath).StartsWith("cmn_"))
                        return LoadTtpCall(filePath);

                    break;
            }

            return null;
        }

        private Component? LoadNavigator(string filePath)
        {
            if (!TryLoadSceneNavigator(filePath, out SceneNavigator? sceneNavigator))
                return null;

            return _componentFactory.CreateNavigatorForm(sceneNavigator!);
        }

        private Component? LoadOutline(string filePath)
        {
            if (!TryLoadRawConfiguration(filePath, out EventTextConfiguration? outlineConfig))
                return null;

            var data = new OutlineData
            {
                Route = $"{Path.GetFileName(filePath)[8]}",
                Data = outlineConfig!
            };

            return _componentFactory.CreateOutlineForm(data);
        }

        private Component? LoadTips(string filePath)
        {
            if (!TryLoadRawConfiguration(filePath, out EventTextConfiguration? tipTitleConfig))
                return null;

            var data = new TipTitlesData
            {
                TipPath = Path.GetDirectoryName(Path.GetFullPath(filePath))!,
                TipTitles = tipTitleConfig!.Texts.Select((t, i) => new TipTitleData { Id = i + 1, Text = t.Text }).ToArray()
            };

            return _componentFactory.CreateTipsForm(data);
        }

        private Component? LoadHelps(string filePath)
        {
            if (!TryLoadRawConfiguration(filePath, out EventTextConfiguration? helpTitleConfig))
                return null;

            var data = new HelpTitlesData
            {
                HelpPath = Path.GetDirectoryName(Path.GetFullPath(filePath))!,
                HelpTitles = helpTitleConfig!.Texts.Select((t, i) => new HelpTitleData { Id = i, Text = t.Text }).ToArray()
            };

            return _componentFactory.CreateHelpsForm(data);
        }

        private Component? LoadTutorials(string filePath)
        {
            if (!TryLoadRawConfiguration(filePath, out EventTextConfiguration? tutorialTitleConfig))
                return null;

            var data = new TutorialTitlesData
            {
                TutorialPath = Path.GetDirectoryName(Path.GetFullPath(filePath))!,
                TutorialTitles = tutorialTitleConfig!.Texts.Select((t, i) => new TutorialTitleData { Id = i, Text = t.Text }).ToArray()
            };

            return _componentFactory.CreateTutorialsForm(data);
        }

        private Component? LoadStaffRoll(string filePath)
        {
            if (!TryLoadRawConfiguration(filePath, out EventTextConfiguration? staffRollConfig))
                return null;

            var data = new StaffrollData
            {
                Data = staffRollConfig!
            };

            return _componentFactory.CreateStaffrollForm(data);
        }

        private Component? LoadTtpCall(string filePath)
        {
            if (!TryLoadTtpCall(filePath, out TtpCallData? callData))
                return null;

            return _componentFactory.CreateTtpCallForm(callData!);
        }

        private bool TryLoadSceneNavigator(string filePath, out SceneNavigator? sceneNavigator)
        {
            sceneNavigator = null;

            try
            {
                using Stream fileStream = File.OpenRead(filePath);
                sceneNavigator = _scnReader.Read(fileStream);
            }
            catch
            {
                return false;
            }

            return true;
        }

        private bool TryLoadRawConfiguration(string filePath, out EventTextConfiguration? eventTextConfig)
        {
            eventTextConfig = null;

            try
            {
                using Stream fileStream = File.OpenRead(filePath);

                Configuration<RawConfigurationEntry> config = _configReader.Read(fileStream, StringEncoding.Sjis);
                eventTextConfig = _textParser.Parse(config);
            }
            catch
            {
                return false;
            }

            return true;
        }

        private bool TryLoadTtpCall(string filePath, out TtpCallData? ttpCallData)
        {
            ttpCallData = null;

            try
            {
                using Stream fileStream = File.OpenRead(filePath);
                ttpCallData = _ttpCallReader.Read(fileStream, StringEncoding.Sjis);
            }
            catch
            {
                return false;
            }

            return true;
        }

        private async Task<bool> CanCloseTabPage(TabPage page)
        {
            if (!page.HasChanges)
                return true;

            RaiseApplicationDisabled();

            DialogResult result = await GetUnsavedChangesResult();

            if (result == DialogResult.Cancel)
            {
                RaiseApplicationEnabled();

                return false;
            }

            if (result == DialogResult.Yes)
            {
                if (_pageComponentLookup.TryGetValue(page, out object? form) && form is ISaveableComponent saveable)
                    saveable.Save();
            }

            RaiseApplicationEnabled();

            return true;
        }

        private void CloseTabPage(TabPage page)
        {
            if (!_tabPageToPathDict.TryGetValue(page, out string? filePath))
                return;

            if (!_pageComponentLookup.TryGetValue(page, out object? form))
                return;

            _tabPageToPathDict.Remove(page);
            _pathToTabPageDict.Remove(filePath);

            _pageComponentLookup.Remove(page);
            _componentPageLookup.Remove(form);
        }

        private void MarkChangedForm(object sender, bool changed)
        {
            if (_componentPageLookup.TryGetValue(sender, out TabPage? page))
                page.HasChanges = changed;
        }

        private async Task<DialogResult> GetUnsavedChangesResult()
        {
            LocalizedString caption = _stringProvider.FileCloseUnsavedChangesCaption();
            LocalizedString text = _stringProvider.FileCloseUnsavedChangesText();
            return await MessageBox.ShowYesNoCancelAsync(caption, text);
        }

        private void ChangeLocale(MenuBarCheckBox checkbox)
        {
            if (!_localeItems.TryGetValue(checkbox, out string? locale))
                return;

            _localizer.ChangeLocale(locale);
        }

        private void ChangeTheme(MenuBarCheckBox checkbox)
        {
            if (!_themeItems.TryGetValue(checkbox, out Theme theme))
                return;

            Style.ChangeTheme(theme);

            _formSettingsProvider.SetThemeSetting(theme);
        }

        private void RaiseTranslationEnablementChanged()
        {
            _eventBroker.Raise(new TranslationEnablementChangedMessage());
        }

        private void RaiseApplicationDisabled()
        {
            _eventBroker.Raise(new ApplicationDisabledMessage());
        }

        private void RaiseApplicationEnabled()
        {
            _eventBroker.Raise(new ApplicationEnabledMessage());
        }
    }
}
