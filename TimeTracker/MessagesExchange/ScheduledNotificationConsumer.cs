using Contracts;
using MassTransit;
using StackExchange.Redis;
using System.Text.Json;
using TimeTracker.Models;

namespace MassTransitSchedulingTest
{
    public class ScheduledNotificationConsumer : IConsumer<IScheduledNotification>
    {
        private readonly ILogger<ScheduledNotificationConsumer> logger;
        private readonly IConnectionMultiplexer redis;

        public ScheduledNotificationConsumer(ILogger<ScheduledNotificationConsumer> logger, IConnectionMultiplexer redis)
        {
            if (redis == null)
            {
                throw new ArgumentNullException(nameof(redis));
            }
            this.logger = logger;
            this.redis = redis;
        }

        public async Task Consume(ConsumeContext<IScheduledNotification> context)
        {
            NotificationMessage message = new NotificationMessage();
            message.Message = context.Message.Value;
            logger.LogInformation($"Notification id:{message.Id}");
            logger.LogInformation($"Message: {context.Message.Value} consumed by API");

            var db = redis.GetDatabase();
            var serialMessage = JsonSerializer.Serialize(message);
            db.HashSet($"message", new HashEntry[] {new HashEntry(message.Id, serialMessage) });
            //db.StringSet(message.Id, context.Message.Value);
            //var getMessage = db.StringGet(message.Id);
            //logger.LogInformation($"Get saved Platform:{getMessage}");
        }
    }
}
