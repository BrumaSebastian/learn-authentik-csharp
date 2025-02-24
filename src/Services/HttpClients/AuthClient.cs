
using Microsoft.Extensions.Options;
using Server.Models.Options;
using Server.Models.Responses;
using src.Utils.AuthModels;
using System.Text.Json;

namespace src.Services.HttpClients
{
    public class AuthClient : IAuthentikClient
    {
        private readonly HttpClient _httpClient;
        private readonly AuthenticationOptions _authenticationOptions;

        public AuthClient(IHttpClientFactory httpClientFactory, IOptions<AuthenticationOptions> authOptions)
        {
            _authenticationOptions = authOptions.Value;
            _httpClient = httpClientFactory.CreateClient(_authenticationOptions.ClientConfigName);
        }

        public async Task<Result<AuthorizationInfo>> RetrieveToken(string authorizationCode)
        {
            var requestParams = new HttpParamsBuilder()
                .AddGrantType(GrantType.Authorization_code)
                .AddAuthorizationCode(authorizationCode)
                .AddClientId(_authenticationOptions.ClientId)
                .AddClientSecret(_authenticationOptions.ClientSecret)
                .AddRedirectUri(_authenticationOptions.CallbackPath)
                .CreateFormContent();

            var response = await _httpClient.PostAsync(_authenticationOptions.JWTTokenEndpoint, requestParams);

            string content = await response.Content.ReadAsStringAsync();

            return response.IsSuccessStatusCode
                ? Result<AuthorizationInfo>.Success(JsonSerializer.Deserialize<AuthorizationInfo>(content, GetSerializerOptions()))
                : Result<AuthorizationInfo>.Failure(JsonSerializer.Deserialize<ErrorMessage>(content, GetSerializerOptions()));
        }

        public async Task<string> ValidateToken(string token)
        {
            var requestParams = new HttpParamsBuilder()
                .AddJWTToken(token)
                .AddClientId(_authenticationOptions.ClientId)
                .AddClientSecret(_authenticationOptions.ClientSecret)
                .AddScopes([Scopes.Openid, Scopes.Offline_access])
                .CreateFormContent();

            var response = await _httpClient.PostAsync(_authenticationOptions.IntrospectEndpoint, requestParams);

            return await response.Content.ReadAsStringAsync();
        }

        private static JsonSerializerOptions GetSerializerOptions() => new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
        };
    }
}