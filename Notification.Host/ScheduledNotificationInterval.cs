using MassTransit.Scheduling;
using System;

namespace Notification.Host.NotificationInterval
{
    public class ScheduledNotificationInterval : DefaultRecurringSchedule
    {
        public ScheduledNotificationInterval()
        {
            CronExpression = "0 0 10 ? * FRIL *";  // "0/10 * * ? * *" - every 10 sec ; "0 0/1 * 1/1 * ? *" - every minute ; "0 0 10 ? * FRIL *"; - at 10:00 every last Friday of every month
            TimeZoneId = TimeZoneInfo.Local.Id;
        }
    }
}
