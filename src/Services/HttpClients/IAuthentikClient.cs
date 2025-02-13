
namespace src.Services.HttpClients
{
    public interface IAuthentikClient
    {
        Task<string> RetrieveToken(string authorizationCode);
        Task<string> ValidateToken(string token);
    }
}