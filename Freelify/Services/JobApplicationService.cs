using Freelify.Data;
using Freelify.Models.Entities;
using Freelify.Models.Enums;
using Freelify.Models.ViewModels.Application;
using Microsoft.EntityFrameworkCore;

namespace Freelify.Services
{
    public class JobApplicationService
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly NotificationService _notificationService;
        private readonly FileUploadService _fileUploadService;
      
        public JobApplicationService(AppDbContext context, IWebHostEnvironment env, NotificationService notificationService, FileUploadService fileUploadService)
        {
            _context = context;
            _env = env;
            _notificationService = notificationService;
            _fileUploadService = fileUploadService;
        }

        public async Task<(bool Success, string ErrorMessage)> CanApplyAsync(int jobId, string userId)
        {
            var freelancer = await _context.FreelancerProfiles
                .FirstOrDefaultAsync(f => f.UserId == userId);



            if (freelancer == null)
            {
                return (false, "You must be a Freelancer to apply for jobs.");
            }

            // u should have skills to apply
            bool hasSkills = await _context.FreelancerSkills
            .AnyAsync(f => f.FreelancerProfileId == freelancer.Id);

            if (!hasSkills)
            { return (false, "Please complete your profile by adding at least one skill before applying."); }

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

            bool alreadyApplied = await _context.Applications.AnyAsync(a => a.JobId == jobId && a.FreelancerProfileId == freelancer.Id);

            if (alreadyApplied)
            {
                return (false, "You have already applied for this job.");
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

            //if (model.Attachments != null && model.Attachments.Any())
            //{
            //    var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".jpg", ".jpeg", ".png" };
            //    var uploadsDir = Path.Combine(_env.ContentRootPath, "Uploads", "ApplicationAttachments");

            //    if (!Directory.Exists(uploadsDir))
            //    {
            //        Directory.CreateDirectory(uploadsDir);
            //    }

            //    foreach (var file in model.Attachments)
            //    {
            //        if (file.Length > 10 * 1024 * 1024)
            //        {
            //            return (false, $"File {file.FileName} exceeds the maximum size limit of 10MB.");
            //        }

            //        var ext = Path.GetExtension(file.FileName).ToLower();
            //        if (!allowedExtensions.Contains(ext))
            //        {
            //            return (false, $"File {file.FileName} has an invalid extension.");
            //        }

            //        var uniqueName = $"{Guid.NewGuid()}{ext}";
            //        var filePath = Path.Combine(uploadsDir, uniqueName);

            //        using (var stream = new FileStream(filePath, FileMode.Create))
            //        {
            //            await file.CopyToAsync(stream);
            //        }

            //        application.Attachments.Add(new ApplicationAttachment
            //        {
            //            FileName = file.FileName,
            //            FileUrl = $"Uploads/ApplicationAttachments/{uniqueName}",
            //            UploadedDate = DateTime.UtcNow
            //        });
            //    }
            //}
            if (model.Attachments != null)
            {
                var allowedExtensions = new[]
                { ".pdf",".doc",".docx",".jpg",".jpeg",".png"};

                foreach (var file in model.Attachments)
                {
                    var ext = Path.GetExtension(file.FileName).ToLowerInvariant();

                    if (!allowedExtensions.Contains(ext))
                    {
                        return (false, "Invalid file extension.");
                    }

                    var uploadResult = await _fileUploadService.UploadFile(file);

                    if (!uploadResult.Success)
                    {
                        return (false, uploadResult.ErrorMessage!);
                    }

                    application.Attachments.Add(new ApplicationAttachment
                    {
                        FileName = file.FileName,
                        FileUrl = uploadResult.Url!,
                        UploadedDate = DateTime.UtcNow
                    });
                }
            }

            _context.Applications.Add(application);
            await _context.SaveChangesAsync();

            // send notification to client
            var job = await _context.Jobs
                .Include(j => j.ClientProfile)
                .FirstOrDefaultAsync(j => j.Id == model.JobId);


            var currentApplicationCount = await _context.Applications.CountAsync(a => a.JobId == model.JobId);

            await _notificationService.AddNotification(new Notification()
            {
                UserId = job.ClientProfile.UserId,
                RelatedEntityId = job.Id,
                Type = NotificationType.ApplicationSubmitted,
                Message = $"You have {currentApplicationCount} application{(currentApplicationCount > 0 ? "s" : "")} submitted for your job '{job.Title}'.",
                CreatedDate = DateTime.UtcNow
            });

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

        public async Task<List<ApplicationListItemViewModel>>GetJobProposalsAsync(int jobId,string userId,bool isAdmin = false)
        {
            var job = await _context.Jobs
                .Include(j => j.ClientProfile)
                .FirstOrDefaultAsync(j => j.Id == jobId);

            if(job == null)
{
                return [];
            }

            if (!isAdmin && job.ClientProfile.UserId != userId)
            {
                return [];
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

        public async Task<ApplicationDetailsViewModel?> GetApplicationDetailsAsync(int applicationId, string userId, bool isAdmin = false)
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

            if (!isAdmin && !isClient && !isFreelancer)
            {
                return null;
            }

            if (!isAdmin && isClient && application.Status == ApplicationStatus.Submitted)
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
                .Include(a => a.FreelancerProfile)
                    .ThenInclude(f => f.User)
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
                .Include(a => a.FreelancerProfile)
                .Where(a => a.JobId == job.Id && a.Id != applicationId)
                .ToListAsync();

            foreach (var other in otherApplications)
            {
                other.Status = ApplicationStatus.Rejected;
            }

            await _context.SaveChangesAsync();

            // send notification to freelancer
            await _notificationService.AddNotification(new Notification()
            {
                UserId = application.FreelancerProfile.UserId,
                RelatedEntityId = application.Id,
                Type = NotificationType.ApplicationAccepted,
                Message = $"Your application for the job '{job.Title}' has been accepted.",
                CreatedDate = DateTime.UtcNow
            });

            // auto reject notification other applications
            foreach (var other in otherApplications)
            {
                await _notificationService.AddNotification(new Notification()
                {
                    UserId = other.FreelancerProfile.UserId,
                    RelatedEntityId = other.Id,
                    Type = NotificationType.ApplicationRejected,
                    Message = $"Your application for the job '{job.Title}' has been rejected.",
                    CreatedDate = DateTime.UtcNow
                });
            }

            return true;
        }

        public async Task<bool> RejectApplicationAsync(int applicationId, string userId)
        {
            var application = await _context.Applications
                .Include(a => a.FreelancerProfile)
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

            // send notification to freelancer
            await _notificationService.AddNotification(new Notification()
            {
                UserId = application.FreelancerProfile.UserId,
                RelatedEntityId = application.Id,
                Type = NotificationType.ApplicationRejected,
                Message = $"Your application for the job '{job.Title}' has been rejected.",
                CreatedDate = DateTime.UtcNow
            });

            return true;
        }
    }
}
