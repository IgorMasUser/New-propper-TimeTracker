﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;
using TimeTracker.BusinessLogic;
using TimeTracker.Data;
using TimeTracker.DTOs;
using TimeTracker.Models;


namespace TimeTracker.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly IUserRepo repository;
        private readonly IMapper mapper;
        private readonly ILogger<UserController> logger;

       // public UserController(ILogger<UserController> logger, ApplicationDbContext db, IUserRepo repository, IMapper mapper)
       public UserController(ILogger<UserController> logger, ApplicationDbContext db, IUserRepo repository, IMapper mapper)
        {
            this.logger = logger;
            this.db = db;
            this.repository = repository;
            this.mapper = mapper;
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
                user.TotalWorkedPerDay = TimeCalculator.ToCalcWorkTimePerDay(ref user);
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
                user.TotalWorkedPerDay = TimeCalculator.ToCalcWorkTimePerDay(ref user);
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

        public ActionResult<UserDTO> GetAllEmployeesInfo()
        {
            var attendance = repository.GetAllEmployeesInfo();
            return View(mapper.Map<HashSet<UserDTO>>(attendance));

        }
    }
}

