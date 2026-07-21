using Freelify.Data;
using Freelify.Models.Entities;

namespace Freelify.Services
{
    public class NotificationService
    {
        private readonly AppDbContext _context;
        public NotificationService(AppDbContext dbContext)
        {
            _context = dbContext;
        }

        public IEnumerable<Notification> GetNotifications(string userId)
        {
            return _context.Notifications.Where(n => n.UserId == userId).ToList();
        }

        public int GetUnreadCount(string userId)
        {
            return _context.Notifications.Count(n => n.UserId == userId && !n.IsRead);
        }

        public async Task MarkAllAsRead(string userId)
        {
            var notifcations = _context.Notifications.Where(n => n.UserId == userId && !n.IsRead).ToList();

            foreach (var notification in notifcations)
            {
                notification.IsRead = true;
            }

            await _context.SaveChangesAsync();
        }
    }
}
