using Freelify.Services;
using Freelify.Models.ViewModels.Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using System.IO;
using Freelify.Data;
using Microsoft.EntityFrameworkCore;

namespace Freelify.Controllers
{
    [Authorize]
    public class ApplicationController : Controller
    {
        private readonly JobApplicationService _applicationService;
        private readonly AppDbContext _context;
        //private readonly IWebHostEnvironment _env;

        public ApplicationController(JobApplicationService applicationService, AppDbContext context)
        {
            _applicationService = applicationService;
            _context = context;
            //_env = env;
        }

        [HttpGet]
        [Authorize(Roles = "Freelancer")]
        public async Task<IActionResult> Create(int jobId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var validation = await _applicationService.CanApplyAsync(jobId, userId);

            if (!validation.Success)
            {
                TempData["ErrorMessage"] = validation.ErrorMessage;
                return RedirectToAction("Details", "Job", new { id = jobId });
            }

            var model = new ApplicationCreateViewModel
            {
                JobId = jobId
            };

            var job = await _context.Jobs.FindAsync(jobId);
            ViewBag.JobTitle = job?.Title ?? string.Empty;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Freelancer")]
        public async Task<IActionResult> Create(ApplicationCreateViewModel model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!ModelState.IsValid)
            {
                var job = await _context.Jobs.FindAsync(model.JobId);
                ViewBag.JobTitle = job?.Title ?? string.Empty;
                return View(model);
            }

            var result = await _applicationService.SubmitApplicationAsync(model, userId);

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage);
                var job = await _context.Jobs.FindAsync(model.JobId);
                ViewBag.JobTitle = job?.Title ?? string.Empty;
                return View(model);
            }

            TempData["SuccessMessage"] = "Proposal submitted successfully!";
            return RedirectToAction(nameof(MyApplications));
        }

        [HttpGet]
        [Authorize(Roles = "Freelancer")]
        public async Task<IActionResult> MyApplications()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var applications = await _applicationService.GetFreelancerApplicationsAsync(userId);
            return View(applications);
        }

        [HttpGet]
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> ForJob(int jobId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var proposals = await _applicationService.GetJobProposalsAsync(jobId, userId);

            var job = await _context.Jobs.FindAsync(jobId);
            ViewBag.JobTitle = job?.Title ?? string.Empty;
            ViewBag.JobId = jobId;

            return View(proposals);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var model = await _applicationService.GetApplicationDetailsAsync(id, userId);

            if (model == null)
            {
                return Forbid();
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> Accept(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var application = await _context.Applications.FindAsync(id);

            if (application == null)
            {
                return NotFound();
            }

            var success = await _applicationService.AcceptApplicationAsync(id, userId);

            if (!success)
            {
                TempData["ErrorMessage"] = "Failed to accept the proposal.";
                return RedirectToAction(nameof(Details), new { id });
            }

            TempData["SuccessMessage"] = "Proposal accepted successfully! The job is now in progress.";
            return RedirectToAction(nameof(ForJob), new { jobId = application.JobId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> Reject(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var application = await _context.Applications.FindAsync(id);

            if (application == null)
            {
                return NotFound();
            }

            var success = await _applicationService.RejectApplicationAsync(id, userId);

            if (!success)
            {
                TempData["ErrorMessage"] = "Failed to reject the proposal.";
                return RedirectToAction(nameof(Details), new { id });
            }

            TempData["SuccessMessage"] = "Proposal rejected successfully.";
            return RedirectToAction(nameof(ForJob), new { jobId = application.JobId });
        }

        [HttpGet]
        public async Task<IActionResult> DownloadAttachment(int id)
        {
            var attachment = await _context.ApplicationAttachments
                .Include(a => a.Application)
                    .ThenInclude(app => app.Job)
                        .ThenInclude(j => j.ClientProfile)
                .Include(a => a.Application)
                    .ThenInclude(app => app.FreelancerProfile)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (attachment == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var isClient = attachment.Application.Job.ClientProfile.UserId == userId;
            var isFreelancer = attachment.Application.FreelancerProfile.UserId == userId;

            if (!isClient && !isFreelancer)
            {
                return Forbid();
            }

            return Redirect(attachment.FileUrl);
        }
    }
    }
