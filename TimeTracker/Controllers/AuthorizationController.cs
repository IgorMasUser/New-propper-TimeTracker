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

        public AuthorizationController(ILogger<AuthorizationController> logger, IConfiguration configuration, IMapper mapper, IUserRepo repository, ITokenService tokenService)
        {
            this.logger = logger;
            this.configuration = configuration;
            this.mapper = mapper;
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
            this.tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
        }

        [HttpGet]
        public IActionResult Authorize()
        {
            return View();
        }

        [HttpPost]
        [Route("/Authorization/Authorize")]
        public async Task<ActionResult<string[]>> Authorize(UserCreateDTO requestedUser)
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
               
                var refreshToken = Request.Cookies["refreshToken"];
                if (refreshToken != null)
                {

                    var tokenDetails = repository.GetUserTokenDetails(refreshToken);

                    if (tokenDetails is null || tokenDetails.RefreshToken != refreshToken || tokenDetails.RefreshTokenExpiresAt <= DateTime.Now)
                    {
                        return BadRequest("Invalid client request");
                    }

                    string jwtAccessToken = tokenService.GenerateAccessToken(claims);
                    SetAccessToken(jwtAccessToken);

                    //return Ok(jwtAccessToken);
                   return Ok();
                }
                else
                {
                    var jwtRefreshToken = await tokenService.AssignRefreshToken(obtainedUser);
                    SetRefreshToken(jwtRefreshToken);
                }

                void SetAccessToken(string jwtAccessToken)
                {
                    var cookieOptions = new CookieOptions
                    {
                        HttpOnly = true,
                        SameSite = SameSiteMode.Strict,
                        Expires = DateTime.Now.AddMinutes(15)
                    };
                    Response.Cookies.Append("accessToken", jwtAccessToken, cookieOptions);
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