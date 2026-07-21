using System;
using System.ComponentModel.DataAnnotations;

namespace Freelify.Models.ViewModels.Profile
{
    public class FreelancerProfileViewModel
    {
        public string FullName { get; set; }

        public string Email { get; set; }

        public string? PhoneNumber { get; set; }

        public string? ProfileImageUrl { get; set; }

        public string Bio { get; set; }

        public string Experience { get; set; }

        public DateTime CreatedDate { get; set; }


        [Required(ErrorMessage = "Please select at least one skill.")]

        public List<string> Skills { get; set; } = new();
    }
}
