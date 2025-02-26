namespace src.Utils.AuthModels
{
    public class HttpParamsBuilder
    {
        private readonly Dictionary<string, string> _params = [];

        public HttpParamsBuilder AddClientId(string clientId)
        {
            AddStringParam("client_id", clientId);

            return this;
        }

        public HttpParamsBuilder AddClientSecret(string clientSecret)
        {
            AddStringParam("client_secret", clientSecret);

            return this;
        }

        public HttpParamsBuilder AddRedirectUri(string redirectUri)
        {
            AddStringParam("redirect_uri", redirectUri);

            return this;
        }

        public HttpParamsBuilder AddGrantType(GrantType grantType)
        {
            _params["grant_type"] = grantType.ToString().ToLower();
            return this;
        }

        public HttpParamsBuilder AddScopes(Scopes[] scopes)
        {
            _params["grant_type"] = string.Join(" ", scopes.Select(s => s.ToString().ToLower()));
            return this;
        }

        public HttpParamsBuilder AddAuthorizationCode(string code)
        {
            AddStringParam("code", code);

            return this;
        }

        public HttpParamsBuilder AddJWTToken(string token)
        {
            AddStringParam("token", token);

            return this;
        }

        public FormUrlEncodedContent CreateFormContent() => new FormUrlEncodedContent(_params);

        private void AddStringParam(string name, string value)
        {
            if (!string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(value))
            {
                _params[name] = value;
            }
        }
    }
}