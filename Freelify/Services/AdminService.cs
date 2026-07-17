using Freelify.Data;
using Freelify.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Freelify.Services
{
    public class AdminService
    {
        private readonly AppDbContext _context;

        public AdminService(AppDbContext context)
        {
            _context = context;
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
    }
}
