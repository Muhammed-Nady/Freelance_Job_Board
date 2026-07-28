using Freelify.Models.ViewModels.Review;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Freelify.Models.ViewModels.Profile
{
    public class FreelancerProfileViewModel
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? ProfileImageUrl { get; set; }
        public string Bio { get; set; } = string.Empty;
        public string Experience { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public double AverageRating { get; set; }
        public int ReviewCount { get; set; }
        public List<ReviewItemViewModel> Reviews { get; set; } = new();

        [Required(ErrorMessage = "Please select at least one skill.")]
        public List<string> Skills { get; set; } = new();
    }
}
