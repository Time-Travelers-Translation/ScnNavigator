using CrossCutting.Core.Contract.Bootstrapping;
using CrossCutting.Core.Contract.Configuration;
using CrossCutting.Core.Contract.DependencyInjection;
using CrossCutting.Core.Contract.DependencyInjection.DataClasses;
using CrossCutting.Core.Contract.EventBrokerage;
using UI.ScnNavigator.Dialogs.Contract;

namespace UI.ScnNavigator.Dialogs
{
    public class ScnNavigatorDialogsActivator : IComponentActivator
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
            kernel.Register<IDialogFactory, DialogFactory>(ActivationScope.Unique);

            kernel.RegisterToSelf<FindSceneDialog>();
            kernel.RegisterToSelf<FindTipDialog>();

            kernel.RegisterToSelf<SubtitleMetricDialog>();
            kernel.RegisterToSelf<NarrationMetricDialog>();
            kernel.RegisterToSelf<TipMetricDialog>();

            kernel.RegisterConfiguration<ScnNavigatorDialogsConfiguration>();
        }

        public void AddMessageSubscriptions(IEventBroker broker)
        {
        }

        public void Configure(IConfigurator configurator)
        {
        }
    }
}
