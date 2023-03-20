using Microsoft.AspNetCore.Mvc;
using TimeTracker.Data;
using TimeTracker.Models;

namespace TimeTracker.Controllers
{
    public class NotificationController : Controller
    {
        private readonly INotificationRepo repository;

        public NotificationController(INotificationRepo repository)
        {
            this.repository = repository;
        }

        [HttpGet]
        public ActionResult<IEnumerable<NotificationMessage>> GetNotifications()
        {
            var notifications = repository.GetAllNotifications();

            if(notifications != null)
            {
                return View(notifications);
            }
            else
            {
                List<NotificationMessage> noNotificationsReply = new List<NotificationMessage>();
                noNotificationsReply.Add(new NotificationMessage {Message = "You don't have any notifications"});
                return View(noNotificationsReply);
            }
        }
    }
}
