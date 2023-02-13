using MassTransit.Scheduling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackgroundService.Host
{
    internal class TaskScheduler : DefaultRecurringSchedule
    {
        public TaskScheduler(string cron)
        {
            CronExpression = cron;
        }
    }
}
