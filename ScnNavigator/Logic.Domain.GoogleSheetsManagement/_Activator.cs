using CrossCutting.Core.Contract.Bootstrapping;
using CrossCutting.Core.Contract.Configuration;
using CrossCutting.Core.Contract.DependencyInjection;
using CrossCutting.Core.Contract.EventBrokerage;
using CrossCutting.Core.Contract.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CrossCutting.Core.Contract.DependencyInjection.DataClasses;
using Logic.Domain.GoogleSheetsManagement.Contract;
using Logic.Domain.GoogleSheetsManagement.InternalContract;

namespace Logic.Domain.GoogleSheetsManagement
{
    public class GoogleSheetsManagementActivator : IComponentActivator
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
            kernel.Register<IGoogleApiConnector, GoogleApiConnector>(ActivationScope.Unique);

            kernel.Register<ICodeFlowProvider, CodeFlowProvider>(ActivationScope.Unique);
            kernel.Register<IApiKeyCodeFlowManager, ApiKeyCodeFlowManager>();
            kernel.Register<IOAuth2CodeFlowManager, OAuth2CodeFlowManager>();

            kernel.Register<IOAuth2TokenProvider, OAuth2TokenProvider>(ActivationScope.Unique);
            kernel.Register<IOAuth2Listener, OAuth2Listener>(ActivationScope.Unique);
            kernel.Register<IOAuth2TokenStorage, OAuth2TokenStorage>(ActivationScope.Unique);

            kernel.Register<ISheetManager, SheetManager>();
            kernel.Register<IDataRangeParser, DataRangeParser>(ActivationScope.Unique);

            kernel.Register<IRequestContentBuilder, RequestContentBuilder>(ActivationScope.Unique);

            kernel.Register<IPropertyAspectProvider, PropertyAspectProvider>(ActivationScope.Unique);

            kernel.RegisterConfiguration<GoogleSheetsManagementConfiguration>();
        }

        public void AddMessageSubscriptions(IEventBroker broker)
        {
        }

        public void Configure(IConfigurator configurator)
        {
        }
    }
}
