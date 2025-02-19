
using Server.Models.Responses;

namespace src.Services.HttpClients
{
    public interface IAuthentikClient
    {
        Task<Result<AuthorizationInfo>> RetrieveToken(string authorizationCode);
        Task<string> ValidateToken(string token);
    }
}