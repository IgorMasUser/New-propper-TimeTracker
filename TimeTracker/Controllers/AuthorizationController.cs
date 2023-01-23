using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using NuGet.Common;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;
using TimeTracker.Data;
using TimeTracker.DTOs;
using TimeTracker.Models;
using TimeTracker.Services;

namespace TimeTrackerControllers
{
    public class AuthorizationController : Controller
    {
        private readonly ILogger<AuthorizationController> logger;
        private readonly IConfiguration configuration;
        private readonly IMapper mapper;
        private readonly IUserRepo repository;
        private readonly ITokenService tokenService;
        private readonly IHttpContextAccessor httpContextAccessor;
        const int tokenExpirationTimeInMinutes = 1;

        public AuthorizationController(ILogger<AuthorizationController> logger, IConfiguration configuration, IMapper mapper,
            IUserRepo repository, ITokenService tokenService, IHttpContextAccessor httpContextAccessor)
        {
            this.logger = logger;
            this.configuration = configuration;
            this.mapper = mapper;
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
            this.tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
            this.httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        public IActionResult Authorize()
        {
            return View();
        }

        [HttpPost]
        [Route("/Authorization/Authorize")]
        public async Task<ActionResult> Authorize(UserCreateDTO requestedUser)
        {
            if (requestedUser is null)
            {
                return BadRequest("Invalid client request");
            }
            var mappedUser = mapper.Map<User>(requestedUser);
            if (repository.CheckIfUserExists(mappedUser, requestedUser))
            {
                var obtainedUser = repository.GetUserClaims(mappedUser);
                List<Claim> claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, obtainedUser.Name),
                        new Claim(ClaimTypes.Email, obtainedUser.Email)
                    };
                string jwtAccessToken = tokenService.GenerateAccessToken(claims, tokenExpirationTimeInMinutes);
                SetAccessToken(jwtAccessToken, tokenExpirationTimeInMinutes);
                SetAccessTokenForDataRetriving(jwtAccessToken, tokenExpirationTimeInMinutes);

                //return Ok(jwtAccessToken); //If we want to expose token to frontend so it cares about token
                var jwtRefreshToken = await tokenService.AssignRefreshToken(obtainedUser.UserId);
                SetRefreshToken(jwtRefreshToken);

                return Ok();

                void SetAccessToken(string jwtAccessToken, int tokenExpirationTimeInMinutes)
                {
                    var cookieOptions = new CookieOptions
                    {
                        HttpOnly = true,
                        SameSite = SameSiteMode.Strict,
                        Expires = DateTime.UtcNow.AddSeconds(tokenExpirationTimeInMinutes),
                    };
                    Response.Cookies.Append("accessToken", jwtAccessToken, cookieOptions);
                }
                void SetAccessTokenForDataRetriving(string jwtAccessToken, int tokenExpirationTimeInMinutes)
                {
                    var cookieOptions = new CookieOptions
                    {
                        HttpOnly = true,
                        SameSite = SameSiteMode.Strict,
                        Expires = DateTime.UtcNow.AddMinutes(tokenExpirationTimeInMinutes).AddSeconds(20),
                    };
                    Response.Cookies.Append("accessTokenForDataRetriving", jwtAccessToken, cookieOptions);
                }
                void SetRefreshToken(RefreshTokenProvider jwtRefreshToken)
                {
                    var cookieOptions = new CookieOptions
                    {
                        HttpOnly = true,
                        SameSite = SameSiteMode.Strict,
                        Expires = jwtRefreshToken.RefreshTokenExpiresAt
                    };
                    Response.Cookies.Append("refreshToken", jwtRefreshToken.RefreshToken, cookieOptions);
                }
            }
            return Unauthorized("Wrong password!");
        }

        [HttpGet]
        [Route("/Authorization/Refresh")]
        public async Task<ActionResult> Refresh()
        {
            var jwtAccessToken = Request.Cookies["accessTokenForDataRetriving"];
            var refreshToken = Request.Cookies["refreshToken"];
            var principal = tokenService.GetPrincipalFromExpiredToken(jwtAccessToken);
            var username = principal?.Identity?.Name;
            var tokenDetails = repository.GetUserTokenDetails(username);

            if (tokenDetails is null || tokenDetails.RefreshToken != refreshToken || tokenDetails.RefreshTokenExpiresAt <= DateTime.Now)
            {
                return BadRequest("Invalid client request");
            }

            string newJwtAccessToken = tokenService.GenerateAccessToken(principal.Claims, tokenExpirationTimeInMinutes);
            SetAccessToken(newJwtAccessToken, tokenExpirationTimeInMinutes);
            var jwtRefreshToken = await tokenService.AssignRefreshToken(tokenDetails.UserId);
            SetRefreshToken(jwtRefreshToken);

            return Ok();
            void SetAccessToken(string jwtAccessToken, int tokenExpirationTimeInMinutes)
            {
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddMinutes(tokenExpirationTimeInMinutes)
                };
                Response.Cookies.Append("accessToken", jwtAccessToken, cookieOptions);
            }

            void SetRefreshToken(RefreshTokenProvider jwtRefreshToken)
            {
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = jwtRefreshToken.RefreshTokenExpiresAt,
                };
                Response.Cookies.Append("refreshToken", jwtRefreshToken.RefreshToken, cookieOptions);
            }

        }

    }

}




//public static class ClaimsExtensions
//{
//    public const string SystemAdminRole = "System Admin";

//    public static readonly string UserIdType = "UserId";
//    public static readonly string FirstNameType = "FirstName";
//    public static readonly string MiddleNameType = "MiddleName";
//    public static readonly string LastNameType = "LastName";
//    public static readonly string UserNameType = "UserName";
//    public static readonly string EmailType = "Email";
//    public static readonly string LockedType = "Locked";

//public static Guid GetSub(this ClaimsPrincipal user)
//{
//    var sub = user.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
//    if (!Guid.TryParse(sub, out var subGuid)) throw new InvalidCastException("Invalid sub claim");

//    return subGuid;
//}

//public static int GetUserId(this ClaimsPrincipal user)
//{
//    var userIdValue = user.Claims.FirstOrDefault(c => c.Type == UserIdType)?.Value;
//    if (!int.TryParse(userIdValue, out var userId)) throw new InvalidCastException("Invalid userId claim");

//    return userId;
//}

//public static string GetFirstName(this ClaimsPrincipal user)
//{
//    return user.Claims.FirstOrDefault(c => c.Type == FirstNameType)?.Value;
//}

//public static string GetMiddleName(this ClaimsPrincipal user)
//{
//    return user.Claims.FirstOrDefault(c => c.Type == MiddleNameType)?.Value;
//}
//}