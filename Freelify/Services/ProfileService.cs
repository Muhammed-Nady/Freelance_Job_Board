using Freelify.Data;
using Freelify.Models.Entities;
using Freelify.Models.Enums;
using Freelify.Models.Results;
using Freelify.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Freelify.Services
{
    public class ProfileService
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProfileService(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }



        private async Task<FreelancerProfileViewModel> _GetFreelancerProfile(ApplicationUser user)
        {
            var freelancer = await _context.FreelancerProfiles.FirstOrDefaultAsync(f => f.Id == user.Id);
            return new FreelancerProfileViewModel
            {
                FullName = freelancer?.FullName ?? user.FullName,
                Email = user.Email ?? "",
                PhoneNumber = user.PhoneNumber ?? "Not Provided",
                ProfileImageUrl = freelancer?.ProfileImageUrl ?? user.ProfileImageUrl,
                Bio = freelancer?.Bio ?? "No bio provided yet. Edit your profile to tell clients about yourself!",
                Experience = freelancer?.Experience ?? "No experience details provided yet.",
                CreatedDate = user.CreatedDate
            };
        }

        private async Task<ClientProfileViewModel> _GetClientProfile(ApplicationUser user)
        {
            var client = await _context.ClientProfiles.FirstOrDefaultAsync(c => c.Id == user.Id);
            return new ClientProfileViewModel
            {
                FullName = client?.FullName ?? user.FullName,
                Email = user.Email ?? "",
                PhoneNumber = user.PhoneNumber ?? "Not Provided",
                ProfileImageUrl = client?.ProfileImageUrl ?? user.ProfileImageUrl,
                CompanyName = client?.CompanyName ?? "No company name added yet",
                CompanyLogoUrl = client?.CompanyLogoUrl ?? client?.ProfileImageUrl ?? user.ProfileImageUrl,
                CompanyDescription = client?.CompanyDescription ?? "No description provided yet.",
                CreatedDate = user.CreatedDate
            };
        }
        private async Task<EditFreelancerProfileViewModel> _GetEditFreelancerProfile(ApplicationUser user)
        {
            var freelancer = await _context.FreelancerProfiles.FirstOrDefaultAsync(f => f.Id == user.Id);
            return new EditFreelancerProfileViewModel
            {
                FullName = freelancer?.FullName ?? user.FullName,
                PhoneNumber = user.PhoneNumber ?? "Not Provided",
                ProfileImageUrl = freelancer?.ProfileImageUrl ?? user.ProfileImageUrl,
                Bio = freelancer?.Bio ?? "",
                Experience = freelancer?.Experience ?? ""
            };
        }

        private async Task<EditClientProfileViewModel> _GetEditClientProfile(ApplicationUser user)
        {
            var client = await _context.ClientProfiles.FirstOrDefaultAsync(c => c.Id == user.Id);
            return new EditClientProfileViewModel
            {
                FullName = client?.FullName ?? user.FullName,
                PhoneNumber = user.PhoneNumber ?? "Not Provided",
                ProfileImageUrl = client?.ProfileImageUrl ?? user.ProfileImageUrl,
                CompanyName = client?.CompanyName ?? "",
                CompanyLogoUrl = client?.CompanyLogoUrl,
                CompanyDescription = client?.CompanyDescription ?? ""
            };
        }
        public async Task<ProfileResult> GetProfileDetailsAsync(ClaimsPrincipal principal)
        {
            var user = await _userManager.GetUserAsync(principal);

            if (user == null) return new ProfileResult { Success = false, ErrorType = ErrorType.NotFound, ErrorMessage = "User Not Found" };

            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Contains("Freelancer"))
            {
                var viewModel = await _GetFreelancerProfile(user);
                return new ProfileResult { Success = true, ViewName = "FreelancerProfile", ViewModel = viewModel };
            }
            else if (roles.Contains("Client"))
            {
                var viewModel = await _GetClientProfile(user);
                return new ProfileResult { Success = true, ViewName = "ClientProfile", ViewModel = viewModel };
            }

            return new ProfileResult { Success = false, ErrorType = ErrorType.BadRequest, ErrorMessage = "Invalid Role" };
        }

        public async Task<ProfileResult> GetEditProfileDetailsAsync(ClaimsPrincipal principal)
        {
            var user = await _userManager.GetUserAsync(principal);

            if (user == null) return new ProfileResult() { Success = false, ErrorType = ErrorType.NotFound, ErrorMessage = "User Not Found" };

            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Contains("Freelancer"))
            {
                var viewModel = await _GetEditFreelancerProfile(user);
                return new ProfileResult { Success = true, ViewName = "EditFreelancer", ViewModel = viewModel };
            }
            else if (roles.Contains("Client"))
            {
                var viewModel = await _GetEditClientProfile(user);
                return new ProfileResult { Success = true, ViewName = "EditClient", ViewModel = viewModel };
            }

            return new ProfileResult { Success = false, ErrorType = ErrorType.BadRequest, ErrorMessage = "Invalid Role" };
        }

        public async Task EditProfile(ClaimsPrincipal principal)
        {
            // TODO
        }

        public async Task ChangePassword(ClaimsPrincipal principal, string newPassword)
        {
            // TODO
        }
    }
}
