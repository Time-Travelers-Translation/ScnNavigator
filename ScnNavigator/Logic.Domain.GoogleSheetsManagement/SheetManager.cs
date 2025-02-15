using Logic.Domain.GoogleSheetsManagement.Contract;
using System.Net.Http.Json;
using System.Text.Json;
using Logic.Domain.GoogleSheetsManagement.Contract.DataClasses;
using Logic.Domain.GoogleSheetsManagement.InternalContract;
using Logic.Domain.GoogleSheetsManagement.InternalContract.DataClasses;

namespace Logic.Domain.GoogleSheetsManagement
{
    internal class SheetManager : ISheetManager
    {
        private const string BaseUrl_ = "https://sheets.googleapis.com";
        private const string SheetResource_ = "v4/spreadsheets";

        private readonly string _sheetId;
        private readonly ICodeFlowProvider _codeFlowProvider;
        private readonly IDataRangeParser _dataRangeParser;
        private readonly IRequestContentBuilder _contentBuilder;

        private readonly HttpClient _client;
        private readonly IDictionary<string, int> _sheetIdCache;

        public SheetManager(string sheetId, ICodeFlowProvider codeFlowProvider, IDataRangeParser dataRangeParser, IRequestContentBuilder contentBuilder)
        {
            _sheetId = sheetId;
            _codeFlowProvider = codeFlowProvider;
            _dataRangeParser = dataRangeParser;
            _contentBuilder = contentBuilder;

            _sheetIdCache = new Dictionary<string, int>();

            _client = new HttpClient
            {
                BaseAddress = new Uri(BaseUrl_)
            };
        }

        public async Task<SheetData[]?> GetSheetsAsync()
        {
            SheetObjectResponseData? sheetObject = await GetSheetObjectAsync();

            return sheetObject?.Sheets.Select(s => new SheetData
            {
                Id = s.Properties.SheetId,
                Name = s.Properties.Title
            }).ToArray();
        }

        public async Task<int?> GetSheetIdAsync(string sheetTitle)
        {
            if (_sheetIdCache.TryGetValue(sheetTitle, out int sheetId))
                return sheetId;

            SheetObjectResponseData? sheetObject = await GetSheetObjectAsync();

            int? resSheetId = sheetObject?.Sheets.FirstOrDefault(s => s.Properties.Title == sheetTitle)?.Properties.SheetId;
            if (resSheetId.HasValue)
                _sheetIdCache[sheetTitle] = resSheetId.Value;

            return resSheetId;
        }

        public async Task<TParse[]?> GetRangeAsync<TParse>(string sheetTitle, CellIdentifier start, CellIdentifier end)
        {
            ICodeFlowManager codeFlow = _codeFlowProvider.GetCodeFlow();

            HttpRequestMessage sheetRequest = await codeFlow.CreateGetRequestAsync($"{SheetResource_}/{_sheetId}/values/{sheetTitle}!{start}:{end}");
            HttpResponseMessage sheetResponse = await _client.SendAsync(sheetRequest);

            if (!sheetResponse.IsSuccessStatusCode)
                return null;

            var valueRange = await sheetResponse.Content.ReadFromJsonAsync<ValueRangeResponseData>();
            TParse[] parsedRange = _dataRangeParser.Parse<TParse>(valueRange?.Values, start, end).ToArray();

            return parsedRange;
        }

        public async Task<bool> UpdateRangeAsync<TUpdate>(IList<TUpdate> updateData, string sheetTitle, CellIdentifier start, CellIdentifier end)
        {
            ICodeFlowManager codeFlow = _codeFlowProvider.GetCodeFlow();

            int? sheetId = await GetSheetIdAsync(sheetTitle);
            if (!sheetId.HasValue)
                return false;

            UpdateCellsRequestData updateCellsRequest = _contentBuilder.CreateUpdateCellsRequest(updateData, sheetId.Value, start, end);
            var postRequest = new PostRequestData
            {
                Requests = new object[] { new { updateCells = updateCellsRequest } }
            };

            HttpRequestMessage sheetRequest = await codeFlow.CreatePostRequestAsync($"{SheetResource_}/{_sheetId}:batchUpdate");
            sheetRequest.Content = JsonContent.Create(postRequest, options: new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, WriteIndented = true });

            HttpResponseMessage sheetResponse = await _client.SendAsync(sheetRequest);

            return sheetResponse.IsSuccessStatusCode;
        }

        private async Task<SheetObjectResponseData?> GetSheetObjectAsync()
        {
            ICodeFlowManager codeFlow = _codeFlowProvider.GetCodeFlow();

            HttpRequestMessage sheetRequest = await codeFlow.CreateGetRequestAsync($"{SheetResource_}/{_sheetId}");
            HttpResponseMessage sheetResponse = await _client.SendAsync(sheetRequest);

            if (!sheetResponse.IsSuccessStatusCode)
                return null;

            return await sheetResponse.Content.ReadFromJsonAsync<SheetObjectResponseData>();
        }
    }
}
