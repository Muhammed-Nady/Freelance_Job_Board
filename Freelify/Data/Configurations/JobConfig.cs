using Freelify.Models.Entities;
using Freelify.Models.Entities.Jobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Freelify.Data.Configurations
{
    public class JobConfig : IEntityTypeConfiguration<Job>
    {
        public void Configure(EntityTypeBuilder<Job> builder)
        {
            builder.HasOne(j => j.ClientProfile)
                .WithMany(c => c.Jobs)
                .HasForeignKey(j => j.ClientProfileId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(j => j.Category)
                .WithMany(c => c.Jobs)
                .HasForeignKey(j => j.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(j => j.Attachments)
                .WithOne(a => a.Job)
                .HasForeignKey(a => a.JobId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(j => j.Applications)
                .WithOne(a => a.Job)
                .HasForeignKey(a => a.JobId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(j => j.Reviews)
                .WithOne(r => r.Job)
                .HasForeignKey(r => r.JobId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(j => new
            {
                j.Status,
                j.CategoryId,
                j.CreatedAt
            });
        }
    }
}
