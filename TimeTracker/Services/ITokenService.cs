using System.Security.Claims;
using TimeTracker.Models;

namespace TimeTracker.Services
{
    public interface ITokenService
    {
        string GenerateAccessToken(IEnumerable<Claim> claims);
        Task<RefreshTokenProvider> AssignRefreshToken(User user);
        bool RegeneratedRefreshTokenAfterValidation(string token);
    }
}
