using Freelify.Data;
using Freelify.Models.ViewModels.Job;
using Freelify.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace Freelify.Controllers
{
    [Authorize(Roles = "Client")]
    public class JobController : Controller
    {
        private readonly JobService _jobService;
        private readonly AppDbContext _context;

        public JobController(JobService jobService, AppDbContext context)
        {
            _jobService = jobService;
            _context = context;
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Categories = new SelectList(
                _context.Categories,
                "Id",
                "Name");

            ViewBag.Skills = new MultiSelectList(
                _context.Skills,
                "Id",
                "Name");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(JobCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = new SelectList(
                    _context.Categories,
                    "Id",
                    "Name");

                ViewBag.Skills = new MultiSelectList(
                    _context.Skills,
                    "Id",
                    "Name");

                return View(model);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            bool created = await _jobService.CreateJobAsync(model, userId);

            if (!created)
            {
                ModelState.AddModelError("", "Unable to create job.");

                ViewBag.Categories = new SelectList(
                    _context.Categories,
                    "Id",
                    "Name");

                ViewBag.Skills = new MultiSelectList(
                    _context.Skills,
                    "Id",
                    "Name");

                return View(model);
            }

            return RedirectToAction(nameof(MyJobs));
        }

        public async Task<IActionResult> MyJobs()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var jobs = await _jobService.GetClientJobsAsync(userId);

            return View(jobs);
        }
        // to do after client profile: return RedirectToAction("Index", "Client");


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            bool deleted = await _jobService.DeleteJobAsync(id, userId);

            if (!deleted)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(MyJobs));
        }


    }
}