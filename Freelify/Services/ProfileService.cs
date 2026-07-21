using Freelify.Data;
using Freelify.Models.Entities;
using Freelify.Models.Entities.Users;
using Freelify.Models.Enums;
using Freelify.Models.Results;
using Freelify.Models.ViewModels.Auth;
using Freelify.Models.ViewModels.Profile;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Freelify.Services
{
    public class ProfileService
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly FileUploadService _fileUploadService;

        public ProfileService(AppDbContext context, UserManager<ApplicationUser> userManager, FileUploadService fileUploadService)
        {
            _context = context;
            _userManager = userManager;
            _fileUploadService = fileUploadService;
        }



        private async Task<FreelancerProfileViewModel?> _GetFreelancerProfile(ApplicationUser user)
        {
            var freelancer = await _context.FreelancerProfiles
                .Include(f => f.FreelancerSkills)
                .ThenInclude(fs => fs.Skill)
                .FirstOrDefaultAsync(f => f.UserId == user.Id);

            if (freelancer == null) return null;

            return new FreelancerProfileViewModel
            {
                FullName = user.FullName,
                Email = user.Email ?? "",
                PhoneNumber = user.PhoneNumber ?? "Not Provided",
                ProfileImageUrl = user.ProfileImageUrl,
                Bio = string.IsNullOrEmpty(freelancer.Bio) ? "No bio provided yet." : freelancer.Bio,
                Experience = string.IsNullOrEmpty(freelancer.Experience) ? "No experience details provided yet." : freelancer.Experience,
                CreatedDate = user.CreatedDate,
                Skills = freelancer.FreelancerSkills
                .Select(fs => fs.Skill.Name)
                .ToList()

            };
        }


        private async Task<ClientProfileViewModel?> _GetClientProfile(ApplicationUser user)
        {
            var client = await _context.ClientProfiles
                .FirstOrDefaultAsync(c => c.UserId == user.Id);

            if (client == null) return null;

            return new ClientProfileViewModel
            {
                FullName = user.FullName,
                Email = user.Email ?? "",
                PhoneNumber = user.PhoneNumber ?? "Not Provided",
                ProfileImageUrl = user.ProfileImageUrl,
                CompanyName = string.IsNullOrEmpty(client.CompanyName) ? "No company name added yet" : client.CompanyName,
                CompanyLogoUrl = string.IsNullOrEmpty(client.CompanyLogoUrl)
                    ? user.ProfileImageUrl
                    : client.CompanyLogoUrl,
                CompanyDescription = string.IsNullOrEmpty(client.CompanyDescription)
                    ? "No description provided yet."
                    : client.CompanyDescription,
                CreatedDate = user.CreatedDate
            };
        }

        private async Task<EditFreelancerProfileViewModel?> _GetEditFreelancerProfile(ApplicationUser user)
        {
            var freelancer = await _context.FreelancerProfiles
                .Include(f => f.FreelancerSkills)
                .FirstOrDefaultAsync(f => f.UserId == user.Id);

            if (freelancer == null) return null;

            return new EditFreelancerProfileViewModel
            {
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber ?? "",
                ExistingProfileImageUrl = user.ProfileImageUrl,
                Bio = freelancer.Bio ?? "",
                Experience = freelancer.Experience ?? "",
                SelectedSkillIds = freelancer.FreelancerSkills
                .Select(x => x.SkillId)
                .ToList(),

            };
        }

        private async Task<EditClientProfileViewModel?> _GetEditClientProfile(ApplicationUser user)
        {
            var client = await _context.ClientProfiles
                .FirstOrDefaultAsync(c => c.UserId == user.Id);

            if (client == null) return null;

            return new EditClientProfileViewModel
            {
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber ?? "",
                ExistingProfileImageUrl = user.ProfileImageUrl,
                CompanyName = client.CompanyName ?? "",
                ExistingCompanyLogoUrl = client.CompanyLogoUrl,
                CompanyDescription = client.CompanyDescription ?? ""
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

                if (viewModel == null)
                    return new ProfileResult { Success = false, ErrorType = ErrorType.NotFound, ErrorMessage = "Freelancer Profile Not Found" };
                return new ProfileResult { Success = true, ViewName = "FreelancerProfile", ViewModel = viewModel };
            }
            else if (roles.Contains("Client"))
            {
                var viewModel = await _GetClientProfile(user);
                if (viewModel == null)
                    return new ProfileResult { Success = false, ErrorType = ErrorType.NotFound, ErrorMessage = "Client Profile Not Found" };
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
                if (viewModel == null)
                    return new ProfileResult { Success = false, ErrorType = ErrorType.NotFound, ErrorMessage = "Freelancer Profile Not Found" };
                return new ProfileResult { Success = true, ViewName = "EditFreelancer", ViewModel = viewModel };
            }
            else if (roles.Contains("Client"))
            {
                var viewModel = await _GetEditClientProfile(user);
                if (viewModel == null)
                    return new ProfileResult { Success = false, ErrorType = ErrorType.NotFound, ErrorMessage = "Client Profile Not Found" };
                return new ProfileResult { Success = true, ViewName = "EditClient", ViewModel = viewModel };
            }

            return new ProfileResult { Success = false, ErrorType = ErrorType.BadRequest, ErrorMessage = "Invalid Role" };
        }

        public async Task<ProfileResult> EditClientProfile(ClaimsPrincipal principal, EditClientProfileViewModel editClientModel)
        {
            var user = await _userManager.GetUserAsync(principal);

            if (user == null) return new ProfileResult { Success = false, ErrorType = ErrorType.NotFound, ErrorMessage = "User Not Found" };

            var client = await _context.ClientProfiles.FirstOrDefaultAsync(c => c.UserId == user.Id);

            if (client == null) return new ProfileResult { Success = false, ErrorType = ErrorType.NotFound, ErrorMessage = "Client Profile Not Found" };

            user.FullName = editClientModel.FullName;
            user.PhoneNumber = editClientModel.PhoneNumber;

            if (editClientModel.ProfileImage != null)
            {
                var uploadResult = await _fileUploadService.UploadFile(editClientModel.ProfileImage, UploadFileType.Image);
                if (!uploadResult.Success)
                {
                    return new ProfileResult
                    {
                        Success = false,
                        ErrorType = uploadResult.ErrorType ?? ErrorType.BadRequest,
                        ErrorMessage = uploadResult.ErrorMessage
                    };
                }

                await _fileUploadService.DeleteFile(user.ProfileImageUrl);
                user.ProfileImageUrl = uploadResult.Url ?? "";
            }

            if (editClientModel.CompanyLogo != null)
            {
                var uploadResult = await _fileUploadService.UploadFile(editClientModel.CompanyLogo, UploadFileType.Image);
                if (!uploadResult.Success)
                {
                    return new ProfileResult
                    {
                        Success = false,
                        ErrorType = uploadResult.ErrorType ?? ErrorType.BadRequest,
                        ErrorMessage = uploadResult.ErrorMessage
                    };
                }

                await _fileUploadService.DeleteFile(client.CompanyLogoUrl);
                client.CompanyLogoUrl = uploadResult.Url ?? "";
            }

            client.CompanyName = editClientModel.CompanyName ?? "";
            client.CompanyDescription = editClientModel.CompanyDescription ?? "";

            var res = await _userManager.UpdateAsync(user);

            if (!res.Succeeded)
            {
                return new ProfileResult
                {
                    Success = false,
                    ErrorType = ErrorType.BadRequest,
                    ErrorMessage = string.Join(", ", res.Errors.Select(e => e.Description))
                };
            }

            await _context.SaveChangesAsync();

            return new ProfileResult { Success = true };
        }

        public async Task<ProfileResult> EditFreelancerProfile(ClaimsPrincipal principal, EditFreelancerProfileViewModel editFreelancerModel)
        {
            var user = await _userManager.GetUserAsync(principal);

            if (user == null) return new ProfileResult { Success = false, ErrorType = ErrorType.NotFound, ErrorMessage = "User Not Found" };

            var freelancer = await _context.FreelancerProfiles
                .Include(f => f.FreelancerSkills)
                .FirstOrDefaultAsync(f => f.UserId == user.Id);

            if (freelancer == null) return new ProfileResult { Success = false, ErrorType = ErrorType.NotFound, ErrorMessage = "Freelancer Profile Not Found" };

            user.FullName = editFreelancerModel.FullName;
            user.PhoneNumber = editFreelancerModel.PhoneNumber;

            if (editFreelancerModel.ProfileImage != null)
            {
                var uploadResult = await _fileUploadService.UploadFile(editFreelancerModel.ProfileImage, UploadFileType.Image);
                if (!uploadResult.Success)
                {
                    return new ProfileResult
                    {
                        Success = false,
                        ErrorType = uploadResult.ErrorType ?? ErrorType.BadRequest,
                        ErrorMessage = uploadResult.ErrorMessage
                    };
                }

                await _fileUploadService.DeleteFile(user.ProfileImageUrl);
                user.ProfileImageUrl = uploadResult.Url ?? "";
            }

            freelancer.Bio = editFreelancerModel.Bio ?? "";
            freelancer.Experience = editFreelancerModel.Experience ?? "";
            freelancer.FreelancerSkills.Clear();

            foreach (var skillId in editFreelancerModel.SelectedSkillIds ?? [])
            {
                freelancer.FreelancerSkills.Add(new FreelancerSkill
                {
                    FreelancerProfileId = freelancer.Id,
                    SkillId = skillId
                });
            }


            var res = await _userManager.UpdateAsync(user);

            if (!res.Succeeded)
            {
                return new ProfileResult
                {
                    Success = false,
                    ErrorType = ErrorType.BadRequest,
                    ErrorMessage = string.Join(", ", res.Errors.Select(e => e.Description))
                };
            }

            await _context.SaveChangesAsync();

            return new ProfileResult { Success = true };
        }

        public async Task<ProfileResult> ChangePassword(ClaimsPrincipal principal, ChangePasswordViewModel model)
        {
            var user = await _userManager.GetUserAsync(principal);

            if (user == null) return new ProfileResult { Success = false, ErrorType = ErrorType.NotFound, ErrorMessage = "User Not Found" };

            var res = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

            if (res.Succeeded) return new ProfileResult { Success = true };

            return new ProfileResult
            {
                Success = false,
                ErrorType = ErrorType.BadRequest,
                ErrorMessage = string.Join(", ", res.Errors.Select(e => e.Description))
            };
        }

        public async Task LoadSkillsAsync(dynamic viewBag, List<int>? selected = null)
        {
            viewBag.Skills = new MultiSelectList(
                await _context.Skills.ToListAsync(),
                "Id",
                "Name",
                selected);
        }
    }
}
