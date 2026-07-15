using System.ComponentModel.DataAnnotations;

namespace Freelify.Models.Entities.Jobs
{
    public class JobAttachment
    {
        public int Id { get; set; }

        [Required]
        public string FileName { get; set; } = string.Empty;

        [Required]
        public string FilePath { get; set; } = string.Empty;

        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        // FK
        public int JobId { get; set; }

        public Job Job { get; set; } = null!;
    }
}