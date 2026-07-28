using Freelify.Models.ViewModels.Review;
using Freelify.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Freelify.Controllers
{
    [Authorize]
    public class ReviewController : Controller
    {
        private readonly RatingService _ratingService;

        public ReviewController(RatingService ratingService)
        {
            _ratingService = ratingService;
        }

        [HttpGet]
        public async Task<IActionResult> Create(int jobId)
        {
            var reviewerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(reviewerId))
            {
                return Forbid();
            }

            var eligibility = await _ratingService.CanLeaveReviewAsync(jobId, reviewerId);
            if (!eligibility.CanReview)
            {
                TempData["ErrorMessage"] = eligibility.ErrorMessage;
                return RedirectToAction("Details", "Job", new { id = jobId });
            }

            var model = new ReviewCreateViewModel
            {
                JobId = jobId,
                RevieweeId = eligibility.RevieweeId!
            };

            ViewBag.RevieweeName = eligibility.RevieweeName;
            ViewBag.JobTitle = eligibility.JobTitle;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ReviewCreateViewModel model)
        {
            var reviewerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(reviewerId))
            {
                return Forbid();
            }

            if (!ModelState.IsValid)
            {
                var eligibility = await _ratingService.CanLeaveReviewAsync(model.JobId, reviewerId);
                ViewBag.RevieweeName = eligibility.RevieweeName;
                ViewBag.JobTitle = eligibility.JobTitle;
                return View(model);
            }

            var result = await _ratingService.CreateReviewAsync(model, reviewerId);

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage);
                var eligibility = await _ratingService.CanLeaveReviewAsync(model.JobId, reviewerId);
                ViewBag.RevieweeName = eligibility.RevieweeName;
                ViewBag.JobTitle = eligibility.JobTitle;
                return View(model);
            }

            TempData["SuccessMessage"] = "Thank you! Your review has been submitted.";
            return RedirectToAction("Details", "Job", new { id = model.JobId });
        }
    }
}
