using Freelify.Models.Entities.Jobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Freelify.Data.Configurations
{
    public class SkillConfig : IEntityTypeConfiguration<Skill>
    {
        public void Configure(EntityTypeBuilder<Skill> builder)
        {
            builder.Property(s => s.Name)
                .HasMaxLength(100)
                .IsRequired();

            builder.HasData(

    // Web Development
    new Skill { Id = 1, Name = "HTML" },
    new Skill { Id = 2, Name = "CSS" },
    new Skill { Id = 3, Name = "JavaScript" },
    new Skill { Id = 4, Name = "TypeScript" },
    new Skill { Id = 5, Name = "React" },
    new Skill { Id = 6, Name = "Angular" },
    new Skill { Id = 7, Name = "Vue.js" },
    new Skill { Id = 8, Name = "ASP.NET Core" },
    new Skill { Id = 9, Name = "Node.js" },
    new Skill { Id = 10, Name = "Express.js" },
    new Skill { Id = 11, Name = "Laravel" },
    new Skill { Id = 12, Name = "Django" },
    new Skill { Id = 13, Name = "Flask" },

    // Mobile Development
    new Skill { Id = 14, Name = "Flutter" },
    new Skill { Id = 15, Name = "React Native" },
    new Skill { Id = 16, Name = "Android" },
    new Skill { Id = 17, Name = "Kotlin" },
    new Skill { Id = 18, Name = "Java" },
    new Skill { Id = 19, Name = "Swift" },
    new Skill { Id = 20, Name = "iOS" },

    // UI/UX
    new Skill { Id = 21, Name = "Figma" },
    new Skill { Id = 22, Name = "Adobe XD" },
    new Skill { Id = 23, Name = "Wireframing" },
    new Skill { Id = 24, Name = "Prototyping" },
    new Skill { Id = 25, Name = "User Research" },

    // Data Science & AI
    new Skill { Id = 26, Name = "Python" },
    new Skill { Id = 27, Name = "Machine Learning" },
    new Skill { Id = 28, Name = "Deep Learning" },
    new Skill { Id = 29, Name = "TensorFlow" },
    new Skill { Id = 30, Name = "PyTorch" },
    new Skill { Id = 31, Name = "Computer Vision" },
    new Skill { Id = 32, Name = "NLP" },
    new Skill { Id = 33, Name = "Pandas" },
    new Skill { Id = 34, Name = "NumPy" },
    new Skill { Id = 35, Name = "SQL" },

    // Graphic Design
    new Skill { Id = 36, Name = "Photoshop" },
    new Skill { Id = 37, Name = "Illustrator" },
    new Skill { Id = 38, Name = "Canva" },
    new Skill { Id = 39, Name = "Brand Identity" },
    new Skill { Id = 40, Name = "Logo Design" },

    // Writing & Translation
    new Skill { Id = 41, Name = "Content Writing" },
    new Skill { Id = 42, Name = "Copywriting" },
    new Skill { Id = 43, Name = "Technical Writing" },
    new Skill { Id = 44, Name = "Arabic Translation" },
    new Skill { Id = 45, Name = "English Translation" },

    // Cyber Security
    new Skill { Id = 46, Name = "Penetration Testing" },
    new Skill { Id = 47, Name = "Network Security" },
    new Skill { Id = 48, Name = "Ethical Hacking" },
    new Skill { Id = 49, Name = "OWASP" },
    new Skill { Id = 50, Name = "SIEM" }
);
        }
    }
}