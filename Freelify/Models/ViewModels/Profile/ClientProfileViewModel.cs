using System;
using System.ComponentModel.DataAnnotations;

namespace Freelify.Models.ViewModels.Profile
{
    public class ClientProfileViewModel
    {
        public string FullName { get; set; }

        public string Email { get; set; }

        public string? PhoneNumber { get; set; }

        public string? ProfileImageUrl { get; set; }

        public string CompanyName { get; set; }

        public string? CompanyLogoUrl { get; set; }

        public string CompanyDescription { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
