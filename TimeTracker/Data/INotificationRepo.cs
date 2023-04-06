using TimeTracker.Models;

namespace TimeTracker.Data
{
    public interface INotificationRepo
    {
        IEnumerable<NotificationMessage?>? GetAllNotifications();
        Task DeleteNotification(string Id);
        Task DeleteAllNotifications();
    }
}
