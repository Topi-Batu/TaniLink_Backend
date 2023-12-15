using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using TaniLink_Backend.Models;
using TaniLink_Backend.ViewModels;

namespace TaniLink_Backend.Controllers
{
    public class UserController : Controller
    {
        private readonly UserManager<User> _userManager;

        public UserController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet]
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

        [HttpGet]
        public IActionResult ResetPassword(string? token, string? id)
        {
            ResetPasswordViewModel resetPasswordViewModel = new ResetPasswordViewModel();
            resetPasswordViewModel.Token = token;
            resetPasswordViewModel.Id = id;
            return View(resetPasswordViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel resetPasswordViewModel)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(resetPasswordViewModel.Id);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "User not found.");
                    return View(resetPasswordViewModel);
                }

                if (resetPasswordViewModel.Password != resetPasswordViewModel.ConfirmPassword)
                {
                    ModelState.AddModelError(string.Empty, "Password and Confirm Password do not match.");
                    return View(resetPasswordViewModel);
                }
                var token = Encoding.UTF8.GetString(Convert.FromBase64String(resetPasswordViewModel.Token));
                var result = await _userManager.ResetPasswordAsync(user, token, resetPasswordViewModel.Password);
                if (!result.Succeeded)
                {

                    ModelState.AddModelError(string.Empty, result.Errors.FirstOrDefault()?.Description.ToString());
                    return View(resetPasswordViewModel);
                }

            }
            else
            {
                ModelState.AddModelError(string.Empty, "Harus diisi semua.");
                return View(resetPasswordViewModel);
            }

            ModelState.AddModelError(string.Empty, "Password reset successfully.");
            return View(resetPasswordViewModel);

        }
    }
}
