using Freelify.Models.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class FreelancerConfig: IEntityTypeConfiguration<FreelancerProfile>
{
    public void Configure(EntityTypeBuilder<FreelancerProfile> builder)
    {
        builder.HasOne(f => f.User)
            .WithOne()
            .HasForeignKey<FreelancerProfile>(f => f.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}