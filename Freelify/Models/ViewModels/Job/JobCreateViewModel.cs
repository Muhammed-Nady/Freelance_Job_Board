    using System.ComponentModel.DataAnnotations;

namespace Freelify.Models.ViewModels.Job
{ 
        public class JobCreateViewModel
        {
            [Required]
            [StringLength(100)]
            public string Title { get; set; } = string.Empty;

            [Required]
            [StringLength(1000)]
            public string Description { get; set; } = string.Empty;

            [Range(1, double.MaxValue,
            ErrorMessage = "Budget must be greater than zero.")]
            public decimal Budget { get; set; }

            [Required]
            [DataType(DataType.Date)]
            public DateTime Deadline { get; set; }

            [Required]
            public int CategoryId { get; set; }

        // Skills 
        public List<int> SelectedSkillIds { get; set; } = new();

            public List<IFormFile> Attachments { get; set; } = new();
        }
    }

