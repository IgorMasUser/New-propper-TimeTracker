using MassTransit;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace MassTransitSchedulingTest
{
    public class SchedulerInitService : BackgroundService
    {
        private IBus myBus;

        public SchedulerInitService(IBus bus)
        {
            myBus = bus;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            myBus.Topology.TryGetPublishAddress<Message>(out var address);
            await myBus.ScheduleRecurringSend(address, new MessageSchedule(), new Message { Value = "It's been 10 sec please perform the task" });
        }
    }
}
