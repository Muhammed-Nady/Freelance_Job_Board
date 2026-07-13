namespace Freelify.Models.Results
{
    public record LoginResult
    {
        public bool Success { get; set; }
        public string? Message { get; set; }

        public string? Role { get; set; }

    }
}
