using Freelify.Models.Entities;
using Freelify.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Freelify.Controllers
{
    public class NotificationController : Controller
    {
        private readonly NotificationService _notificationService;

        public NotificationController(NotificationService notificationService)
        {
            _notificationService = notificationService;
        }
        public IActionResult Index()
        {
            return View();
        }


        [Authorize]
        [HttpGet("Notifications")]
        public ActionResult<IEnumerable<Notification>> GetNotifications()
        {
            var notifications = _notificationService.GetNotifications(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            return Ok(notifications);
        }

        [Authorize]
        [HttpGet("Notifications/MarkAllAsRead")]
        public async Task<object> MarkAllAsRead()
        {
            await _notificationService.MarkAllAsRead(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            return Ok(new { success = true });
        }

        [Authorize]
        [HttpGet("Notifications/UnreadCount")]
        public ActionResult<object> GetUnreadCount()
        {
            var unreadCount = _notificationService.GetUnreadCount(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            return Ok(new { count = unreadCount });
        }
    }
}
