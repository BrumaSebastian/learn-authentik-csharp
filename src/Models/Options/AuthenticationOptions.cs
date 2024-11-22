namespace Server.Models.Options;

public class AuthenticationOptions
{
    // public string Authority { get; init; }
    public string Authorization { get; init; }
    public string BaseUri { get; init; }
    public string ClientId { get; init; }
    public string ClientSecret { get; init; }
    public string CallbackPath { get; init; }
}