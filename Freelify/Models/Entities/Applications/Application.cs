using Freelify.Models.Entities.Users;
using Freelify.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace Freelify.Models.Entities
{
    public class Application
    {
        public int Id { get; set; }

        public int JobId { get; set; }

        public Job Job { get; set; } = null!;

        public int FreelancerProfileId { get; set; }

        public FreelancerProfile FreelancerProfile { get; set; } = null!;

        [MaxLength(3000)]
        public string CoverLetter { get; set; } = string.Empty;

        public decimal BidAmount { get; set; }

        public int EstimatedCompletionDays { get; set; }

        public ApplicationStatus Status { get; set; }

        public DateTime SubmittedDate { get; set; } = DateTime.UtcNow;

        public ICollection<ApplicationAttachment> Attachments { get; set; } = new List<ApplicationAttachment>();
    }
}