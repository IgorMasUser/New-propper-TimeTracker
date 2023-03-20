using Contracts;

namespace Notification.Host.Notification
{
    public class ScheduledNotification : IScheduledNotification
    {
        public string Value { get; set; }
    }
}
