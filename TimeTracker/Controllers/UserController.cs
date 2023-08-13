using AutoMapper;
using Contracts;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TimeTracker.Data;
using TimeTracker.DTOs;
using TimeTracker.Models;
using TimeTracker.Services;

namespace TimeTracker.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly IUserRepo repository;
        private readonly IMapper mapper;
        private readonly IRequestClient<NewComerApprovalRequest> newComerApprovalRequestClient;
        private readonly IRequestClient<CheckApprovalStatus> checkApprovalStatusClient;
        private readonly IRequestClient<NewComerRequestApproved> toApproveNewComer;
        private readonly IRequestClient<NewComerRequestRejected> toRejectNewComer;
        private readonly ITokenService tokenService;
        private readonly ILogger<UserController> logger;

        public UserController(ILogger<UserController> logger, IUserRepo repository, IMapper mapper,
            IRequestClient<NewComerApprovalRequest> newComerApprovalRequestClient, IRequestClient<CheckApprovalStatus> checkApprovalStatusClient,
            IRequestClient<NewComerRequestApproved> toApproveNewComer, IRequestClient<NewComerRequestRejected> toRejectNewComer, ITokenService tokenService)
        {
            this.logger = logger;
            this.repository = repository;
            this.mapper = mapper;
            this.newComerApprovalRequestClient = newComerApprovalRequestClient ?? throw new ArgumentNullException(nameof(newComerApprovalRequestClient));
            this.checkApprovalStatusClient = checkApprovalStatusClient ?? throw new ArgumentNullException(nameof(checkApprovalStatusClient));
            this.toApproveNewComer = toApproveNewComer ?? throw new ArgumentNullException(nameof(toApproveNewComer));
            this.toRejectNewComer = toRejectNewComer ?? throw new ArgumentNullException(nameof(toRejectNewComer));
            this.tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
        }

        [Authorize(Policy = "Developers")]
        public async Task<IActionResult> ToFillInWorkHours(UserCreateDTO requestedUser)
        {
            var mappedUser = mapper.Map<User>(requestedUser);
            var jwtAccessToken = Request.Cookies["accessTokenForDataRetriving"];
            if (jwtAccessToken != null)
            {
                var principal = tokenService.GetPrincipalFromExpiredToken(jwtAccessToken);
                var username = principal?.Identity?.Name;
                var tokenDetails = repository.GetUserTokenDetails(username);
                await repository.AddWorkedHours(mappedUser, tokenDetails.UserId);

                return RedirectToAction("GetAttendanceOfUser");
            }

            return NotFound();
        }


        [Authorize(Policy = "Developers")]
        [HttpGet]
        public ActionResult<IEnumerable<UserReadDTO>> GetAttendanceOfUser(string search)
        {
            ViewData["GetUserDetails"] = search;
            var attendanceOfUser = repository.GetAttendanceOfUser(search);

            return View(mapper.Map<IEnumerable<UserReadDTO>>(attendanceOfUser));
        }

        [Authorize(Policy = "HR")]
        [HttpGet]
        public IActionResult CreateUser()
        {
            return View();
        }

        [Authorize(Policy = "HR")]
        public async Task<IActionResult> ToRejectNewComer(Guid id)
        {
            var response = await toRejectNewComer.GetResponse<NewComerRequestRejected>(new
            {
                ApprovalId = id,
                TimeStamp = InVar.Timestamp,
            });

            await repository.UpdateApprovalStatus(response.Message);

            return Ok(response);
        }

        [Authorize(Policy = "HR")]
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
            var status = await checkApprovalStatusClient.GetResponse<ApprovalStatus>(new { ApprovalId = id });

            ViewData["ApprovalStatus"] = status.Message.State;
            return View();

        }

        [HttpGet]
        public IActionResult GetNewComersRequestedForApproval()
        {
            var newComersRequestedForApproval = repository.NewComersRequestedForApproval();
            if (newComersRequestedForApproval != null)
            {
                return View(newComersRequestedForApproval);
            }
            else
            {
                return View();
            }
        }

        [HttpGet]
        public IActionResult GetNewComersStatus()
        {
            var newComersRequestedForApproval = repository.GetNewComersApprovalStatus();

            if (newComersRequestedForApproval != null)
            {
                return View(newComersRequestedForApproval);
            }
            else
            {
                return View();
            }
        }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUser(UserCreateDTO requestedUser)
        {
            if (ModelState.IsValid)
            {
                var mappedUser = mapper.Map<User>(requestedUser);
                await repository.CreateUser(mappedUser, requestedUser);
                logger.LogInformation("Message sent");
                var response = await newComerApprovalRequestClient.GetResponse<NewComerApprovalRequestAccepted>(new
                {
                    ApprovalId = requestedUser.ApprovalId,
                    TimeStamp = InVar.Timestamp,
                    UserEmail = requestedUser.Email
                });

                return Ok(response);
            }
            return View(requestedUser);
        }

        [Authorize(Policy = "Manager")]
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

        [Authorize(Policy = "Manager")]
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

        [Authorize(Policy = "Developers")]
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

        [Authorize(Policy = "HR")]
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

        [Authorize(Policy = "HR")]
        [HttpPost]
        public async Task<IActionResult> DeleteUser(int id)
        {
            await repository.DeleteUser(id);
            return RedirectToAction("GetAttendanceOfUser");
        }

        [Authorize(Policy = "TeamAccess")]
        public ActionResult<UserReadDTO> GetAllEmployeesInfo()
        {
            var totalAttendance = repository.GetAllEmployeesInfo();
            return View(mapper.Map<HashSet<UserReadDTO>>(totalAttendance));
        }
    }
}

