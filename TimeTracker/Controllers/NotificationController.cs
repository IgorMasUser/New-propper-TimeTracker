using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using TimeTracker.Data;
using TimeTracker.Models;

namespace TimeTracker.Controllers
{
    public class NotificationController : Controller
    {
        private readonly INotificationRepo repository;
        private readonly IMemoryCache memoryCache;

        public NotificationController(INotificationRepo repository, IMemoryCache memoryCache)
        {
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
            this.memoryCache = memoryCache;
        }

        [HttpGet]
        public ActionResult<IEnumerable<NotificationMessage>> GetNotifications()
        {
            var notifications = repository.GetAllNotifications();

            if (notifications != null)
            {
                var cachedNotifications = memoryCache.Get<IEnumerable<NotificationMessage?>>("notifications");

                if (cachedNotifications is null)
                {
                    cachedNotifications = repository.GetAllNotifications();

                    memoryCache.Set("notifications", cachedNotifications, TimeSpan.FromMinutes(1));
                }

                return View(cachedNotifications);
            }
            else
            {
                List<NotificationMessage> noNotificationsReply = new List<NotificationMessage>();
                noNotificationsReply.Add(new NotificationMessage { Message = "You don't have any notifications" });
                return View(noNotificationsReply);
            }
        }

        public async Task<IActionResult> DeleteNotification(string id)
        {
            await repository.DeleteNotification(id);

            return RedirectToAction("GetNotifications");
        }

        public async Task<IActionResult> DeleteAllNotifications()
        {
            await repository.DeleteAllNotifications();

            return RedirectToAction("GetNotifications");
        }
    }
}
