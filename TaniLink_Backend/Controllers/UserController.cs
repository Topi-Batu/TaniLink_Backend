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
        private readonly SignInManager<User> _signInManager;

        public UserController(UserManager<User> userManager,
            SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "User");
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity.IsAuthenticated)
                return Redirect(returnUrl ?? Url.Content("~/Dashboard"));
            ViewData["Template"] = "NoTemplate";
            LoginViewModel loginViewModel = new LoginViewModel();
            loginViewModel.ReturnUrl = returnUrl ?? Url.Content("~/Dashboard");
            return View(loginViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            ViewData["Template"] = "NoTemplate";
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(loginViewModel.Email, loginViewModel.Password, loginViewModel.RememberMe, lockoutOnFailure: true);
                if (result.Succeeded)
                {
                    return Redirect(loginViewModel.ReturnUrl ?? Url.Content("~/"));
                }
                if (result.IsLockedOut)
                {
                    ModelState.AddModelError(string.Empty, "Your account is locked out. Kindly wait for 1 minutes and try again");
                    return View(loginViewModel);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View(loginViewModel);
                }
            }
            return View(loginViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Verify([FromQuery] string token, [FromQuery] string id)
        {
            ViewData["Template"] = "DefaultTemplate";
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
            ViewData["Template"] = "DefaultTemplate";
            ResetPasswordViewModel resetPasswordViewModel = new ResetPasswordViewModel();
            resetPasswordViewModel.Token = token;
            resetPasswordViewModel.Id = id;
            return View(resetPasswordViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel resetPasswordViewModel)
        {
            ViewData["Template"] = "DefaultTemplate";
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
