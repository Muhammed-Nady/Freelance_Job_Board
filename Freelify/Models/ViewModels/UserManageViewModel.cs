namespace Freelify.Models.ViewModels
{
    public class UserManageViewModel
    {
        public string Id { get; set; } = null!;

        public string FullName { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string Role { get; set; } = null!;

        public bool IsActive { get; set; }
    }
}
