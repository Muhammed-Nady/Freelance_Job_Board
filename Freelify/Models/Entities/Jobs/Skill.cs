using System.ComponentModel.DataAnnotations;

namespace Freelify.Models.Entities.Jobs
{
    public class Skill
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public ICollection<JobSkill> JobSkills { get; set; } = new List<JobSkill>();

        public ICollection<FreelancerSkill> FreelancerSkills { get; set; } = new List<FreelancerSkill>();
    }
}