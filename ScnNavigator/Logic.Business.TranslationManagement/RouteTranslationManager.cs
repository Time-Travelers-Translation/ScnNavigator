using CrossCutting.Abstract.DataClasses;
using Logic.Business.TranslationManagement.Contract;
using Logic.Business.TranslationManagement.InternalContract.DataClasses;
using Logic.Domain.GoogleSheetsManagement.Contract.DataClasses;
using Logic.Domain.GoogleSheetsManagement.Contract;

namespace Logic.Business.TranslationManagement
{
    internal class RouteTranslationManager : IRouteTranslationManager
    {
        private const string TableName_ = "Terminology";

        private readonly ISheetManager _sheetManager;

        private readonly IDictionary<char, string> _routeNameLookup;

        public RouteTranslationManager(TranslationManagementConfiguration config, IGoogleApiConnector apiConnector)
        {
            _sheetManager = apiConnector.CreateSheetManager(config.SheetId);

            _routeNameLookup = new Dictionary<char, string>();
        }

        public async Task<TextData?> GetRouteName(string sceneName)
        {
            char route = sceneName[0];
            if (route is 'A' or 'I')
                return null;

            if (!_routeNameLookup.TryGetValue(route, out string? routeName))
            {
                CellIdentifier startCell = CellIdentifier.Parse("B50");
                CellIdentifier endCell = CellIdentifier.Parse("B55");

                RouteTextRangeData[]? range = await _sheetManager.GetRangeAsync<RouteTextRangeData>(TableName_, startCell, endCell);
                if (range == null)
                    return null;

                _routeNameLookup['S'] = range[0].Text;
                _routeNameLookup['H'] = range[1].Text;
                _routeNameLookup['C'] = range[2].Text;
                _routeNameLookup['R'] = range[3].Text;
                _routeNameLookup['P'] = range[4].Text;
                _routeNameLookup['M'] = range[5].Text;

                if (!_routeNameLookup.TryGetValue(route, out routeName))
                    return null;
            }

            return new TextData
            {
                Name = sceneName,
                Text = routeName
            };
        }
    }
}
