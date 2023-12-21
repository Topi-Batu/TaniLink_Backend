using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TaniLink_Backend.Controllers.Dashboard
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [Area("Dashboard")]
        [Authorize/*(Roles = "Admin")*/]
        [Route("Dashboard")]
        public IActionResult Index()
        {

            return View();
        }
    }
}
