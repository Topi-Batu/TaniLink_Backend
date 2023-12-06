using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using TaniLink_Backend.Models;

namespace TaniLink_Backend.Controllers
{
    public class UserController : Controller
    {
        private readonly UserManager<User> _userManager;

        public UserController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet()]
        public async Task<IActionResult> Verify([FromQuery] string token, [FromQuery] string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                if (user.EmailConfirmed != true)
                {
                    var tokenReal = Encoding.UTF8.GetString(Convert.FromBase64String(token));
                    var result = await _userManager.ConfirmEmailAsync(user, tokenReal);
                    if (result.Succeeded)
                    {
                        ViewBag.IsSuccess = true;
                        return View();
                    }
                }
                else
                    ViewBag.Message = "Email already verified or link expired";
            }
            else
                ViewBag.Message = "User not found";

            ViewBag.IsSuccess = false;
            return View();
        }
    }
}
