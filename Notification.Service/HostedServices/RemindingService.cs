using Contracts;
using MassTransit;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Notification.Service.Notification;
using Notification.Service.NotificationInterval;

namespace Notification.Service.HostedServices
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

            string notificationMessage = "Please, do not forget to submit your monthly attendance";

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