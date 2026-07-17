using Freelify.Models.Enums;

namespace Freelify.Models.ViewModels.Job
{
    public class JobListItemViewModel
    {
        public int Id { get; set; }

        public string Title { get; set; } 

        public string CategoryName { get; set; } 

        public decimal Budget { get; set; }

        public DateTime Deadline { get; set; }

        public JobStatus Status { get; set; }

        public DateTime CreatedDate { get; set; }

        public string ClientCompanyName { get; set; }
    }
}
