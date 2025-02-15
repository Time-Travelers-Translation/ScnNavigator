using Logic.Domain.GoogleSheetsManagement.InternalContract;
using System.Net.Http.Json;
using System.Text;
using Logic.Domain.GoogleSheetsManagement.InternalContract.DataClasses;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Net;
using CrossCutting.Core.Contract.Logging;
using Logic.Domain.GoogleSheetsManagement.Contract.DataClasses;

namespace Logic.Domain.GoogleSheetsManagement
{
    internal class OAuth2TokenProvider : IOAuth2TokenProvider
    {
        private const string BaseAuthUrl_ = "https://accounts.google.com/o/";
        private const string AuthResource_ = "oauth2/v2/auth";

        private const string BaseTokenUrl_ = "https://www.googleapis.com/";
        private const string TokenResource_ = "oauth2/v4/token";

        private const string RedirectUri_ = "http://localhost:6001/";

        private readonly Random _rand = new();
        private readonly Dictionary<Scope, string> _scopeUrls = new()
        {
            [Scope.Read] = "https://www.googleapis.com/auth/spreadsheets.readonly",
            [Scope.Write] = "https://www.googleapis.com/auth/spreadsheets"
        };

        private readonly Scope _scope;
        private readonly string _clientId;
        private readonly string _clientSecret;

        private readonly IOAuth2Listener _oauthListener;
        private readonly IOAuth2TokenStorage _tokenStorage;
        private readonly ILogger _logger;

        public OAuth2TokenProvider(GoogleSheetsManagementConfiguration config, IOAuth2Listener oauthListener, IOAuth2TokenStorage tokenStorage, ILogger logger)
        {
            _scope = config.Scope;
            _clientId = config.ClientId;
            _clientSecret = config.ClientSecret;

            _oauthListener = oauthListener;
            _tokenStorage = tokenStorage;
            _logger = logger;
        }

        public async Task<string?> GetAccessToken()
        {
            // If access token is compatible with the requested scope
            if (IsScopeSupported(_tokenStorage.GetScope()))
            {
                // If access token is not expired, return it
                if (_tokenStorage.GetExpiration() > DateTime.UtcNow.ToLocalTime())
                    return _tokenStorage.GetAccessToken();

                // If the refresh token is set, use it to refresh the access token
                if (!string.IsNullOrEmpty(_tokenStorage.GetRefreshToken()))
                    return await RefreshAccessToken();
            }

            // Otherwise create a new set of access and refresh token
            return await CreateAccessToken();
        }

        private async Task<string?> RefreshAccessToken()
        {
            OAuth2TokenResponseData? tokenResponse = await ExchangeRefreshToken();
            if (tokenResponse == null)
                return null;

            _tokenStorage.SetAccessToken(tokenResponse.AccessToken);
            _tokenStorage.SetExpiration(DateTime.UtcNow + TimeSpan.FromSeconds(tokenResponse.ExpiresIn));

            _tokenStorage.Persist();

            return tokenResponse.AccessToken;
        }

        private async Task<string?> CreateAccessToken()
        {
            UserConsentData consentData = await IsUserConsent();
            if (!consentData.IsConsent)
            {
                _logger.Error("User didn't give consent: {0}", consentData.Error!);
                return null;
            }

            OAuth2TokenResponseData? tokenResponse = await ExchangeAuthCode(consentData.Code!);
            if (tokenResponse == null)
                return null;

            _tokenStorage.SetScope(_scope);
            _tokenStorage.SetAccessToken(tokenResponse.AccessToken);
            _tokenStorage.SetRefreshToken(tokenResponse.RefreshToken);
            _tokenStorage.SetExpiration(DateTime.Now + TimeSpan.FromSeconds(tokenResponse.ExpiresIn));

            _tokenStorage.Persist();

            return tokenResponse.AccessToken;
        }

        private async Task<OAuth2TokenResponseData?> ExchangeRefreshToken()
        {
            var client = new HttpClient { BaseAddress = new Uri(BaseTokenUrl_) };

            string queryString = TokenResource_;
            queryString += $"?refresh_token={_tokenStorage.GetRefreshToken()}";
            queryString += $"&client_id={_clientId}";
            queryString += $"&client_secret={_clientSecret}";
            queryString += "&grant_type=refresh_token";

            HttpRequestMessage request = new(HttpMethod.Post, queryString);
            HttpResponseMessage response = await client.SendAsync(request);

            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<OAuth2TokenResponseData>();
        }

        private async Task<OAuth2TokenResponseData?> ExchangeAuthCode(string code)
        {
            var client = new HttpClient { BaseAddress = new Uri(BaseTokenUrl_) };

            string queryString = TokenResource_;
            queryString += $"?code={code}";
            queryString += $"&client_id={_clientId}";
            queryString += $"&client_secret={_clientSecret}";
            queryString += $"&redirect_uri={RedirectUri_}";
            queryString += "&grant_type=authorization_code";

            HttpRequestMessage request = new(HttpMethod.Post, queryString);
            HttpResponseMessage response = await client.SendAsync(request);

            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<OAuth2TokenResponseData>();
        }

        private async Task<UserConsentData> IsUserConsent()
        {
            string state = GetRandomString(10);
            NameValueCollection authQuery = await RetrieveAuthQuery(state);

            if (authQuery["state"] != state)
                return new UserConsentData { Error = "The state was corrupted." };

            return new UserConsentData
            {
                IsConsent = true,
                Code = authQuery["code"]
            };
        }

        private async Task<NameValueCollection> RetrieveAuthQuery(string state)
        {
            _oauthListener.Start(RedirectUri_);

            OpenUrl(GetAuthUri(state));

            HttpListenerRequest request = await _oauthListener.AwaitRequest();
            return request.QueryString;
        }

        private string GetAuthUri(string state)
        {
            string queryString = AuthResource_;
            queryString += $"?client_id={_clientId}";
            queryString += "&response_type=code";
            queryString += $"&scope={_scopeUrls[_scope]}";
            queryString += $"&redirect_uri={RedirectUri_}";
            queryString += $"&state={state}";
            queryString += $"&nonce={GetRandomString(10)}";
            queryString += "&access_type=offline";

            return BaseAuthUrl_ + queryString;
        }

        /// <summary>
        /// Open a specified url.
        /// </summary>
        /// <param name="url">The url to open.</param>
        /// <remarks>https://github.com/dotnet/corefx/issues/10361</remarks>
        private void OpenUrl(string url)
        {
            var psi = new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            };

            Process.Start(psi);
        }

        private string GetRandomString(int length)
        {
            var sb = new StringBuilder(length);

            for (var i = 0; i < length; i++)
            {
                var randChar = (char)_rand.Next(0x41, 0x5A);
                sb.Append(randChar);
            }

            return sb.ToString();
        }

        private bool IsScopeSupported(Scope? scope)
        {
            if (scope == null)
                return false;

            if (_scope == Scope.Read)
                return true;

            return scope == Scope.Write;
        }
    }
}
