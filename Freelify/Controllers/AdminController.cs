using Freelify.Services;
using Microsoft.AspNetCore.Mvc;

namespace Freelify.Controllers
{
    public class AdminController : Controller
    {
        private readonly AdminService _adminService;

        public AdminController(AdminService adminService)
        {
            _adminService = adminService;
        }

        public async Task<IActionResult> Users()
        {
            var users = await _adminService.GetUsersAsync();

            return View(users);
        }

        [HttpPost]
        public async Task<IActionResult> ToggleStatus(string id)
        {
            await _adminService.ToggleUserStatusAsync(id);

            return RedirectToAction(nameof(Users));
        }


        public async Task<IActionResult> Jobs()
        {
            var jobs = await _adminService.GetAllJobsAsync();

            return View(jobs);
        }

        public async Task<IActionResult> Dashboard()
        {
            var model = await _adminService.GetDashboardAsync();

            return View(model);
        }

    }
}
