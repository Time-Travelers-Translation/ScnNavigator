using CrossCutting.Core.Contract.DependencyInjection;
using CrossCutting.Core.Contract.DependencyInjection.DataClasses;
using Logic.Domain.GoogleSheetsManagement.Contract;
using Logic.Domain.GoogleSheetsManagement.Contract.DataClasses;
using Logic.Domain.GoogleSheetsManagement.InternalContract;

namespace Logic.Domain.GoogleSheetsManagement
{
    internal class CodeFlowProvider : ICodeFlowProvider
    {
        private readonly ICoCoKernel _kernel;
        private readonly CodeFlowType _codeFlowType;

        public CodeFlowProvider(GoogleSheetsManagementConfiguration config, ICoCoKernel kernel)
        {
            _kernel = kernel;
            _codeFlowType = config.FlowType;
        }

        public ICodeFlowManager GetCodeFlow()
        {
            switch (_codeFlowType)
            {
                case CodeFlowType.ApiKey:
                    return _kernel.Get<IApiKeyCodeFlowManager>();

                case CodeFlowType.OAuth2:
                    return _kernel.Get<IOAuth2CodeFlowManager>();

                default:
                    throw new InvalidOperationException($"Unknown code flow type {_codeFlowType}.");
            }
        }
    }
}
