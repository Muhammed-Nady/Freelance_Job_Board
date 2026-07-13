using Freelify.Models.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Freelify.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ClientProfile>().ToTable("ClientProfiles");
            builder.Entity<FreelancerProfile>().ToTable("FreelancerProfiles");
        }


        public DbSet<ClientProfile> ClientProfiles { get; set; }

        public DbSet<FreelancerProfile> FreelancerProfiles { get; set; }


    }
}
