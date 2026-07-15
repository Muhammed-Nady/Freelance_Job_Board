using Freelify.Models.Entities.Users;
using System.ComponentModel.DataAnnotations;

namespace Freelify.Models.Entities
{
    public class PortfolioItem
    {
        public int Id { get; set; }

        public int FreelancerProfileId { get; set; }

        public FreelancerProfile FreelancerProfile { get; set; } = null!;

        [MaxLength(150)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        public string FileUrl { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}