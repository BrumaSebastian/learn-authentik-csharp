
using Microsoft.Extensions.Options;
using Server.Models.Options;
using src.Utils.AuthModels;
using System.Text;

namespace src.Services.HttpClients
{
    public class AuthClient : IAuthentikClient
    {
        private readonly HttpClient _httpClient;
        private readonly AuthenticationOptions _authenticationOptions;

        public AuthClient(IHttpClientFactory httpClientFactory, IOptions<AuthenticationOptions> authOptions)
        {
            _httpClient = httpClientFactory.CreateClient("AuthClient");
            _authenticationOptions = authOptions.Value;
        }

        public async Task<string> RetrieveToken(string authorizationCode)
        {
            var paramBuilder = new HttpParamsBuilder()
                .AddGrantType(GrantType.Authorization_code)
                .AddAuthorizationCode(authorizationCode)
                .AddClientId(_authenticationOptions.ClientId)
                .AddClientSecret(_authenticationOptions.ClientSecret)
                .AddRedirectUri(_authenticationOptions.CallbackPath)
                .ToString();

            var response = await _httpClient.PostAsync(_authenticationOptions.JWTTokenEndpoint, new StringContent(paramBuilder, Encoding.UTF8, "application/x-www-form-urlencoded"));

            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> ValidateToken(string token)
        {
            var paramBuilder = new HttpParamsBuilder()
                .AddJWTToken(token)
                .AddClientId(_authenticationOptions.ClientId)
                .AddClientSecret(_authenticationOptions.ClientSecret)
                .AddScopes([Scopes.Openid, Scopes.Offline_access])
                .ToString();

            var response = await _httpClient.PostAsync(_authenticationOptions.IntrospectEndpoint, new StringContent(paramBuilder, Encoding.UTF8, "application/x-www-form-urlencoded"));

            return await response.Content.ReadAsStringAsync();
        }
    }
}