using Client.Settings;
using IdentityModel.Client;

namespace Client.Services
{
    public class TokenService : ITokenService
    {
        private readonly IdentityServerSettings _identityServerSettings;
        private readonly ServiceUrlSettings _serviceUrlSettings;
        private readonly HttpClient _client;
        private readonly DiscoveryDocumentResponse _discoveryDocumentResponse;

        public TokenService(IdentityServerSettings identityServerSettings, HttpClient client)
        {
            _identityServerSettings = identityServerSettings;
            _client = client;
            _discoveryDocumentResponse = _client.GetDiscoveryDocumentAsync(_serviceUrlSettings.IdentityServerUrl).Result;

            if (_discoveryDocumentResponse.IsError)
                throw new Exception("Unable to get discovery document", _discoveryDocumentResponse.Exception);
        }

        public async Task<TokenResponse> GetToken(string scope)
        {
            var clientCredentialsTokenRequest = new ClientCredentialsTokenRequest()
            {
                Address = _discoveryDocumentResponse.TokenEndpoint,
                ClientId = _identityServerSettings.ClientName,
                ClientSecret = _identityServerSettings.ClientPassword,
                Scope = scope
            };
            var tokenResponse = await _client.RequestClientCredentialsTokenAsync(clientCredentialsTokenRequest);

            if (tokenResponse.IsError)
                throw new Exception("Unable to get token", tokenResponse.Exception);

            return tokenResponse;
        }
    }
}
