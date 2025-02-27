namespace Server.Models.Options;

public class AuthenticationOptions
{
    public string ClientConfigName { get; set; }
    public string AuthorizationEndpoint { get; init; }
    public string JWTTokenEndpoint { get; init; }
    public string IntrospectEndpoint { get; init; }
    public string BaseUri { get; init; }
    public string AppName { get; init; }
    public string ClientId { get; init; }
    public string ClientSecret { get; init; }
    public string CallbackPath { get; init; }
}