using Freelify.Data;
using Freelify.Models.Entities.Reviews;
using Freelify.Models.ViewModels.NewFolder;
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

        public async Task<(bool Success, string ErrorMessage)> CreateReviewAsync(ReviewCreateViewModel model,string reviewerId)
        {
            if (model.Rating < 1 || model.Rating > 5)
                return (false, "Rating must be between 1 and 5.");

            var job = await _context.Jobs.FirstOrDefaultAsync(j => j.Id == model.JobId);
            if (job == null)
                return (false, "Job not found.");

            if (reviewerId == model.RevieweeId)
                return (false, "You cannot review yourself.");

            var reviewer = await _context.Users.FindAsync(reviewerId);
            if (reviewer == null)
                return (false, "Reviewer not found.");

            var reviewee = await _context.Users.FindAsync(model.RevieweeId);
            if (reviewee == null)
                return (false, "Reviewee not found.");

            var exists = await _context.Reviews.AnyAsync(r => r.JobId == model.JobId && r.ReviewerId == reviewerId);
            if (exists)
                return (false, "You have already reviewed this job.");

            var review = new Review
            {
                JobId = model.JobId,
                ReviewerId = reviewerId,
                RevieweeId = model.RevieweeId,
                Rating = model.Rating,
                Comment = model.Comment
            };

            _context.Reviews.Add(review);

            // Update reviewee profile rating (could be freelancer or client)
            var freelancerProfile = await _context.FreelancerProfiles.FirstOrDefaultAsync(f => f.UserId == model.RevieweeId);
            if (freelancerProfile != null)
            {
                var oldCount = freelancerProfile.ReviewCount;
                var oldAvg = freelancerProfile.AverageRating;
                var newCount = oldCount + 1;
                var newAvg = ((oldAvg * oldCount) + model.Rating) / newCount;
                freelancerProfile.ReviewCount = newCount;
                freelancerProfile.AverageRating = Math.Round(newAvg, 2);
            }
            else
            {
                var clientProfile = await _context.ClientProfiles.FirstOrDefaultAsync(c => c.UserId == model.RevieweeId);
                if (clientProfile != null)
                {
                    var oldCount = clientProfile.ReviewCount;
                    var oldAvg = clientProfile.AverageRating;
                    var newCount = oldCount + 1;
                    var newAvg = ((oldAvg * oldCount) + model.Rating) / newCount;
                    clientProfile.ReviewCount = newCount;
                    clientProfile.AverageRating = Math.Round(newAvg, 2);
                }
            }

            await _context.SaveChangesAsync();

            return (true, string.Empty);
        }

        public async Task<List<Review>> GetReviewsForUserAsync(string userId)
        {
            return await _context.Reviews
                .Include(r => r.Reviewer)
                .Include(r => r.Reviewee)
                .Where(r => r.RevieweeId == userId)
                .OrderByDescending(r => r.Id)
                .ToListAsync();
        }

      
    }
}
