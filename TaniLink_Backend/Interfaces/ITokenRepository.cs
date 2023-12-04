using TaniLink_Backend.Models;

namespace TaniLink_Backend.Interfaces
{
    public interface ITokenRepository
    {
        Task<string> CreateAccessToken(User user);
    }
}
