using CrossCutting.Core.Contract.DependencyInjection;
using CrossCutting.Core.Contract.DependencyInjection.DataClasses;
using Logic.Domain.GoogleSheetsManagement.Contract;

namespace Logic.Domain.GoogleSheetsManagement
{
    internal class GoogleApiConnector : IGoogleApiConnector
    {
        private readonly ICoCoKernel _kernel;

        public GoogleApiConnector(ICoCoKernel kernel)
        {
            _kernel = kernel;
        }

        public ISheetManager CreateSheetManager(string sheetId)
        {
            return _kernel.Get<ISheetManager>(
                new ConstructorParameter("sheetId", sheetId));
        }
    }
}
