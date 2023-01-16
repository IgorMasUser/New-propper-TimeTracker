using System.Security.Claims;
using TimeTracker.Models;

namespace TimeTracker.Services
{
    public interface ITokenService
    {
        string GenerateAccessToken(IEnumerable<Claim> claims);
        Task<string> AssignRefreshToken(User user);
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }
}
