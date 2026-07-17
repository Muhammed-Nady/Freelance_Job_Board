using System.ComponentModel.DataAnnotations;

namespace Freelify.Models.ViewModels.Application
{
    public class ApplicationCreateViewModel
    {

        public int JobId { get; set; }

        [MaxLength(3000)]
        public string CoverLetter { get; set; } = string.Empty;

        public decimal BidAmount { get; set; }

        public int EstimatedCompletionDays { get; set; }

        //todo
        //Attachments

    }
}
