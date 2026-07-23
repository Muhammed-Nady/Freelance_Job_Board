using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Freelify.Models.ViewModels.Application
{
    public class ApplicationCreateViewModel
    {
        public int JobId { get; set; }

        [Required(ErrorMessage = "Cover letter is required.")]
        [StringLength(3000, MinimumLength = 20, ErrorMessage = "Cover letter must be between 20 and 3000 characters.")]
        public string CoverLetter { get; set; } = string.Empty;

        [Range(1, 1000000, ErrorMessage = "Bid amount must be greater than 0 and less than 1,000,000.")]
        public decimal BidAmount { get; set; }

        [Range(1, 365, ErrorMessage = "Estimated completion days must be between 1 and 365.")]
        public int EstimatedCompletionDays { get; set; }

        public List<IFormFile> ? Attachments { get; set; } 
    }
}
