using Logic.Domain.GoogleSheetsManagement.InternalContract;
using System.Net.Http.Headers;

namespace Logic.Domain.GoogleSheetsManagement
{
    internal class ApiKeyCodeFlowManager : IApiKeyCodeFlowManager
    {
        private readonly string _apiKey;

        public ApiKeyCodeFlowManager(GoogleSheetsManagementConfiguration config)
        {
            _apiKey = config.ApiKey;
        }

        public Task<HttpRequestMessage> CreateGetRequestAsync(string relativeUri)
        {
            relativeUri += $"?key={_apiKey}";

            var request = new HttpRequestMessage(HttpMethod.Get, relativeUri);

            return Task.FromResult(request);
        }

        public Task<HttpRequestMessage> CreatePostRequestAsync(string relativeUri)
        {
            relativeUri += $"?key={_apiKey}";

            var request = new HttpRequestMessage(HttpMethod.Post, relativeUri);

            return Task.FromResult(request);
        }
    }
}
