using Freelify.Models.ViewModels;
using Freelify.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Freelify.Controllers
{
    public class AccountController : Controller
    {
        private readonly AccountService _accountService;

        public AccountController(AccountService accountService)
        {
            _accountService = accountService;
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
                return RedirectToAction("Login", "Account");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {

                if (!ModelState.IsValid)
                    return View(model);

                var LoginResult = await _accountService.LoginAsync(model);


                if (!LoginResult.Success)
                {
                    ModelState.AddModelError("", LoginResult.Message);
                    return View(model);

                }


                if (LoginResult.Role=="Freelancer")
                {
                return RedirectToAction("Index", "Freelancer");

            }
                else if (LoginResult.Role == "Client")
                {
                    return RedirectToAction("MyJobs", "Job");
                }
                else //Admin
                {
                    return RedirectToAction("Index", "Admin");
                }
            
          


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