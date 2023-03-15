using System;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Hosting;
using Notification.Host;
using Notification.Host.Extansions;



namespace Notification.Host.Extansions
{

    public class Worker : IHostedService
    {
        private readonly IBusControl bus;
        private readonly ILogger logger;

        public Worker(IBusControl bus, ILoggerFactory loggerFactory)
        {
            this.bus = bus;
            this.logger = loggerFactory.CreateLogger<Worker>();
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Starting bus");

            logger.LogInformation("---------------- SETTING UP RECURRING SCHEDULE -----------------");

            Uri sendEndpointUri = new("queue:scheduler");

            var sendEndpoint = await bus.GetSendEndpoint(sendEndpointUri);

            string consumerEndpointName = DefaultEndpointNameFormatter.Instance.Consumer<ScheduledMessageConsumer>();
            //string cron = "0 0/2 * 1/1 * ? *";
            string cron = "0/10 * * ? * *";

            await sendEndpoint.ScheduleRecurringSend(new Uri($"queue:{consumerEndpointName}"), new TaskScheduler(cron), new SimpleRequest("Hello world!"), cancellationToken);
            await bus.StartAsync(cancellationToken).ConfigureAwait(false);

        }
        public Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Stopping bus");
            return bus.StopAsync(cancellationToken);
        }


    }
}

