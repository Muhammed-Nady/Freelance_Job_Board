using Freelify.Data;
using Freelify.Models.Entities;
using Freelify.Models.Entities.Users;
using Freelify.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Freelify.Services
{
    public class AdminService
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminService(AppDbContext context,UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
     
        public async Task<AdminDashboardViewModel> GetDashboardAsync()
        {
            return new AdminDashboardViewModel
            {
                TotalUsers = await _context.Users.CountAsync(),
                TotalClients = await _context.ClientProfiles.CountAsync(),
                TotalFreelancers = await _context.FreelancerProfiles.CountAsync(),
                TotalJobs = await _context.Jobs.CountAsync(),
                TotalApplications = await _context.Applications.CountAsync()
            };
        }
        // --------------Get all users with their roles and active status-----------------
        public async Task<List<UserManageViewModel>> GetUsersAsync()
        {
            var users = await _context.Users.ToListAsync();

            var list = new List<UserManageViewModel>();

            foreach (var user in users)
            {
                var role = (await _userManager.GetRolesAsync(user)).FirstOrDefault();

                list.Add(new UserManageViewModel
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email!,
                    Role = role!,
                    IsActive = user.IsActive
                });
            }

            return list;
        }

        // --------------------------Change user active status--------------------------
        public async Task ToggleUserStatusAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
                return;

            var roles = await _userManager.GetRolesAsync(user);

            if (roles.Contains("Admin"))
                return;

            user.IsActive = !user.IsActive;

            await _userManager.UpdateAsync(user);
        }


        public async Task<List<Job>> GetAllJobsAsync()
        {
            return await _context.Jobs
                .Include(j => j.ClientProfile)
                .Include(j => j.Category)
                .Include(j => j.Applications)
                .OrderByDescending(j => j.CreatedAt)
                .ToListAsync();
        }

    }
}
