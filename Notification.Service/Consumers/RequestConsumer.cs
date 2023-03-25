using Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Notification.Service.Consumers
{
    public class RequestConsumer : IConsumer<NewComerApprovalRequest>
    {
        private readonly ILogger<RequestConsumer> _logger;

        public RequestConsumer(ILogger<RequestConsumer> logger)
        {
            _logger = logger;
        }
        public async Task Consume(ConsumeContext<NewComerApprovalRequest> context)
        {
            _logger.Log(LogLevel.Debug, "New comer {0} sent for approval", context.Message.UserId);

            await context.Publish<NewComerApprovalRequested>(new
            {
                context.Message.ApprovalId,
                context.Message.TimeStamp,
                context.Message.UserId
            });

            await context.RespondAsync<NewComerApprovalRequestAccepted>(new
            {
                InVar.Timestamp,
                context.Message.ApprovalId,
                context.Message.UserId
            });
        }
    }
}
