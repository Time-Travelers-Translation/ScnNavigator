using CrossCutting.Core.Contract.Aspects;
using ImGui.Forms.Localization;
using ImGui.Forms.Models;
using UI.ScnNavigator.Resources.Contract.Exceptions;

namespace UI.ScnNavigator.Resources.Contract
{
    [MapException(typeof(ScnNavigatorResourcesException))]
    public interface IStringResourceProvider
    {
        public LocalizedString FileCloseUnsavedChangesCaption();
        public LocalizedString FileCloseUnsavedChangesText();

        public LocalizedString MenuSettingsCaption();
        public LocalizedString MenuMetricsCaption();

        public LocalizedString MenuSettingsChangeTranslationEnablementCaption();
        public LocalizedString MenuSettingsChangeLanguageCaption();
        public LocalizedString MenuSettingsChangeThemeCaption();
        public LocalizedString MenuSettingsThemeCaption(Theme theme);

        public LocalizedString MenuSubtitleMetricsCaption();
        public LocalizedString MenuNarrationMetricsCaption();
        public LocalizedString MenuTipsMetricsCaption();

        public LocalizedString FindSceneDialogCaption();
        public LocalizedString FindTipDialogCaption();

        public LocalizedString MetricDialogLoadingCaption();
        public LocalizedString MetricDialogShowWarningsCaption();

        public LocalizedString SubtitleMetricDialogCaption();
        public LocalizedString NarrationMetricDialogCaption();
        public LocalizedString TipMetricDialogCaption();
    }
}
