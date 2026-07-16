using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Freelify.Models.Entities;


namespace Freelify.Data.Configurations
{

    public class ApplicationConfig: IEntityTypeConfiguration<Application>
    {
        public void Configure(EntityTypeBuilder<Application> builder)
        {
           
            builder.HasOne(a => a.FreelancerProfile)
                .WithMany(f => f.Applications)
                .HasForeignKey(a => a.FreelancerProfileId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(a => a.Attachments)
                .WithOne(att => att.Application)
                .HasForeignKey(att => att.ApplicationId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
