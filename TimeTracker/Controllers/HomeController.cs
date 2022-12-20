using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using TimeTracker.Models;
using TimeTracker.Data;

namespace TimeTrackerControllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}