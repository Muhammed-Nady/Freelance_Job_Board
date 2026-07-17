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

        //______________edit_______________

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var model = await _jobService.GetJobForEditAsync(id, userId);

            if (model == null)
                return NotFound();

            await _jobService.LoadDropdownsAsync(
                ViewBag,
                model.CategoryId,
                model.SelectedSkillIds);

            return View(model);

        }

        [HttpPost]
        public async Task<IActionResult> Edit(JobEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await _jobService.LoadDropdownsAsync(
                    ViewBag,
                    model.CategoryId,
                    model.SelectedSkillIds);

                return View(model);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            bool success = await _jobService.UpdateJobAsync(model, userId);

            if (!success)
                return NotFound();

            return RedirectToAction(nameof(MyJobs));
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var model = await _jobService.GetJobDetailsAsync(id);

            if (model == null)
                return NotFound();

            return View(model);
        }

    }
    }