using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;
using TimeTracker.BusinessLogic;
using TimeTracker.Data;
using TimeTracker.Models;


namespace TimeTracker.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext db;

        private readonly ILogger<UserController> logger;

        public UserController(ILogger<UserController> logger, ApplicationDbContext db)
        {
            this.logger = logger;
            this.db = db;
        }

        public DateTime TimeCalc(ref User user)
        {
            DateTime breake = new DateTime().AddMinutes(user.Break);
            TimeSpan temp = new DateTime().Subtract(breake);
            DateTime finished = user.FinishedWorkDayAt;
            DateTime started = user.StartedWorkDayAt;
            TimeSpan workDay = finished.Subtract(started.Subtract(temp));
            DateTime total = Convert.ToDateTime(workDay.ToString());

            return total;
        }

        [HttpGet]
        public IActionResult GetAttendance(string search)
        {
            ViewData["GetUserDetails"] = search;
            var data = db.User.Select(x => x);
            if (!string.IsNullOrEmpty(search))
            {
                data = data.Where(a => a.Name.Contains(search) || a.Surname.Contains(search));
            }
            return View(data);
        }

        //get
        public IActionResult Create()
        {
            return View();
        }

        //post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(User user)
        {
            if (ModelState.IsValid)
            {
                user.TotalWorkedPerDay = TimeCalc(ref user);
                user.Date = user.StartedWorkDayAt;
                db.User.Add(user);
                await db.SaveChangesAsync();

                return RedirectToAction("GetAttendance");
            }
            return View(user);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("GetAttendance");
            }
            var user = await db.User.FindAsync(id);
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(User user)
        {
            if (ModelState.IsValid)
            {
                db.User.Update(user);
                user.TotalWorkedPerDay = TimeCalc(ref user);
                user.Date = user.StartedWorkDayAt;
                await db.SaveChangesAsync();
                return RedirectToAction("GetAttendance");
            }
            return View(user);
        }

        public async Task<IActionResult> GetDetails(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("GetAttendance");
            }
            var user = await db.User.FindAsync(id);
            return View(user);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("GetAttendance");
            }
            var user = await db.User.FindAsync(id);
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await db.User.FindAsync(id);
            if (user != null)
            {
                db.User.Remove(user);
                await db.SaveChangesAsync();
                return RedirectToAction("GetAttendance");
            }
            return NotFound(id);
        }

        public IActionResult ViewUsers()
        {
            using (db)
            {
                var data = db.User.Select(x => x).ToList();
                var setOfUsers = new UserTimeCalculator().GetTotalWorkedTimeForAllUsers(data);
                return View(setOfUsers);
            }
        }
    }
}

