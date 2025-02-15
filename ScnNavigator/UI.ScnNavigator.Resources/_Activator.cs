using CrossCutting.Core.Contract.Bootstrapping;
using CrossCutting.Core.Contract.Configuration;
using CrossCutting.Core.Contract.DependencyInjection;
using CrossCutting.Core.Contract.DependencyInjection.DataClasses;
using CrossCutting.Core.Contract.EventBrokerage;
using ImGui.Forms.Localization;
using UI.ScnNavigator.Resources.Contract;

namespace UI.ScnNavigator.Resources
{
    public class ScnNavigatorResourcesActivator : IComponentActivator
    {
        public void Activating()
        {
        }

        public void Activated()
        {
        }

        public void Deactivating()
        {
        }

        public void Deactivated()
        {
        }

        public void Register(ICoCoKernel kernel)
        {
            kernel.Register<ILocalizer, Localizer>(ActivationScope.Unique);

            kernel.Register<IStringResourceProvider, StringResourceProvider>(ActivationScope.Unique);
            kernel.Register<IImageResourceProvider, ImageResourceProvider>(ActivationScope.Unique);
            kernel.Register<IScreenResourceProvider, ScreenResourceProvider>(ActivationScope.Unique);

            kernel.Register<IFontProvider, FontProvider>(ActivationScope.Unique);

            kernel.RegisterConfiguration<ScnNavigatorResourcesConfiguration>();
        }

        public void AddMessageSubscriptions(IEventBroker broker)
        {
        }

        public void Configure(IConfigurator configurator)
        {
        }
    }
}
