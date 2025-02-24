
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Server.Models.Options;
using src.Utils.AuthModels;
using System.Text.Json;

namespace Server.Middlewares
{
    public class AuthentikIntrospectionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly HttpClient _httpClient;
        private readonly AuthenticationOptions _authenticationOptions;

        public AuthentikIntrospectionMiddleware(RequestDelegate next, HttpClient httpClient,
            IOptions<AuthenticationOptions> authOptions)
        {
            _next = next;
            _httpClient = httpClient;
            _authenticationOptions = authOptions.Value;
        }

        public async Task Invoke(HttpContext context)
        {
            var endpoint = context.GetEndpoint();
            var requiresAuth = endpoint?.Metadata.GetMetadata<IAuthorizeData>() is not null;

            if (!requiresAuth)
            {
                await _next(context);
                return;
            }

            var token = context.Request.Headers.Authorization.FirstOrDefault()?.Split(" ").Last();

            if (string.IsNullOrEmpty(token))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Missing or invalid token.");
                return;
            }

            var formContent = new HttpParamsBuilder()
                .AddJWTToken(token)
                .AddClientId(_authenticationOptions.ClientId)
                .AddClientSecret(_authenticationOptions.ClientSecret)
                .AddScopes([Scopes.Openid, Scopes.Offline_access])
                .CreateFormContent();

            var response = await _httpClient.PostAsync($"{_authenticationOptions.BaseUri}{_authenticationOptions.IntrospectEndpoint}", formContent);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync("Invalid token.");
                return;
            }

            var isTokenActive = JsonSerializer.Deserialize<JsonElement>(responseBody).GetProperty("active").GetBoolean();

            if (!isTokenActive)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Expired token.");
                return;
            }

            await _next(context);
        }
    }
}
