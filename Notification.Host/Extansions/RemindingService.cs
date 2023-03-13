using MassTransit;

namespace Notification.Host.Extensions
{
    public class RemindingService : IHostedService
    {
        private readonly ILogger logger;
        public RemindingService(ILoggerFactory loggerFactory)
        {
            this.logger = loggerFactory.CreateLogger<RemindingService>();
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Service started!!!!!");

            //while (true)
            //{
                logger.LogInformation("it's been 5 seconds");
                await Task.Delay(5000);
            //}

        }
        public Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Stopping bus");
            return Task.CompletedTask;
        }


    }
}