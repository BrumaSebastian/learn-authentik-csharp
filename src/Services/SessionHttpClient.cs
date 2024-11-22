using System.Net;

namespace Server.Services
{
    public class SessionHttpClient
    {
        public HttpClient Client { get; }
        public CookieContainer Cookies { get; }

        public SessionHttpClient()
        {
            Cookies = new CookieContainer();
            var handler = new HttpClientHandler
            {
                CookieContainer = Cookies,
                UseCookies = true
            };
            Client = new HttpClient(handler)
            {
                BaseAddress = new Uri("http://localhost:9000")
            };
        }
    }
}