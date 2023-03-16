using Contracts;
using MassTransit;

namespace MassTransitSchedulingTest
{
    public class ScheduledNotificationConsumer : IConsumer<IScheduledNotification>
    {
        private readonly ILogger<ScheduledNotificationConsumer> logger;

        public ScheduledNotificationConsumer(ILogger<ScheduledNotificationConsumer> logger)
        {
            this.logger = logger;
        }

        public async Task Consume(ConsumeContext<IScheduledNotification> context)
        {
            logger.LogInformation($"Message {context.Message.Value} consumed by API");
        }
    }
}
