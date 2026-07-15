using Freelify.Models.Entities.Jobs;
using Freelify.Models.Entities.Reviews;
using Freelify.Models.Entities.Users;

using Freelify.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace Freelify.Models.Entities
{
    public class Job
    {
       
            public int Id { get; set; }

            [Required]
            [MaxLength(100)]
            public string Title { get; set; } = null!;

            [Required]
            public string Description { get; set; } = null!;

            [Required]
            public decimal Budget { get; set; }
            [Required]
            public DateTime Deadline { get; set; }

            public JobStatus Status { get; set; }

            public DateTime CreatedAt { get; set; }

            // FK
            public int ClientProfileId { get; set; }

            public ClientProfile ClientProfile { get; set; }

            // FK   
            [Required]
            public int CategoryId { get; set; }

            public Category Category { get; set; }

        // __________________Navigation_________________________

        public ICollection<JobSkill> JobSkills { get; set; }
            = new List<JobSkill>();

        public ICollection<JobAttachment> Attachments { get; set; }
            = new List<JobAttachment>();

        public ICollection<Application> Applications { get; set; }
            = new List<Application>();

        public ICollection<Review> Reviews { get; set; }
            = new List<Review>();

    }

    }

