using MassTransit;
using Microsoft.Extensions.Logging;

namespace BackgroundService.Host.Extensions
{
    internal class ScheduledMessageConsumer : IConsumer<IScheduledMessage>
    {
        private readonly ILogger<ScheduledMessageConsumer> logger;
        public ScheduledMessageConsumer(ILogger<ScheduledMessageConsumer> logger)
        {
            this.logger = logger;
        }
        public async Task Consume(ConsumeContext<IScheduledMessage> context)
        {
            logger.Log(LogLevel.Information, "Reminding:{scheduledMessage} ", context.Message.scheduledMessage);

            await context.Publish<MessageConsumed>(new
            {
                Timestamp = DateTime.Now,
                Message = context.Message.scheduledMessage
            });
        }
    }
}