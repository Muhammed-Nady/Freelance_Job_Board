using Freelify.Models.Enums;
using Freelify.Models.ViewModels;
using Freelify.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Freelify.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly ProfileService _profileService;

        public ProfileController(ProfileService profileService)
        {
            _profileService = profileService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {


            var profileResult = await _profileService.GetProfileDetailsAsync(User);
            if (!profileResult.Success)
            {
                return profileResult.ErrorType switch
                {
                    ErrorType.NotFound => NotFound(profileResult.ErrorMessage),
                    ErrorType.BadRequest => BadRequest(profileResult.ErrorMessage),
                    _ => BadRequest(profileResult.ErrorMessage)
                };
            }

            return View(profileResult.ViewName, profileResult.ViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Edit()
        {


            var editResult = await _profileService.GetEditProfileDetailsAsync(User);
            if (!editResult.Success)
            {
                return editResult.ErrorType switch
                {
                    ErrorType.NotFound => NotFound(editResult.ErrorMessage),
                    ErrorType.BadRequest => BadRequest(editResult.ErrorMessage),
                    _ => BadRequest(editResult.ErrorMessage)
                };
            }

            var model = (EditFreelancerProfileViewModel)editResult.ViewModel;

            await _profileService.LoadSkillsAsync(
                ViewBag,
                model.SelectedSkillIds);

            return View(editResult.ViewName, editResult.ViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditFreelancer(EditFreelancerProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("EditFreelancer", model);
            }

            var editResult = await _profileService.EditFreelancerProfile(User, model);
            if (!editResult.Success)
            {
                ModelState.AddModelError(string.Empty, editResult.ErrorMessage);
                await _profileService.LoadSkillsAsync(ViewBag,model.SelectedSkillIds);
                return View(model);
            }

            TempData["SuccessMessage"] = "Freelancer profile updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditClient(EditClientProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("EditClient", model);
            }

            var editResult = await _profileService.EditClientProfile(User, model);
            if (!editResult.Success)
            {
                ModelState.AddModelError(string.Empty, editResult.ErrorMessage);
                return View(model);
            }

            TempData["SuccessMessage"] = "Client profile updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var changePasswordResult = await _profileService.ChangePassword(User, model);

            if (!changePasswordResult.Success)
            {
                ModelState.AddModelError(string.Empty, changePasswordResult.ErrorMessage);
                return View(model);
            }

            TempData["SuccessMessage"] = "Password changed successfully";
            return RedirectToAction(nameof(Index));
        }
    }
}
