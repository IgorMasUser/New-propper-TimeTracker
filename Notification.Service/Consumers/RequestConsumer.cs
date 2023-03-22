using Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Notification.Service
{
    public class RequestConsumer : IConsumer<ISimpleRequest>
    {
        private readonly ILogger<RequestConsumer> logger;

        public RequestConsumer(ILogger<RequestConsumer> logger)
        {
            this.logger = logger;
        }

        public async Task Consume(ConsumeContext<ISimpleRequest> context)
        {
            logger.Log(LogLevel.Information, "Obtained message {0}", context.Message.SentMessage);

            await context.RespondAsync<ISimpleResponse>(new
            {
                Timestamp = context.Message.Timestamp,
                ResponseMessage = "I have got message"
            });
        }

    }
}
