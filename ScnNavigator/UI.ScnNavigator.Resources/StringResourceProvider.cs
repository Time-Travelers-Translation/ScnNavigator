using ImGui.Forms.Localization;
using ImGui.Forms.Models;
using UI.ScnNavigator.Resources.Contract;

namespace UI.ScnNavigator.Resources
{
    internal class StringResourceProvider : IStringResourceProvider
    {
        public LocalizedString FileCloseUnsavedChangesCaption()
            => LocalizedString.FromId("File.Close.UnsavedChanges.Caption");
        public LocalizedString FileCloseUnsavedChangesText()
            => LocalizedString.FromId("File.Close.UnsavedChanges.Text");

        public LocalizedString MenuSettingsCaption()
            => LocalizedString.FromId("Menu.Settings.Caption");
        public LocalizedString MenuMetricsCaption()
            => LocalizedString.FromId("Menu.Metrics.Caption");

        public LocalizedString MenuSettingsChangeTranslationEnablementCaption()
            => LocalizedString.FromId("Menu.Settings.ChangeTranslationEnablement");
        public LocalizedString MenuSettingsChangeLanguageCaption()
            => LocalizedString.FromId("Menu.Settings.Languages.Caption");
        public LocalizedString MenuSettingsChangeThemeCaption()
            => LocalizedString.FromId("Menu.Settings.Themes.Caption");
        public LocalizedString MenuSettingsThemeCaption(Theme theme)
            => LocalizedString.FromId($"Menu.Settings.Theme.{theme}.Caption");

        public LocalizedString MenuSubtitleMetricsCaption()
            => LocalizedString.FromId("Menu.Metrics.Subtitles.Caption");
        public LocalizedString MenuNarrationMetricsCaption()
            => LocalizedString.FromId("Menu.Metrics.Narration.Caption");
        public LocalizedString MenuTipsMetricsCaption()
            => LocalizedString.FromId("Menu.Metrics.Tips.Caption");

        public LocalizedString FindSceneDialogCaption()
            => LocalizedString.FromId("Dialog.Find.Scene.Caption");
        public LocalizedString FindTipDialogCaption()
            => LocalizedString.FromId("Dialog.Find.Tip.Caption");

        public LocalizedString MetricDialogLoadingCaption()
            => LocalizedString.FromId("Dialog.Metric.Loading.Caption");

        public LocalizedString MetricDialogShowWarningsCaption()
            => LocalizedString.FromId("Dialog.Metric.ShowWarnings.Caption");

        public LocalizedString SubtitleMetricDialogCaption()
            => LocalizedString.FromId("Dialog.Metric.Subtitle.Caption");
        public LocalizedString NarrationMetricDialogCaption()
            => LocalizedString.FromId("Dialog.Metric.Narration.Caption");
        public LocalizedString TipMetricDialogCaption()
            => LocalizedString.FromId("Dialog.Metric.Tip.Caption");
    }
}
