using AutoMapper;
using Contracts;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TimeTracker.Data;
using TimeTracker.DTOs;
using TimeTracker.Models;


namespace TimeTracker.Controllers
{
    //[Authorize]
    public class UserController : Controller
    {
        private readonly IUserRepo repository;
        private readonly IMapper mapper;
        private readonly IConfiguration configuration;
        private readonly IRequestClient<NewComerApprovalRequest> newComerApprovalRequestClient;
        private readonly IRequestClient<CheckApprovalStatus> checkApprovalStatusClient;
        private readonly IRequestClient<NewComerRequestApproved> toApproveNewComer;
        private readonly ILogger<UserController> logger;

        public UserController(ILogger<UserController> logger, IUserRepo repository, IMapper mapper, IConfiguration configuration,
            IRequestClient<NewComerApprovalRequest> newComerApprovalRequestClient, IRequestClient<CheckApprovalStatus> checkApprovalStatusClient,
            IRequestClient<NewComerRequestApproved> toApproveNewComer)
        {
            this.logger = logger;
            this.repository = repository;
            this.mapper = mapper;
            this.configuration = configuration;
            this.newComerApprovalRequestClient = newComerApprovalRequestClient;
            this.checkApprovalStatusClient = checkApprovalStatusClient;
            this.toApproveNewComer = toApproveNewComer;
        }

        //[Authorize(Policy = "Manager")]
        [HttpGet]
        public ActionResult<IEnumerable<UserReadDTO>> GetAttendanceOfUser(string search)
        {
            ViewData["GetUserDetails"] = search;
            var attendanceOfUser = repository.GetAttendanceOfUser(search);

            return View(mapper.Map<IEnumerable<UserReadDTO>>(attendanceOfUser));
        }

        //[Authorize(Policy = "HR")]
        [HttpGet]
        public IActionResult CreateUser()
        {
            return View();
        }


        //[Authorize(Policy = "HR")]
        [HttpGet]
        public async Task<IActionResult> ToApproveNewComer(Guid id)
        {
            var response = await toApproveNewComer.GetResponse<NewComerRequestApproved>(new
            {
                ApprovalId = id,
                TimeStamp = InVar.Timestamp,
            });

            await repository.UpdateApprovalStatus(response.Message);

            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetUserApprovalStatus(Guid id)
        {
            var (status, notFound) = await checkApprovalStatusClient.GetResponse<ApprovalStatus, NewComerNotFound>(new { ApprovalId = id });
            if (status.IsCompleted)
            {
                var response = await status;
                return Ok(response.Message);
            }

            else
            {
                var response = await notFound;
                return NotFound(response.Message);
            }
        }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUser(UserCreateDTO requestedUser)
        {
            if (ModelState.IsValid)
            {
                var mappedUser = mapper.Map<User>(requestedUser);
                await repository.CreateUser(mappedUser, requestedUser);
                Console.WriteLine("Message sent");
                logger.LogInformation("Message sent");
                var response = await newComerApprovalRequestClient.GetResponse<NewComerApprovalRequestAccepted>(new
                {
                    ApprovalId = Guid.NewGuid(),
                    TimeStamp = InVar.Timestamp,
                    UserEmail = requestedUser.Email
                });

                return Ok(response);
            }
            return View(requestedUser);
        }

        //[Authorize(Policy = "Manager")]
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

        //[Authorize(Policy = "Manager")]
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

        //[Authorize(Policy = "Developers")]
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

        //[Authorize(Policy = "HR")]
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

        //[Authorize(Policy = "HR")]
        [HttpPost]
        public async Task<IActionResult> DeleteUser(int id)
        {
            await repository.DeleteUser(id);
            return RedirectToAction("GetAttendanceOfUser");
        }

        //[Authorize(Policy = "TeamAccess")]
        public ActionResult<UserReadDTO> GetAllEmployeesInfo()
        {
            var totalAttendance = repository.GetAllEmployeesInfo();
            return View(mapper.Map<HashSet<UserReadDTO>>(totalAttendance));
        }
    }
}

