using MassTransit;

namespace BackgroundService.Host
{
    public class ScheduleNotificationConsumer :IConsumer<ScheduleNotification>
    {
        public async Task Consume(ConsumeContext<ScheduleNotification> context)
        {
            Uri notificationService = new Uri("queue:notification-service");
            await context.ScheduleSend<SendNotification>(notificationService,
                context.Message.DeliveryTime, new()
                {
                    EmailAddress = context.Message.EmailAddress,
                    Body = context.Message.Body
                });
        }
    }
}