using Freelify.Models.Enums;

namespace Freelify.Models.Results
{
    public class ProfileResult
    {
        public bool Success { get; set; }

        public ErrorType ErrorType { get; set; }

        public string ErrorMessage { get; set; }
        public string ViewName { get; set; }
        public object ViewModel { get; set; }
    }
}
