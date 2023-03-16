using MassTransit;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace MassTransitSchedulingTest
{
    public class ScheduledNotificationConsumer : IConsumer<IScheduledNotification>
    {
        readonly ILogger<ScheduledNotificationConsumer> myLogger;

        public ScheduledNotificationConsumer(ILogger<ScheduledNotificationConsumer> logger)
        {
            myLogger = logger;
        }

        public async Task Consume(ConsumeContext<IScheduledNotification> context)
        {
            myLogger.LogDebug(context.Message.Value);
            myLogger.LogInformation($"Message {context.Message.Value} consumed by API");
        }
    }
}
