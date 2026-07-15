using Freelify.Models.Entities.Users;

namespace Freelify.Models.Entities.Reviews
{
    public class Review
    {
        public int Id { get; set; }

        public int JobId { get; set; }
        public Job Job { get; set; } = null!;

        public string ReviewerId { get; set; } = null!;
        public ApplicationUser Reviewer { get; set; } = null!;

        public string RevieweeId { get; set; } = null!;
        public ApplicationUser Reviewee { get; set; } = null!;

        public int Rating { get; set; }

        public string? Comment { get; set; }
    }
}
