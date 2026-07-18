using Freelify.Models.Enums;
using System;

namespace Freelify.Models.ViewModels.Application
{
    public class ApplicationListItemViewModel
    {
        public int Id { get; set; }
        public int JobId { get; set; }
        public string JobTitle { get; set; } = string.Empty;
        public string FreelancerFullName { get; set; } = string.Empty;
        public decimal BidAmount { get; set; }
        public int EstimatedCompletionDays { get; set; }
        public ApplicationStatus Status { get; set; }
        public DateTime SubmittedDate { get; set; }
    }
}
