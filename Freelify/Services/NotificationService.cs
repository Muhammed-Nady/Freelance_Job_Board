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
                n.Type == newNotification.Type && !n.IsRead)
                .ToListAsync();

            if (relatedNotifications.Any())
            {
                foreach (var notification in relatedNotifications)
                {
                    notification.Message = newNotification.Message; // Update the message

                    notification.CreatedDate = DateTime.UtcNow;

                    await _notificationHub.Clients.User(notification.UserId).SendAsync("ReceiveNotification", notification);
                }
            }
            else
            {
                _context.Notifications.Add(newNotification);

                await _notificationHub.Clients.User(newNotification.UserId).SendAsync("ReceiveNotification", newNotification);
            }

            await _context.SaveChangesAsync();


        }
        public async Task<IEnumerable<Notification>> GetNotifications(string userId)
        {
            return await _context.Notifications.Where(n => n.UserId == userId).ToListAsync();
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
