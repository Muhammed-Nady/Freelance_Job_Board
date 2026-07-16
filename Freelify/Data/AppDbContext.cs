using Freelify.Models.Entities;
using Freelify.Models.Entities.Jobs;
using Freelify.Models.Entities.Reviews;
using Freelify.Models.Entities.Users;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Freelify.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }


  

        public DbSet<ClientProfile> ClientProfiles { get; set; }

        public DbSet<FreelancerProfile> FreelancerProfiles { get; set; }

        public DbSet<Job> Jobs { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Skill> Skills { get; set; }

        public DbSet<JobSkill> JobSkills { get; set; }

        public DbSet<JobAttachment> JobAttachments { get; set; }

        public DbSet<Application> Applications { get; set; }

        public DbSet<ApplicationAttachment> ApplicationAttachments { get; set; }

        public DbSet<Review> Reviews { get; set; }

        public DbSet<PortfolioItem> PortfolioItems { get; set; }

        public DbSet<FreelancerSkill> FreelancerSkills { get; set; }

        public DbSet<Notification> Notifications { get; set; }


    }
}
