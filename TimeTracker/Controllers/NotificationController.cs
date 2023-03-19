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
            public IEnumerable<NotificationMessage> GetNotifications()
        {
           var notifications =  repository.GetAllNotifications();

            return notifications;
        }


    }
}
