using Freelify.Data;
using Freelify.Hubs;
using Freelify.Models.Entities;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Freelify.Services
{
    public class NotificationService
    {
        private readonly AppDbContext _context;
        private readonly IHubContext<NotificationHub> _notificationHub;
        public NotificationService(AppDbContext dbContext, IHubContext<NotificationHub> notificationHub)
        {
            _context = dbContext;
            _notificationHub = notificationHub;
        }

        public async Task AddNotification(Notification newNotification)
        {
            // check if the notification already exists for the user and related entity
            var relatedNotifications = await _context.Notifications.Where(
                n => n.UserId == newNotification.UserId &&
                n.RelatedEntityId == newNotification.RelatedEntityId &&
                n.Type == newNotification.Type)
                .ToListAsync();

            var unreadNotification = relatedNotifications.FirstOrDefault(n => !n.IsRead);

            if (unreadNotification != null)
            {
                // If there's an unread notification, update its message and timestamp
                unreadNotification.Message = newNotification.Message;
                unreadNotification.CreatedDate = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                await _notificationHub.Clients.User(unreadNotification.UserId).SendAsync("ReceiveNotification", unreadNotification);
            }
            else
            {
                if (relatedNotifications.Any())
                {
                    _context.Notifications.RemoveRange(relatedNotifications);
                }

                _context.Notifications.Add(newNotification);

                await _context.SaveChangesAsync();

                await _notificationHub.Clients.User(newNotification.UserId).SendAsync("ReceiveNotification", newNotification);
            }


        }
        public async Task<IEnumerable<Notification>> GetNotifications(string userId)
        {
            return await _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedDate)
                .ToListAsync();
        }

        public async Task<int> GetUnreadCount(string userId)
        {
            return await _context.Notifications.CountAsync(n => n.UserId == userId && !n.IsRead);
        }

        public async Task MarkAllAsRead(string userId)
        {
            var notifcations = await _context.Notifications.Where(n => n.UserId == userId && !n.IsRead).ToListAsync();

            foreach (var notification in notifcations)
            {
                notification.IsRead = true;
            }

            await _context.SaveChangesAsync();
        }
    }
}
