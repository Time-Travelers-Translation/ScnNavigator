using CrossCutting.Core.Contract.Settings;
using Logic.Domain.GoogleSheetsManagement.Contract.DataClasses;
using Logic.Domain.GoogleSheetsManagement.InternalContract;
using Logic.Domain.GoogleSheetsManagement.InternalContract.DataClasses;

namespace Logic.Domain.GoogleSheetsManagement
{
    internal class OAuth2TokenStorage : IOAuth2TokenStorage
    {
        private readonly ISettingsProvider _settingProvider;
        private readonly TokenData _tokenData;

        public OAuth2TokenStorage(ISettingsProvider settingsProvider)
        {
            _settingProvider = settingsProvider;
            _tokenData = new TokenData
            {
                Scope = settingsProvider.Get<Scope?>("Google.OAuth2.Scope", null),
                Expiration = settingsProvider.Get<DateTime>("Google.OAuth2.Expiration", default),
                AccessToken = settingsProvider.Get<string?>("Google.OAuth2.AccessToken", null),
                RefreshToken = settingsProvider.Get<string?>("Google.OAuth2.RefreshToken", null)
            };
        }

        public Scope? GetScope()
        {
            return _tokenData.Scope;
        }

        public string? GetAccessToken()
        {
            return _tokenData.AccessToken;
        }

        public string? GetRefreshToken()
        {
            return _tokenData.RefreshToken;
        }

        public DateTime GetExpiration()
        {
            return _tokenData.Expiration;
        }

        public void SetScope(Scope scope)
        {
            _tokenData.Scope = scope;
        }

        public void SetAccessToken(string accessToken)
        {
            _tokenData.AccessToken = accessToken;
        }

        public void SetRefreshToken(string refreshToken)
        {
            _tokenData.RefreshToken = refreshToken;
        }

        public void SetExpiration(DateTime expiration)
        {
            _tokenData.Expiration = expiration;
        }

        public void Persist()
        {
            _settingProvider.Set("Google.OAuth2.Scope", _tokenData.Scope);
            _settingProvider.Set("Google.OAuth2.Expiration", _tokenData.Expiration);
            _settingProvider.Set("Google.OAuth2.AccessToken", _tokenData.AccessToken);
            _settingProvider.Set("Google.OAuth2.RefreshToken", _tokenData.RefreshToken);
        }
    }
}
