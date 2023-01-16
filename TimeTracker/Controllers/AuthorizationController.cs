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

namespace TimeTrackerControllers
{
    public class AuthorizationController : Controller
    {
        private readonly ILogger<AuthorizationController> logger;
        private readonly IConfiguration configuration;
        private readonly IMapper mapper;
        private readonly IUserRepo repository;

        public AuthorizationController(ILogger<AuthorizationController> logger, IConfiguration configuration, IMapper mapper, IUserRepo repository)
        {
            this.logger = logger;
            this.configuration = configuration;
            this.mapper = mapper;
            this.repository = repository;
        }

        //[HttpGet]
        //public IActionResult Register()
        //{
        //    return View();
        //}

        //[HttpPost]
        //public async Task<IActionResult> Register(UserCreateDTO requestedUser)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var mappedUser = mapper.Map<User>(requestedUser);
        //        await repository.RegisterUser(mappedUser, requestedUser);

        //        return RedirectToAction("GetAttendanceOfUser");
        //    }
        //    return View(requestedUser);
        //}

        [HttpGet]
        public IActionResult Authorize()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Authorize(UserCreateDTO requestedUser)
        {
            var mappedUser = mapper.Map<User>(requestedUser);
            if (repository.CheckIfUserExists(mappedUser, requestedUser))
            {
                var obtainedUser = repository.GetUserClaims(mappedUser, requestedUser);

                List<Claim> claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, obtainedUser.Name),
                        new Claim(ClaimTypes.Email, obtainedUser.Email)
                    };

                var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configuration.GetSection("JWT:Key").Value));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

                var setToken = new JwtSecurityToken(
                    configuration["JWT:Issuer"],
                    configuration["JWT:Audience"],
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(15),
                    signingCredentials: creds); ;

                var jwt = new JwtSecurityTokenHandler().WriteToken(setToken);
                return Ok(jwt);
            }

            return BadRequest("Wrong password!");

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