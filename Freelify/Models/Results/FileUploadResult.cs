using Freelify.Models.Enums;

namespace Freelify.Models.Results
{
    public class FileUploadResult
    {
        public bool Success { get; set; }
        public string? Url { get; set; }
        public string? ErrorMessage { get; set; }
        public ErrorType? ErrorType { get; set; }
    }
}
