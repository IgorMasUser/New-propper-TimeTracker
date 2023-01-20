using System.Security.Claims;
using TimeTracker.Models;

namespace TimeTracker.Services
{
    public interface ITokenService
    {
        string GenerateAccessToken(IEnumerable<Claim> claims, int tokenExpirationTimeInMinutes);
        Task<RefreshTokenProvider> AssignRefreshToken(int user);
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }
}
