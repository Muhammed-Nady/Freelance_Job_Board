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
                return RedirectToAction("Index", "Home");
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var serviceResult = await _accountService.LoginAsync(model);


            if (!serviceResult.Success)
            { 
                ModelState.AddModelError("", serviceResult.Message);
                return View(model);

            }
           

          if(User.IsInRole("Freelancer"))
            {
                return RedirectToAction("Index", "Freelancer");

            }
            else if (User.IsInRole("Client"))
            {
                return RedirectToAction("Index", "Client");
            }
            else //Admin
            {
                return RedirectToAction("Index", "Admin");
            }
          


        }

      
    }
}