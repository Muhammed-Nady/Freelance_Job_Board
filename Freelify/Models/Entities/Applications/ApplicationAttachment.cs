using System.ComponentModel.DataAnnotations;

namespace Freelify.Models.Entities
{
    public class ApplicationAttachment
    {
        public int Id { get; set; }

        public int ApplicationId { get; set; }

        public Application Application { get; set; } = null!;

        [MaxLength(255)]
        public string FileName { get; set; } = string.Empty;

        public string FileUrl { get; set; } = string.Empty;

        public DateTime UploadedDate { get; set; } = DateTime.UtcNow;
    }
}