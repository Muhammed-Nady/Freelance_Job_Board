using System.ComponentModel.DataAnnotations;

namespace Freelify.Models.Entities.Jobs
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public ICollection<Job> Jobs { get; set; } = new List<Job>();
    }
}