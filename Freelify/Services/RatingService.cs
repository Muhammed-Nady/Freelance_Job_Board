using Freelify.Data;
using Freelify.Models.Entities.Reviews;
using Freelify.Models.Enums;
using Freelify.Models.ViewModels.Review;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Freelify.Services
{
    public class RatingService
    {
        private readonly AppDbContext _context;

        public RatingService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<(bool CanReview, string? RevieweeId, string? RevieweeName, string? JobTitle, string? ErrorMessage)> CanLeaveReviewAsync(int jobId, string reviewerId)
        {
            var job = await _context.Jobs
                .Include(j => j.ClientProfile)
                .FirstOrDefaultAsync(j => j.Id == jobId);

            if (job == null)
            {
                return (false, null, null, null, "Job not found.");
            }

            if (job.Status != JobStatus.Completed)
            {
                return (false, null, null, null, "Only completed jobs can be reviewed.");
            }

            var acceptedApp = await _context.Applications
                .Include(a => a.FreelancerProfile)
                    .ThenInclude(f => f.User)
                .FirstOrDefaultAsync(a => a.JobId == jobId && a.Status == ApplicationStatus.Accepted);

            if (acceptedApp == null)
            {
                return (false, null, null, null, "No accepted freelancer found for this job.");
            }

            string revieweeId;
            string revieweeName;

            if (job.ClientProfile.UserId == reviewerId)
            {
                revieweeId = acceptedApp.FreelancerProfile.UserId;
                revieweeName = acceptedApp.FreelancerProfile.User.FullName;
            }
            else if (acceptedApp.FreelancerProfile.UserId == reviewerId)
            {
                revieweeId = job.ClientProfile.UserId;
                var clientUser = await _context.Users.FindAsync(job.ClientProfile.UserId);
                revieweeName = clientUser?.FullName ?? "Client";
            }
            else
            {
                return (false, null, null, null, "You were not a participant in this job.");
            }

            var alreadyReviewed = await _context.Reviews
                .AnyAsync(r => r.JobId == jobId && r.ReviewerId == reviewerId);

            if (alreadyReviewed)
            {
                return (false, revieweeId, revieweeName, job.Title, "You have already left a review for this job.");
            }

            return (true, revieweeId, revieweeName, job.Title, string.Empty);
        }

        public async Task<bool> HasReviewedJobAsync(int jobId, string reviewerId)
        {
            return await _context.Reviews
                .AnyAsync(r => r.JobId == jobId && r.ReviewerId == reviewerId);
        }

        public async Task<(bool Success, string ErrorMessage)> CreateReviewAsync(ReviewCreateViewModel model, string reviewerId)
        {
            if (model.Rating < 1 || model.Rating > 5)
            {
                return (false, "Rating must be between 1 and 5.");
            }

            var eligibility = await CanLeaveReviewAsync(model.JobId, reviewerId);
            if (!eligibility.CanReview)
            {
                return (false, eligibility.ErrorMessage ?? "You are not eligible to review this job.");
            }

            if (reviewerId == model.RevieweeId)
            {
                return (false, "You cannot review yourself.");
            }

            var review = new Review
            {
                JobId = model.JobId,
                ReviewerId = reviewerId,
                RevieweeId = model.RevieweeId,
                Rating = model.Rating,
                Comment = model.Comment?.Trim()
            };

            _context.Reviews.Add(review);

            var existingReviews = await _context.Reviews
                .Where(r => r.RevieweeId == model.RevieweeId)
                .ToListAsync();

            existingReviews.Add(review);

            var freelancerProfile = await _context.FreelancerProfiles
                .FirstOrDefaultAsync(f => f.UserId == model.RevieweeId);

            if (freelancerProfile != null)
            {
                freelancerProfile.ReviewCount = existingReviews.Count;
                freelancerProfile.AverageRating = Math.Round((decimal)existingReviews.Average(r => r.Rating), 2);
            }
            else
            {
                var clientProfile = await _context.ClientProfiles
                    .FirstOrDefaultAsync(c => c.UserId == model.RevieweeId);

                if (clientProfile != null)
                {
                    clientProfile.ReviewCount = existingReviews.Count;
                    clientProfile.AverageRating = Math.Round((decimal)existingReviews.Average(r => r.Rating), 2);
                }
            }

            await _context.SaveChangesAsync();
            return (true, string.Empty);
        }

        public async Task<List<ReviewItemViewModel>> GetReviewsForUserAsync(string userId)
        {
            return await _context.Reviews
                .Include(r => r.Reviewer)
                .Include(r => r.Job)
                .Where(r => r.RevieweeId == userId)
                .OrderByDescending(r => r.Id)
                .Select(r => new ReviewItemViewModel
                {
                    Id = r.Id,
                    JobId = r.JobId,
                    JobTitle = r.Job.Title,
                    ReviewerName = r.Reviewer.FullName,
                    ReviewerImageUrl = r.Reviewer.ProfileImageUrl,
                    Rating = r.Rating,
                    Comment = r.Comment
                })
                .ToListAsync();
        }
    }
}
