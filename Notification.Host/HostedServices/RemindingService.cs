using MassTransit;
using MassTransitSchedulingTest;

namespace Notification.Host.Extensions
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
            logger.LogInformation("Service started!!!!!");

            string notification = "It's been 10 sec please perform the task";

            //Uri sendEndpointUri = new("queue:scheduler");
            bus.Topology.TryGetPublishAddress<IScheduledNotification>(out var address);
            await bus.ScheduleRecurringSend(address, new MessageSchedule(), new ScheduledNotification { Value = notification });

        }
        public Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Stopping bus");
            return Task.CompletedTask;
        }


    }
}