using Freelify.Models.Entities.Jobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Freelify.Data.Configurations
{
    public class CategoryConfig: IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.Property(c => c.Name)
                .HasMaxLength(100)
                .IsRequired();


            builder.HasData(
                new Category
                {
                    Id = 1,
                    Name = "Web Development",
                },
                new Category
                {
                    Id = 2,
                    Name = "Mobile Development",
                },
                new Category
                {
                    Id = 3,
                    Name = "UI/UX Design",
                },
                new Category
                {
                    Id = 4,
                    Name = "Data Science & AI",
                },
                new Category
                {
                    Id = 5,
                    Name = "Graphic Design",
                },
                new Category
                {
                    Id = 6,
                    Name = "Writing & Translation",
                },
                new Category
                {
                    Id = 7,
                    Name = "Cyber Security",
                }
            );
        }
    }
}
