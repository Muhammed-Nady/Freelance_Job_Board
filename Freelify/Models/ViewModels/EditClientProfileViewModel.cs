using System.ComponentModel.DataAnnotations;

namespace Freelify.Models.ViewModels
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

        [Display(Name = "Profile Image URL")]
        [Url(ErrorMessage = "Invalid image URL.")]
        public string? ProfileImageUrl { get; set; }

        [Required(ErrorMessage = "Company Name is required.")]
        [Display(Name = "Company Name")]
        public string CompanyName { get; set; }

        [Display(Name = "Company Logo URL")]
        [Url(ErrorMessage = "Invalid logo URL.")]
        public string? CompanyLogoUrl { get; set; }

        [Required(ErrorMessage = "Company Description is required.")]
        [Display(Name = "Company Description")]
        public string CompanyDescription { get; set; }
    }
}
