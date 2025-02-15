using CrossCutting.Core.Contract.Configuration.DataClasses;

namespace UI.ScnNavigator.Resources
{
    public class ScnNavigatorResourcesConfiguration
    {
        [ConfigMap("UI.ScnNavigator.Resources", "LocalizationPath")]
        public virtual string LocalizationPath { get; set; } = "resources/langs";

        [ConfigMap("UI.ScnNavigator.Resources", "DefaultLocale")]
        public virtual string DefaultLocale { get; set; }
    }
}
