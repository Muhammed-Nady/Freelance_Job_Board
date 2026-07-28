using Freelify.Models.ViewModels.Review;
using System;
using System.Collections.Generic;

namespace Freelify.Models.ViewModels.Profile
{
    public class ClientProfileViewModel
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? ProfileImageUrl { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string? CompanyLogoUrl { get; set; }
        public string CompanyDescription { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public double AverageRating { get; set; }
        public int ReviewCount { get; set; }
        public List<ReviewItemViewModel> Reviews { get; set; } = new();
    }
}
