using Freelify.Models.Enums;
using System;
using System.Collections.Generic;

namespace Freelify.Models.ViewModels.Application
{
    public class ApplicationDetailsViewModel
    {
        public int Id { get; set; }
        public int JobId { get; set; }
        public string JobTitle { get; set; } = string.Empty;
        public int FreelancerProfileId { get; set; }
        public string FreelancerFullName { get; set; } = string.Empty;
        public string FreelancerEmail { get; set; } = string.Empty;
        public string FreelancerPhoneNumber { get; set; } = string.Empty;
        public string FreelancerBio { get; set; } = string.Empty;
        public string FreelancerExperience { get; set; } = string.Empty;
        public string CoverLetter { get; set; } = string.Empty;
        public decimal BidAmount { get; set; }
        public int EstimatedCompletionDays { get; set; }
        public ApplicationStatus Status { get; set; }
        public DateTime SubmittedDate { get; set; }
        public List<AttachmentInfo> Attachments { get; set; } = new();
    }

    public class AttachmentInfo
    {
        public int Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FileUrl { get; set; } = string.Empty;
    }
}
