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


namespace TimeTracker.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserRepo repository;
        private readonly IMapper mapper;
        private readonly IConfiguration configuration;
        private readonly ILogger<UserController> logger;

        public UserController(ILogger<UserController> logger, IUserRepo repository, IMapper mapper, IConfiguration configuration)
        {
            this.logger = logger;
            this.repository = repository;
            this.mapper = mapper;
            this.configuration = configuration;
        }

        [HttpGet]
        public IActionResult Authorization()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Authorization(User user)
        {
            if (ModelState.IsValid)
            {
                User userFromDB = new User();
                if (user.Email.Equals(userFromDB.Email) & user.Password.Equals(userFromDB.Password))
                {
                    //UserAuthenticationcs.ToAuthenticateUser(user.Name);

                    List<Claim> claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.Name)
                    };

                    var key = new SymmetricSecurityKey(System.Text.Encoding.ASCII.GetBytes(configuration.GetSection("JWT:Key").Value));
                    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

                    var setToken = new JwtSecurityToken(
                        claims: claims,
                        expires: DateTime.Now.AddMinutes(5),
                        signingCredentials: creds);

                    var jwt = new JwtSecurityTokenHandler().WriteToken(setToken);
                    return Ok(jwt);
                }
                else BadRequest(StatusCodes.Status401Unauthorized);
            }
            return View();
        }

        [HttpGet]
        public ActionResult<IEnumerable<UserReadDTO>> GetAttendanceOfUser(string search)
        {
            ViewData["GetUserDetails"] = search;
            var attendanceOfUser = repository.GetAttendanceOfUser(search);

            return View(mapper.Map<IEnumerable<UserReadDTO>>(attendanceOfUser));
        }

        [Authorize]
        public IActionResult CreateUser()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUser(User user)
        {
            if (ModelState.IsValid)
            {
                await repository.CreateUser(user);

                return RedirectToAction("GetAttendanceOfUser");
            }
            return View(user);
        }

        public async Task<IActionResult> EditAttendanceOfUser(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("GetAttendanceOfUser");
            }
            var getUserToEdit = await repository.EditAttendanceOfUser(id);
            var userDTO = mapper.Map<UserReadDTO>(getUserToEdit);
            return View(userDTO);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAttendanceOfUser(UserCreateDTO user)
        {
            if (ModelState.IsValid)
            {
                var mappedUser = mapper.Map<User>(user);
                await repository.EditAttendanceOfUser(mappedUser);
                return RedirectToAction("GetAttendanceOfUser");
            }
            return View(user);
        }

        public async Task<IActionResult> GetDetailsOfUser(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("GetAttendanceOfUser");
            }
            var foundUser = await repository.GetDetailsOfUser(id);
            var mappedUser = mapper.Map<UserReadDTO>(foundUser);
            return View(mappedUser);
        }

        public async Task<IActionResult> DeleteUser(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("GetAttendanceOfUser");
            }
            var foundUser = await repository.GetUserToDelete(id);
            var mappedUser = mapper.Map<UserReadDTO>(foundUser);
            return View(mappedUser);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(int id)
        {
            await repository.DeleteUser(id);
            return RedirectToAction("GetAttendanceOfUser");
        }

        public ActionResult<UserReadDTO> GetAllEmployeesInfo()
        {
            var totalAttendance = repository.GetAllEmployeesInfo();
            return View(mapper.Map<HashSet<UserReadDTO>>(totalAttendance));

        }
    }
}

