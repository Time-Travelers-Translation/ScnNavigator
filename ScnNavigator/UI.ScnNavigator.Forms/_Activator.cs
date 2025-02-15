using CrossCutting.Core.Contract.Bootstrapping;
using CrossCutting.Core.Contract.Configuration;
using CrossCutting.Core.Contract.DependencyInjection;
using CrossCutting.Core.Contract.DependencyInjection.DataClasses;
using CrossCutting.Core.Contract.EventBrokerage;
using Logic.Business.TimeTravelersManagement.Contract;
using UI.ScnNavigator.Forms.Contract;
using UI.ScnNavigator.Resources.Contract;

namespace UI.ScnNavigator.Forms
{
    public class ScnNavigatorFormsActivator : IComponentActivator
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
            kernel.Register<IFormFactory, FormFactory>(ActivationScope.Unique);
            kernel.Register<IFormSettingsProvider, FormSettingsProvider>(ActivationScope.Unique);

            kernel.RegisterToSelf<MainForm>();

            kernel.RegisterConfiguration<ScnNavigatorFormsConfiguration>();
        }

        public void AddMessageSubscriptions(IEventBroker broker)
        {
        }

        public void Configure(IConfigurator configurator)
        {
        }
    }
}
