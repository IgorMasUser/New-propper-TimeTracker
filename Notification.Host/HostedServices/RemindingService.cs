using Contracts;
using MassTransit;
using Notification.Host.Notification;
using Notification.Host.NotificationInterval;

namespace Notification.Host.HostedServices
{
    public class RemindingService : IHostedService
    {
        private readonly ILogger logger;
        private readonly IBus bus;

        public RemindingService(ILoggerFactory loggerFactory, IBus bus)
        {
            this.logger = loggerFactory.CreateLogger<RemindingService>();
            this.bus = bus;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Service started");

            string notificationMessage = "It's been 1 minute please perform the task";

            bus.Topology.TryGetPublishAddress<IScheduledNotification>(out var sendEndpointUri);
            await bus.ScheduleRecurringSend(sendEndpointUri, new ScheduledNotificationInterval(), new ScheduledNotification { Value = notificationMessage });

        }
        public Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Stopping bus");
            return Task.CompletedTask;
        }


    }
}