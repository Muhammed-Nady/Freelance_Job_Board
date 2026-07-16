using System.ComponentModel.DataAnnotations;

namespace Freelify.Models.Entities.Users
{
    public class FreelancerProfile 
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } 
        public ApplicationUser User { get; set; } 
        public string? Bio { get; set; }

        public string? Experience { get; set; }

        public decimal AverageRating { get; set; }

        public int ReviewCount { get; set; }

        public ICollection<FreelancerSkill> FreelancerSkills { get; set; } = new List<FreelancerSkill>();

        public ICollection<PortfolioItem> PortfolioItems { get; set; } = new List<PortfolioItem>();

        public ICollection<Application> Applications { get; set; } = new List<Application>();

    }
}
