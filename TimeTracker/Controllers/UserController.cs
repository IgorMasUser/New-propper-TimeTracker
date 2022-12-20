﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
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
            DateTime finished = user.Finished;
            DateTime started = user.Started;
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
                user.TotalWorked = TimeCalc(ref user);
                user.Date = user.Started;
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
                user.TotalWorked = TimeCalc(ref user);
                user.Date = user.Started;
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

        public class UserComparer : IEqualityComparer<User>
        {
            public bool Equals(User? x, User? y)
            {
                return String.Equals(x.UserId, y.UserId);
            }

            public int GetHashCode([DisallowNull] User obj)
            {
                return obj.UserId.GetHashCode();
            }
        }

        public IActionResult ViewUsers()
        {
            using (db)
            {
                int i = 0;
                var data = db.User.Select(x => x);
                var setOfUsers = new HashSet<User>(new UserComparer());

                foreach (var user in data)
                {
                    setOfUsers.Add(user);
                }

                var listOfUsers = new List<User>();
                var userCalculatedTime = new List<DateTime>();

                foreach (var user in data)
                {
                    listOfUsers.Add(user);
                }
                foreach (var item in listOfUsers)
                {
                    foreach (var item2 in listOfUsers)
                    {
                        if (item.UserId.Equals(item2.UserId))
                        {
                            item.UserTotalWorked = item.UserTotalWorked.Add(item2.TotalWorked.TimeOfDay);
                        }
                    }
                    userCalculatedTime.Add(item.UserTotalWorked);
                }
                foreach (var count in setOfUsers)
                {
                    i++;
                    count.Numeration = i;
                }
                return View(setOfUsers);
            }
        }
    }
}

