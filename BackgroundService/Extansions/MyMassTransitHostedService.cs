using MassTransit;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BackgroundService.Host.Extensions
{
    public class MyMassTransitHostedService : IHostedService
    {
        private readonly IBusControl bus;
        private readonly ILogger logger;

        public MyMassTransitHostedService(IBusControl bus, ILoggerFactory loggerFactory)
        {
            this.bus = bus;
            this.logger = loggerFactory.CreateLogger<MyMassTransitHostedService>();
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Starting bus");
            await bus.StartAsync(cancellationToken).ConfigureAwait(false);

        }
        public Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Stopping bus");
            return bus.StopAsync(cancellationToken);
        }


    }
}