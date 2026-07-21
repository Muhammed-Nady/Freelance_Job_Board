namespace Freelify.Models.ViewModels.NewFolder
{
    public class ReviewCreateViewModel
    {
        public int JobId { get; set; }
        public string RevieweeId { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }

       
    }
}
