using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using TaniLink_Backend.Models;

namespace TaniLink_Backend.Controllers
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
            ViewData["Template"] = "DefaultTemplate";
            return View();
        }

        public IActionResult Privacy()
        {
            ViewData["Template"] = "DefaultTemplate";
            return View();
        }

        public IActionResult AccessDenied()
        {
            ViewData["Template"] = "DefaultTemplate";
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            ViewData["Template"] = "DefaultTemplate";
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
