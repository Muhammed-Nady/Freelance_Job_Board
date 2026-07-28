namespace Freelify.Models.ViewModels.Review
{
    public class ReviewItemViewModel
    {
        public int Id { get; set; }
        public int JobId { get; set; }
        public string JobTitle { get; set; } = string.Empty;
        public string ReviewerName { get; set; } = string.Empty;
        public string? ReviewerImageUrl { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
    }
}
