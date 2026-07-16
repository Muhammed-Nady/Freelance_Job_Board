using Freelify.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class FreelancerSkillConfig : IEntityTypeConfiguration<FreelancerSkill>
{
    public void Configure(EntityTypeBuilder<FreelancerSkill> builder)
    {
        builder.HasKey(fs => new
        {
            fs.FreelancerProfileId,
            fs.SkillId
        });

        builder.HasOne(fs => fs.FreelancerProfile)
            .WithMany(f => f.FreelancerSkills)
            .HasForeignKey(fs => fs.FreelancerProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(fs => fs.Skill)
            .WithMany(s => s.FreelancerSkills)
            .HasForeignKey(fs => fs.SkillId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}