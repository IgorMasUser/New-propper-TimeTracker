using Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;
using Notification.Service.ApprovalStateMachine;

namespace Notification.Service
{
    public class RequestConsumer : IConsumer<INewComerApprovalRequest>
    {
        private readonly ILogger<RequestConsumer> logger;

        public RequestConsumer(ILogger<RequestConsumer> logger)
        {
            this.logger = logger;
        }

        public async Task Consume(ConsumeContext<INewComerApprovalRequest> context)
        {
            logger.Log(LogLevel.Information, "New comer {0} sent for approval with ApprovalId {1}", context.Message.UserId, context.Message.ApprovalId);

            await context.Publish<INewComerApproval>(new
            {
                context.Message.ApprovalId,
                context.Message.TimeStamp,
                context.Message.UserId
            });

            logger.Log(LogLevel.Information, "published to Saga");

            await context.RespondAsync<ISimpleResponse>(new
            {
                context.Message.TimeStamp,
                context.Message.UserId,
                ResponseMessage = "I have got new comer request"
            });


        }

    }
}
