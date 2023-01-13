using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using NuGet.Common;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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

        public AuthorizationController(ILogger<AuthorizationController> logger, IConfiguration configuration)
        {
            this.logger = logger;
            this.configuration = configuration;
        }

        [HttpGet]
        public IActionResult Authorize()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Authorize(User user)
        {
            if (ModelState.IsValid)
            {
                User userFromDB = new User();
                if (user.Email.Equals(userFromDB.Email) & user.Password.Equals(userFromDB.Password))
                {
                    //UserAuthenticationcs.ToAuthenticateUser(user.Name);

                    List<Claim> claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.Name),
                        new Claim(ClaimTypes.Email, user.Email)
                    };

                    var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configuration.GetSection("JWT:Key").Value));
                    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

                    var setToken = new JwtSecurityToken(
                        configuration["JWT:Issuer"],
                        configuration["JWT:Audience"],
                        claims: claims,
                        expires: DateTime.Now.AddMinutes(5),
                        signingCredentials: creds);;

                    var jwt = new JwtSecurityTokenHandler().WriteToken(setToken);
                    return Ok(jwt);
                }
                else BadRequest(StatusCodes.Status401Unauthorized);
            }
            return View();
        }
    }
}