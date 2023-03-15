using MassTransit.Scheduling;

namespace Notification.Host.Extansions
{
    internal class TaskScheduler : DefaultRecurringSchedule
    {
        public TaskScheduler(string cron)
        {
            CronExpression = cron;
        }
    }
}
