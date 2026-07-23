using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Freelify.Models.Entities.Users
{
    public class ApplicationUser : IdentityUser
    {
        [MinLength(2)]
        public string FullName { get; set; }

        public string? ProfileImageUrl { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = true; 



        // Nav Properties






        // =============
    }
}
