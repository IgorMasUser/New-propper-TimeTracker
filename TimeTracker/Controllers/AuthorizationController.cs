using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
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
                var obtainedUser = repository.GetUserDetails(mappedUser);
                List<Claim> claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, obtainedUser.Name),
                        new Claim(ClaimTypes.Email, obtainedUser.Email),
                        new Claim(ClaimTypes.Role, obtainedUser.Role.ToString())
                    };
                string jwtAccessToken = tokenService.GenerateAccessToken(claims, tokenExpirationTimeInMinutes);
                SetAccessToken(jwtAccessToken, tokenExpirationTimeInMinutes);
                SetAccessTokenForDataRetriving(jwtAccessToken, tokenExpirationTimeInMinutes);

                //return Ok(jwtAccessToken); //If we want to expose token to frontend so it cares about token
                var jwtRefreshToken = await tokenService.GenerateAndAssignRefreshToken(obtainedUser.UserId);
                SetRefreshToken(jwtRefreshToken);

                return Ok(string.Format("Hello {0} {1}", obtainedUser.Name, obtainedUser.Surname));
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
            SetAccessTokenForDataRetriving(newJwtAccessToken, tokenExpirationTimeInMinutes);
            var newJwtRefreshToken = await tokenService.GenerateAndAssignRefreshToken(tokenDetails.UserId);
            if (!newJwtRefreshToken.Equals(refreshToken))
            {
                SetRefreshToken(newJwtRefreshToken);
            }
            return Ok();
        }

        void SetAccessToken(string jwtAccessToken, int tokenExpirationTimeInMinutes)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddMinutes(tokenExpirationTimeInMinutes),
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
}