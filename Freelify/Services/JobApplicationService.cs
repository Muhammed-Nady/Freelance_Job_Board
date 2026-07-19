using Freelify.Data;
using Freelify.Models.Entities;
using Freelify.Models.Entities.Users;
using Freelify.Models.Enums;
using Freelify.Models.ViewModels.Application;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Freelify.Services
{
    public class JobApplicationService
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public JobApplicationService(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<(bool Success, string ErrorMessage)> CanApplyAsync(int jobId, string userId)
        {
            var freelancer = await _context.FreelancerProfiles
                .FirstOrDefaultAsync(f => f.UserId == userId);

            if (freelancer == null)
            {
                return (false, "You must be a Freelancer to apply for jobs.");
            }

            var job = await _context.Jobs
                .Include(j => j.ClientProfile)
                .FirstOrDefaultAsync(j => j.Id == jobId);

            if (job == null)
            {
                return (false, "Job not found.");
            }

            if (job.Status != JobStatus.Open)
            {
                return (false, "This job is no longer open for applications.");
            }

            if (job.ClientProfile.UserId == userId)
            {
                return (false, "You cannot apply to your own job.");
            }

            var alreadyApplied = await _context.Applications
                .AnyAsync(a => a.JobId == jobId && a.FreelancerProfileId == freelancer.Id);

            if (alreadyApplied)
            {
                return (false, "You have already submitted a proposal for this job.");
            }

            return (true, string.Empty);
        }

        public async Task<(bool Success, string ErrorMessage)> SubmitApplicationAsync(ApplicationCreateViewModel model, string userId)
        {
            var validation = await CanApplyAsync(model.JobId, userId);
            if (!validation.Success)
            {
                return (false, validation.ErrorMessage);
            }

            var freelancer = await _context.FreelancerProfiles
                .FirstAsync(f => f.UserId == userId);

            var application = new Application
            {
                JobId = model.JobId,
                FreelancerProfileId = freelancer.Id,
                CoverLetter = model.CoverLetter,
                BidAmount = model.BidAmount,
                EstimatedCompletionDays = model.EstimatedCompletionDays,
                Status = ApplicationStatus.Submitted,
                SubmittedDate = DateTime.UtcNow
            };

            if (model.Attachments != null && model.Attachments.Any())
            {
                var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".jpg", ".jpeg", ".png" };
                var uploadsDir = Path.Combine(_env.ContentRootPath, "Uploads", "ApplicationAttachments");

                if (!Directory.Exists(uploadsDir))
                {
                    Directory.CreateDirectory(uploadsDir);
                }

                foreach (var file in model.Attachments)
                {
                    if (file.Length > 10 * 1024 * 1024)
                    {
                        return (false, $"File {file.FileName} exceeds the maximum size limit of 10MB.");
                    }

                    var ext = Path.GetExtension(file.FileName).ToLower();
                    if (!allowedExtensions.Contains(ext))
                    {
                        return (false, $"File {file.FileName} has an invalid extension.");
                    }

                    var uniqueName = $"{Guid.NewGuid()}{ext}";
                    var filePath = Path.Combine(uploadsDir, uniqueName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    application.Attachments.Add(new ApplicationAttachment
                    {
                        FileName = file.FileName,
                        FileUrl = $"Uploads/ApplicationAttachments/{uniqueName}",
                        UploadedDate = DateTime.UtcNow
                    });
                }
            }

            _context.Applications.Add(application);
            await _context.SaveChangesAsync();

            return (true, string.Empty);
        }

        public async Task<List<ApplicationListItemViewModel>> GetFreelancerApplicationsAsync(string userId)
        {
            var freelancer = await _context.FreelancerProfiles
                .Include(f => f.User)
                .FirstOrDefaultAsync(f => f.UserId == userId);

            if (freelancer == null)
            {
                return new List<ApplicationListItemViewModel>();
            }

            return await _context.Applications
                .Include(a => a.Job)
                .Where(a => a.FreelancerProfileId == freelancer.Id)
                .OrderByDescending(a => a.SubmittedDate)
                .Select(a => new ApplicationListItemViewModel
                {
                    Id = a.Id,
                    JobId = a.JobId,
                    JobTitle = a.Job.Title,
                    FreelancerFullName = freelancer.User.FullName,
                    BidAmount = a.BidAmount,
                    EstimatedCompletionDays = a.EstimatedCompletionDays,
                    Status = a.Status,
                    SubmittedDate = a.SubmittedDate
                })
                .ToListAsync();
        }

        public async Task<List<ApplicationListItemViewModel>> GetJobProposalsAsync(int jobId, string userId)
        {
            var job = await _context.Jobs
                .Include(j => j.ClientProfile)
                .FirstOrDefaultAsync(j => j.Id == jobId);

            if (job == null || job.ClientProfile.UserId != userId)
            {
                return new List<ApplicationListItemViewModel>();
            }

            return await _context.Applications
                .Include(a => a.FreelancerProfile)
                    .ThenInclude(f => f.User)
                .Where(a => a.JobId == jobId)
                .OrderByDescending(a => a.SubmittedDate)
                .Select(a => new ApplicationListItemViewModel
                {
                    Id = a.Id,
                    JobId = a.JobId,
                    JobTitle = job.Title,
                    FreelancerFullName = a.FreelancerProfile.User.FullName,
                    BidAmount = a.BidAmount,
                    EstimatedCompletionDays = a.EstimatedCompletionDays,
                    Status = a.Status,
                    SubmittedDate = a.SubmittedDate
                })
                .ToListAsync();
        }

        public async Task<ApplicationDetailsViewModel?> GetApplicationDetailsAsync(int applicationId, string userId)
        {
            var application = await _context.Applications
                .Include(a => a.Attachments)
                .Include(a => a.Job)
                    .ThenInclude(j => j.ClientProfile)
                .Include(a => a.FreelancerProfile)
                    .ThenInclude(f => f.User)
                .FirstOrDefaultAsync(a => a.Id == applicationId);

            if (application == null)
            {
                return null;
            }

            var isClient = application.Job.ClientProfile.UserId == userId;
            var isFreelancer = application.FreelancerProfile.UserId == userId;

            if (!isClient && !isFreelancer)
            {
                return null;
            }

            if (isClient && application.Status == ApplicationStatus.Submitted)
            {
                application.Status = ApplicationStatus.UnderReview;
                await _context.SaveChangesAsync();
            }

            return new ApplicationDetailsViewModel
            {
                Id = application.Id,
                JobId = application.JobId,
                JobTitle = application.Job.Title,
                FreelancerProfileId = application.FreelancerProfileId,
                FreelancerFullName = application.FreelancerProfile.User.FullName,
                FreelancerEmail = application.FreelancerProfile.User.Email ?? string.Empty,
                FreelancerPhoneNumber = application.FreelancerProfile.User.PhoneNumber ?? string.Empty,
                FreelancerBio = application.FreelancerProfile.Bio ?? string.Empty,
                FreelancerExperience = application.FreelancerProfile.Experience ?? string.Empty,
                CoverLetter = application.CoverLetter,
                BidAmount = application.BidAmount,
                EstimatedCompletionDays = application.EstimatedCompletionDays,
                Status = application.Status,
                SubmittedDate = application.SubmittedDate,
                Attachments = application.Attachments.Select(att => new AttachmentInfo
                {
                    Id = att.Id,
                    FileName = att.FileName,
                    FileUrl = att.FileUrl
                }).ToList()
            };
        }

        public async Task<bool> AcceptApplicationAsync(int applicationId, string userId)
        {
            var application = await _context.Applications
                .Include(a => a.Job)
                    .ThenInclude(j => j.ClientProfile)
                .FirstOrDefaultAsync(a => a.Id == applicationId);

            if (application == null)
            {
                return false;
            }

            var job = application.Job;
            if (job.ClientProfile.UserId != userId || job.Status != JobStatus.Open)
            {
                return false;
            }

            if (application.Status != ApplicationStatus.Submitted && application.Status != ApplicationStatus.UnderReview)
            {
                return false;
            }

            application.Status = ApplicationStatus.Accepted;
            job.Status = JobStatus.InProgress;

            var otherApplications = await _context.Applications
                .Where(a => a.JobId == job.Id && a.Id != applicationId)
                .ToListAsync();

            foreach (var other in otherApplications)
            {
                other.Status = ApplicationStatus.Rejected;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RejectApplicationAsync(int applicationId, string userId)
        {
            var application = await _context.Applications
                .Include(a => a.Job)
                    .ThenInclude(j => j.ClientProfile)
                .FirstOrDefaultAsync(a => a.Id == applicationId);

            if (application == null)
            {
                return false;
            }

            var job = application.Job;
            if (job.ClientProfile.UserId != userId || job.Status != JobStatus.Open)
            {
                return false;
            }

            if (application.Status != ApplicationStatus.Submitted && application.Status != ApplicationStatus.UnderReview)
            {
                return false;
            }

            application.Status = ApplicationStatus.Rejected;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
