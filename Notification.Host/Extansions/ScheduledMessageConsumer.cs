using MassTransit;

namespace Notification.Host.Extansions
{
    public class ScheduledMessageConsumer : IConsumer<SimpleRequest>
    {
        private readonly ILogger<ScheduledMessageConsumer> logger;

        public ScheduledMessageConsumer(ILogger<ScheduledMessageConsumer> logger)
        {
            this.logger = logger;
        }
        public async Task Consume(ConsumeContext<SimpleRequest> context)
        {
            logger.Log(LogLevel.Information, "Reminding:{scheduledMessage} ", context.Message);

            await context.Publish<MessageConsumed>(new
            {
                Timestamp = DateTime.Now,
                Message = context.Message
            });
        }
    }
}