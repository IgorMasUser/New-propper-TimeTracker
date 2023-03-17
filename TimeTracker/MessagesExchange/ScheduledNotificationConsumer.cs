using CacheService.Models;
using Contracts;
using MassTransit;
using StackExchange.Redis;

namespace MassTransitSchedulingTest
{
    public class ScheduledNotificationConsumer : IConsumer<IScheduledNotification>
    {
        private readonly ILogger<ScheduledNotificationConsumer> logger;
        private readonly IConnectionMultiplexer redis;

        public ScheduledNotificationConsumer(ILogger<ScheduledNotificationConsumer> logger, IConnectionMultiplexer redis)
        {
            this.logger = logger;
            this.redis = redis;
        }

        public async Task Consume(ConsumeContext<IScheduledNotification> context)
        {
            var message = new { Id = $"message:{Guid.NewGuid().ToString()}"};
        
            logger.LogInformation($"Notification id:{message.Id}");
            logger.LogInformation($"Message: {context.Message.Value} consumed by API");

            var db = redis.GetDatabase();
            db.StringSet(message.Id, context.Message.Value);
            var platGet = db.StringGet(message.Id);
            logger.LogInformation($"Get saved Platform:{platGet}");
        }
    }
}
