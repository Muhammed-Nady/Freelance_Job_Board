using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Freelify.Models.ViewModels.Profile
{
    public class EditClientProfileViewModel
    {
        [Required(ErrorMessage = "Full Name is required.")]
        [MinLength(2, ErrorMessage = "Full Name must be at least 2 characters.")]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Phone(ErrorMessage = "Invalid phone number.")]
        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "Profile Image")]
        public IFormFile? ProfileImage { get; set; }

        public string? ExistingProfileImageUrl { get; set; }

        [Display(Name = "Company Name")]
        public string? CompanyName { get; set; }

        [Display(Name = "Company Logo")]
        public IFormFile? CompanyLogo { get; set; }

        public string? ExistingCompanyLogoUrl { get; set; }

        //[Required(ErrorMessage = "Company Description is required.")]
        [Display(Name = "Company Description")]
        public string? CompanyDescription { get; set; }
    }
}
