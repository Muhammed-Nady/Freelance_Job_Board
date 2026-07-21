using Freelify.Models.ViewModels.NewFolder;
using Freelify.Services;
using Freelify.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ReviewCreateViewModel model)
        {
            var reviewerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(reviewerId))
                return Forbid();

            var result = await _ratingService.CreateReviewAsync(model, reviewerId);

            if (!result.Success)
            {
                TempData["Error"] = result.ErrorMessage;
            }

            return RedirectToAction("Details", "Job", new { id = model.JobId });
        }
    }
}
