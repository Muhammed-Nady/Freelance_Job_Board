using Freelify.Models.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class ClientConfig: IEntityTypeConfiguration<ClientProfile>

    {
        public void Configure(EntityTypeBuilder<ClientProfile> builder)
        {
            builder.HasOne(f => f.User)
                .WithOne()
                .HasForeignKey<ClientProfile>(f => f.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
