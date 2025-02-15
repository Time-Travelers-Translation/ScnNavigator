using System.Net.Http.Headers;
using Logic.Domain.GoogleSheetsManagement.InternalContract;

namespace Logic.Domain.GoogleSheetsManagement
{
    internal class OAuth2CodeFlowManager : IOAuth2CodeFlowManager
    {
        private readonly IOAuth2TokenProvider _tokenProvider;

        public OAuth2CodeFlowManager(IOAuth2TokenProvider tokenProvider)
        {
            _tokenProvider = tokenProvider;
        }

        public async Task<HttpRequestMessage> CreateGetRequestAsync(string relativeUri)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, relativeUri);

            string accessToken = await _tokenProvider.GetAccessToken() ?? string.Empty;
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            return request;
        }

        public async Task<HttpRequestMessage> CreatePostRequestAsync(string relativeUri)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, relativeUri);

            string accessToken = await _tokenProvider.GetAccessToken() ?? string.Empty;
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            return request;
        }
    }
}
