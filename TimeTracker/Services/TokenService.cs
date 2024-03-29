﻿using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using TimeTracker.Data;
using TimeTracker.Models;
using TimeTracker.Options;
using Microsoft.Extensions.Options;

namespace TimeTracker.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration configuration;
        private readonly IUserRepo repository;
        private readonly JWTOptions jwt;

        public TokenService(IConfiguration configuration, IUserRepo repository, IOptions<JWTOptions> jwt)
        {
            this.configuration = configuration;
            this.repository = repository;
            this.jwt = jwt.Value;
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

        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key)),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidIssuer = jwt.Issuer,
                ValidAudience = jwt.Audience,
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256Signature, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            return principal;

        }
    }
}
