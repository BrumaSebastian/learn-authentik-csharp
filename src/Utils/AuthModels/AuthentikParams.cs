namespace src.Utils.AuthModels
{
    public enum GrantType
    {
        Authorization_code,
        Refresh_token,
        Implicit,
        Client_credentials,
        Password
    }

    public enum Scopes
    {
        Openid,
        Email,
        Profile,
        Entitlements,
        Offline_access
    }
}