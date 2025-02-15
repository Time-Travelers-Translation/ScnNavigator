using CrossCutting.Core.Contract.Bootstrapping;
using CrossCutting.Core.Contract.Configuration;
using CrossCutting.Core.Contract.DependencyInjection;
using CrossCutting.Core.Contract.EventBrokerage;
using CrossCutting.Core.Contract.Messages;
using Logic.Domain.GraphVizManagement.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CrossCutting.Core.Contract.DependencyInjection.DataClasses;
using Logic.Domain.GraphVizManagement.InternalContract;

namespace Logic.Domain.GraphVizManagement
{
    public class GraphVizManagementActivator : IComponentActivator
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
            kernel.Register<IGraphSyntaxCreator, GraphSyntaxCreator>(ActivationScope.Unique);
            kernel.Register<IGraphLayoutCreator, GraphLayoutCreator>(ActivationScope.Unique);

            kernel.Register<IGraphVizExecutor, GraphVizExecutor>(ActivationScope.Unique);
            kernel.Register<IGraphVizProvider, GraphVizProvider>(ActivationScope.Unique);

            kernel.RegisterConfiguration<GraphVizManagementConfiguration>();
        }

        public void AddMessageSubscriptions(IEventBroker broker)
        {
        }

        public void Configure(IConfigurator configurator)
        {
        }
    }
}
