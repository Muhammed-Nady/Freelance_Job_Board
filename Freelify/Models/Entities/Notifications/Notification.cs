
using Freelify.Models.Entities.Users;
using Freelify.Models.Enums;
using System.ComponentModel.DataAnnotations;

    namespace Freelify.Models.Entities
    {
        public class Notification
        {
            public int Id { get; set; }

            [Required]
            public string UserId { get; set; } = null!;

            public ApplicationUser User { get; set; } = null!;

            public NotificationType Type { get; set; }

            [MaxLength(300)]
            public string Message { get; set; } = string.Empty;

            public int RelatedEntityId { get; set; }

            public bool IsRead { get; set; }

            public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        }
    }

