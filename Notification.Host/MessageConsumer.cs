using MassTransit;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace MassTransitSchedulingTest
{
    public class MessageConsumer : IConsumer<Message>
    {
        readonly ILogger<MessageConsumer> myLogger;

        public MessageConsumer(ILogger<MessageConsumer> logger)
        {
            myLogger = logger;
        }

        public async Task Consume(ConsumeContext<Message> context)
        {
            myLogger.LogDebug(context.Message.Value);
            myLogger.LogInformation($"Message {context.Message.Value} consumed");
        }
    }
}
