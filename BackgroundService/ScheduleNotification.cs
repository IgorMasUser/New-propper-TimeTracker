using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackgroundService.Host
{
    public record ScheduleNotification
    {
        public DateTime DeliveryTime { get; init; }
        public string EmailAddress { get; init; }
        public string Body { get; init; }
    }
}
