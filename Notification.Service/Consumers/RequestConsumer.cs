using Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Notification.Service.Consumers
{
    public class RequestConsumer : IConsumer<NewComerApprovalRequest>
    {
        private readonly ILogger<RequestConsumer> logger;

        public RequestConsumer(ILogger<RequestConsumer> logger)
        {
            this.logger = logger;
        }
        public async Task Consume(ConsumeContext<NewComerApprovalRequest> context)
        {
            logger.Log(LogLevel.Information, "New comer {0} sent for approval", context.Message.UserEmail);

            await context.Publish<NewComerApprovalRequested>(new
            {
                context.Message.ApprovalId,
                context.Message.TimeStamp,
                context.Message.UserEmail
            });

            await context.RespondAsync<NewComerApprovalRequestAccepted>(new
            {
                InVar.Timestamp,
                context.Message.ApprovalId,
                context.Message.UserEmail
            });
        }
    }
}
