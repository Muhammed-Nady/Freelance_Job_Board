using System.ComponentModel.DataAnnotations;

namespace Freelify.Models.ViewModels
{
    public class EditFreelancerProfileViewModel
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

        //[Required(ErrorMessage = "Bio is required.")]
        [Display(Name = "Bio")]
        public string? Bio { get; set; }

        //[Required(ErrorMessage = "Experience description is required.")]
        [Display(Name = "Experience")]
        public string? Experience { get; set; }
        public List<int> SelectedSkillIds { get; set; } = new();
    }
}
