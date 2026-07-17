using Freelify.Data;
using Freelify.Models.Entities;
using Freelify.Models.Entities.Jobs;
using Freelify.Models.Enums;
using Freelify.Models.ViewModels.Job;
using Microsoft.EntityFrameworkCore;

namespace Freelify.Services
{
    public class JobService
    {
        private readonly AppDbContext _context;

        public JobService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CreateJobAsync(JobCreateViewModel model, string userId)
        {
            // Get client profile
            var client = await _context.ClientProfiles
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (client == null)
                return false;

            // Check category
            bool categoryExists = await _context.Categories
                .AnyAsync(c => c.Id == model.CategoryId);

            if (!categoryExists)
                return false;

            // Business validation
            if (model.Budget <= 0)
                return false;

            if (model.Deadline <= DateTime.UtcNow)
                return false;

            // Create job
            var job = new Job
            {
                Title = model.Title,
                Description = model.Description,
                Budget = model.Budget,
                Deadline = model.Deadline,

                CategoryId = model.CategoryId,
                ClientProfileId = client.Id,

                Status = JobStatus.Open,
                CreatedAt = DateTime.UtcNow
            };



            // Add skills
            foreach (var skillId in model.SelectedSkillIds)
            {
                job.JobSkills.Add(new JobSkill
                {
                    SkillId = skillId
                });
            }

            // TODO: Upload attachments

            _context.Jobs.Add(job);

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<List<Job>> GetClientJobsAsync(string userId)
        {
            return await _context.Jobs
                .Include(j => j.Category)
                .Include(j => j.ClientProfile)
                .Where(j => j.ClientProfile.UserId == userId)
                .OrderByDescending(j => j.CreatedAt)
                .ToListAsync();
        }


        public async Task<bool> DeleteJobAsync(int jobId, string userId)
        {
            var job = await _context.Jobs
                .Include(j => j.ClientProfile)
                .FirstOrDefaultAsync(j => j.Id == jobId);

            if (job == null)
                return false;

            if (job.ClientProfile.UserId != userId) // Check if the user is the owner of the job
                return false;

            _context.Jobs.Remove(job);

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<JobBrowseViewModel> GetAllJobsWithFilters(JobBrowseViewModel model)
        {
          var jobs = await _context.Jobs.Include(j=>j.Category).Include(j=>j.ClientProfile).ToListAsync();

            if(!jobs.Any())
            {
                return model; 

            }
            var jobsVM = new List<JobListItemViewModel>();

            foreach(var job in jobs)
            {
                var jobVM = new JobListItemViewModel()
                {
                    Id = job.Id,
                    Title = job.Title,
                    CategoryName = job.Category.Name,
                    Budget = job.Budget,
                    Deadline = job.Deadline,
                    Status = job.Status,
                    CreatedDate = job.CreatedAt,
                    ClientCompanyName = job.ClientProfile.CompanyName
                };
                jobsVM.Add(jobVM);
            }
            model.Results= jobsVM;
            model.TotalCount= jobsVM.Count;

            return model;







        }


    }
}