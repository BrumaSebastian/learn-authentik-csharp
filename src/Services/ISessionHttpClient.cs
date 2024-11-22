namespace Server.Services
{
    public interface ISessionHttpClient
    {
        Task<HttpResponseMessage> SendAsync(string url);
    }
}