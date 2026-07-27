using Freelify.Services;
using Freelify.Services;
using Freelify.Models.ViewModels.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;


namespace Freelify.Controllers
{
    public class AccountController : Controller
    {
        private readonly AccountService _accountService;
        public readonly AdminService _adminService;

        public AccountController(AccountService accountService , AdminService adminService)
        {
            _accountService = accountService;
            _adminService = adminService;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _accountService.RegisterAsync(model);

            if (result.Succeeded)
            {

                TempData["Success"] = "Registration successful. We've sent a confirmation email. Please check your inbox.";

                return RedirectToAction("Login", "Account");

            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            var result = await _accountService.ConfirmEmailAsync(userId, token);

            if (result.Succeeded)
            {
                return View("ConfirmEmailSuccess");
            }

            return View("ConfirmEmailFailed");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResendConfirmationEmail(string email)
        {
            var result = await _accountService.ResendConfirmationEmailAsync(email);

            if (result.Succeeded)
            {
                TempData["Success"] =
                    "A new confirmation email has been sent.";
            }
            else
            {
                TempData["Error"] = result.Errors.First().Description;
            }

            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(Models.ViewModels.Auth.ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _accountService.SendPasswordResetEmailAsync(model.Email);

            if (result.Succeeded)
            {
                TempData["Success"] = "If the email exists, a reset link has been sent.";
                return RedirectToAction("Login");
            }

            TempData["Error"] = result.Errors.FirstOrDefault()?.Description ?? "Unable to send reset email.";
            return View(model);
        }

        // Password reset actions
        [HttpGet]
        public IActionResult ResetPassword(string userId, string token)
        {
            var model = new Models.ViewModels.Auth.ResetPasswordViewModel
            {
                UserId = userId,
                Token = token
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(Models.ViewModels.Auth.ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _accountService.ResetPasswordAsync(model.UserId, model.Token, model.NewPassword);

            if (result.Succeeded)
            {
                TempData["Success"] = "Password has been reset successfully.";
                return RedirectToAction("Login");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {

                if (!ModelState.IsValid)
                    return View(model);

                var LoginResult = await _accountService.LoginAsync(model);


                if (!LoginResult.Success)
                {

                if (LoginResult.Message == "Please confirm your email first.")
                {
                    ViewBag.ShowResendButton = true;
                }

                ModelState.AddModelError("", LoginResult.Message);
                    return View(model);

                }



                if (LoginResult.Role == "Freelancer")
                {
                return RedirectToAction("Index", "JobBrowse");

                }

                if (LoginResult.Role == "Client")
            {
                return RedirectToAction("MyJobs", "Job");
            }



                else //Admin
                {
                    return RedirectToAction("Admin", "Account");
                }





        }
            [Authorize(Roles = "Admin")]
            public async Task<IActionResult> Admin()
            {
                var model = await _adminService.GetDashboardAsync();

                return View("~/Views/Admin/Dashboard.cshtml", model);
            }

        [HttpGet]
        public async Task<IActionResult> LogOut()
        {

             await _accountService.LogOutAsync();

           return RedirectToAction("Index", "Home");

        }


        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }


        [Authorize]
        public  IActionResult test()
        {
            return View();
        }




    }
}
