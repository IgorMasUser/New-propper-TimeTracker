using MassTransit;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BackgroundService.Host.Extensions
{
    internal class RequestService : IHostedService
    {
        private readonly IBusControl bus;
        private readonly ILogger logger;

        public RequestService(IBusControl bus, ILoggerFactory loggerFactory)
        {
            this.bus = bus;
            this.logger = loggerFactory.CreateLogger<RequestService>();
        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Starting RequestService bus");
            await bus.StartAsync(cancellationToken).ConfigureAwait(false);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Stopping RequestService bus");
            return bus.StopAsync(cancellationToken);
        }
    }
}