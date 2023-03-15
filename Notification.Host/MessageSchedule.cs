using MassTransit.Scheduling;
using System;

namespace MassTransitSchedulingTest
{
    public class MessageSchedule : DefaultRecurringSchedule
    {
        public MessageSchedule()
        {
            //CronExpression = "0 0/1 * 1/1 * ? *"; //every minute
            CronExpression = "0/10 * * ? * *";  //every 10 sec
            TimeZoneId = TimeZoneInfo.Local.Id;
        }
    }
}
