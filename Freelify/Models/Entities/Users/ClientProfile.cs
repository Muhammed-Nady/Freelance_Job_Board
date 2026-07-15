using System.ComponentModel.DataAnnotations;

namespace Freelify.Models.Entities.Users
{
    public class ClientProfile 
    {

        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;

        public string CompanyName { get; set; }

        public string? CompanyLogoUrl { get; set; }

        public string? CompanyDescription { get; set; }


        public decimal AverageRating { get; set; }

        public int ReviewCount { get; set; }

        public ICollection<Job> Jobs { get; set; } = new List<Job>();
    }
}
