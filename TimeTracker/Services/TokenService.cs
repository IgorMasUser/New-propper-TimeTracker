using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using TimeTracker.Data;
using TimeTracker.Models;

namespace TimeTracker.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration configuration;
        private readonly IUserRepo repository;

        public TokenService(IConfiguration configuration, IUserRepo repository)
        {
            this.configuration = configuration;
            this.repository = repository;
        }

        public string GenerateAccessToken(IEnumerable<Claim> claims, int tokenExpirationTimeInMinutes)
        {
            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configuration.GetSection("JWT:Key").Value));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            var setToken = new JwtSecurityToken(
                configuration["JWT:Issuer"],
                configuration["JWT:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(tokenExpirationTimeInMinutes),
                signingCredentials: creds);

            var jwt = new JwtSecurityTokenHandler().WriteToken(setToken);
            return jwt;
        }
        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        public async Task<RefreshTokenProvider> GenerateAndAssignRefreshToken(int userId)
        {
            var refreshToken = GenerateRefreshToken();
            var refreshTokenProvider = await repository.SaveRefreshToken(userId, refreshToken);

            return refreshTokenProvider;
        }
    }
}
