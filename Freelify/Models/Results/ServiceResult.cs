namespace Freelify.Models.Results
{
    public record ServiceResult
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
    }
}
