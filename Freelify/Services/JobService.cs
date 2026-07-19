using Freelify.Data;
using Freelify.Models.Entities;
using Freelify.Models.Entities.Jobs;
using Freelify.Models.Enums;
using Freelify.Models.ViewModels;
using Freelify.Models.ViewModels.Job;
using Microsoft.AspNetCore.Mvc.Rendering;
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


        public async Task<(bool Success, string ErrorMessage)> DeleteJobAsync(int jobId, string userId)
        {
            var job = await _context.Jobs
                .Include(j => j.ClientProfile)
                .Include(j=>j.Applications)
                .FirstOrDefaultAsync(j => j.Id == jobId);

            //delete
            if (job.Status == JobStatus.Open && !job.Applications.Any())
            {

                if (job == null)
                    return (false, "Not Found");

                if (job.ClientProfile.UserId != userId) // Check if the user is the owner of the job
                    return (false, "Not your job");



                _context.Jobs.Remove(job);

                await _context.SaveChangesAsync();

                return (true, "");
            }
            else
            {
                job.Status= JobStatus.Cancelled;
                await _context.SaveChangesAsync();
                return (true, "");

            }
        }
        public async Task<(bool Success, string ErrorMessage,JobEditViewModel? jobEditVM)> GetJobForEditAsync(int jobId, string userId)
        {
            var job = await _context.Jobs
                .Include(j => j.ClientProfile)
                .Include(j => j.JobSkills)
                .FirstOrDefaultAsync(j => j.Id == jobId);

           


            if (job == null)
                return (false,"Not Found",null);

        if (job.ClientProfile.UserId != userId)
                return (false, "not your job", null);
            if(job.Status!= JobStatus.Open)
            {
                return (false, "You can't edit not open jobs", null);
            }

            
            return (true, "",  new JobEditViewModel
            {
                Id = job.Id,
                Title = job.Title,
                Description = job.Description,
                Budget = job.Budget,
                Deadline = job.Deadline,
                CategoryId = job.CategoryId,
                SelectedSkillIds = job.JobSkills
                                      .Select(js => js.SkillId)
                                      .ToList()
            });
        }

        public async Task<bool> UpdateJobAsync(JobEditViewModel model, string userId)
        {
            var job = await _context.Jobs
                .Include(j => j.ClientProfile)
                .Include(j => j.JobSkills)
                .FirstOrDefaultAsync(j => j.Id == model.Id);

            if (job == null)
                return false;

            if (job.ClientProfile.UserId != userId)
                return false;

            job.Title = model.Title;
            job.Description = model.Description;
            job.Budget = model.Budget;
            job.Deadline = model.Deadline;
            job.CategoryId = model.CategoryId;
            _context.JobSkills.RemoveRange(job.JobSkills);

            foreach (var skillId in model.SelectedSkillIds)
            {
                job.JobSkills.Add(new JobSkill
                {
                    SkillId = skillId,
                    JobId = job.Id
                });
            }
            await _context.SaveChangesAsync();

            return true;
        }

        //______________________Details____________________________
        public async Task<JobDetailsViewModel?> GetJobDetailsAsync(int jobId)
        {
            var job = await _context.Jobs
                .Include(j => j.Category)
                .Include(j => j.ClientProfile)
                .Include(j => j.JobSkills)
                    .ThenInclude(js => js.Skill)
                .Include(j => j.Attachments)
                .FirstOrDefaultAsync(j => j.Id == jobId);

            if (job == null)
                return null;

            return new JobDetailsViewModel
            {
                Id = job.Id,
                Title = job.Title,
                Description = job.Description,
                Budget = job.Budget,
                Deadline = job.Deadline,
                CreatedDate = job.CreatedAt,
                Status = job.Status.ToString(),
                ClientName = job.ClientProfile.CompanyName,
                ClientUserId = job.ClientProfile.UserId,
                CategoryName = job.Category.Name,

                Skills = job.JobSkills
                    .Select(js => js.Skill.Name)
                    .ToList(),

                AttachmentPaths = job.Attachments
                    .Select(a => a.FilePath)
                    .ToList()
            };
        }

        //--------------------Search------------------------------
        private async Task LoadFilters(JobBrowseViewModel model)
        {
            model.Categories = new SelectList(
                await _context.Categories.ToListAsync(),
                "Id",
                "Name");

            model.Skills = new MultiSelectList(
                await _context.Skills.ToListAsync(),
                "Id",
                "Name");
        }

        public async Task<JobBrowseViewModel> GetAllJobsWithFilters(JobBrowseViewModel model)
        {
            await LoadFilters(model);
            var query =  _context.Jobs.Include(j=>j.Category).Include(j=>j.ClientProfile).Include(j=>j.JobSkills).AsQueryable();

            if(!query.Any())
            {
                return model;

            }

            //TODO
            //SearchTerm

            if(model.CategoryId != null)
            {
                 query = query.Where(j => j.CategoryId == model.CategoryId);
            }

            //under test
            if (model.SkillIds.Any())
            {
                query = query.Where(j => j.JobSkills.Any(js => model.SkillIds.Contains(js.SkillId)));
               
            }

            if(model.MaxBudget!=null)
            {
                query = query.Where(j => j.Budget<=model.MaxBudget);

            }
            if (model.MinBudget != null)
            {
                query = query.Where(j => j.Budget >= model.MinBudget);

            }
            if(model.SortBy!=null)
            {
                switch (model.SortBy)
                {
                    case JobSortBy.Newest:
                        query = query.OrderByDescending(j => j.CreatedAt);
                        break;

                    case JobSortBy.Oldest:
                        query = query.OrderBy(j => j.CreatedAt);
                        break;

                    case JobSortBy.HighestBudget:
                        query = query.OrderByDescending(j => j.Budget);
                        break;

                    case JobSortBy.LowestBudget:
                        query = query.OrderBy(j => j.Budget);
                        break;
                }
            }
            
            model.TotalCount= query.Count();
            query = query.Skip((model.Page-1)*model.PageSize).Take(model.PageSize);

            var jobs = await query.ToListAsync();

            if (!jobs.Any())
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

            return model;
        }

        public async Task LoadDropdownsAsync(dynamic viewBag, int? categoryId = null, List<int>? selectedSkills = null)
        {
            viewBag.Categories = new SelectList(
                await _context.Categories.ToListAsync(),
                "Id",
                "Name",
                categoryId);

            viewBag.Skills = new MultiSelectList(
                await _context.Skills.ToListAsync(),
                "Id",
                "Name",
                selectedSkills);
        }


    }
}