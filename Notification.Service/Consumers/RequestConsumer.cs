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
            if (context.Message.UserId.Contains("TEST"))
            {
                if (context.RequestId != null)
                {
                    await context.RespondAsync<NewComerApprovalRequestRejected>(new
                    {
                        InVar.Timestamp,
                        context.Message.ApprovalId,
                        context.Message.UserId,
                        Reason = $"Test Customer cannot submit order:{context.Message.UserId}"
                    });
                }
                return;
            }

            await context.Publish<NewComerApprovalRequested>(new
            {
                context.Message.ApprovalId,
                context.Message.TimeStamp,
                context.Message.UserId
            });

            if (context.RequestId != null)
            {
                await context.RespondAsync<NewComerApprovalRequestAccepted>(new
                {
                    InVar.Timestamp,
                    context.Message.ApprovalId,
                    context.Message.UserId

                });
            }
        }
    }
}
