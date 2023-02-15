
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BackgroundService.Host.Extensions
{
    public class MyHostedService : IHostedService
    {
        private readonly ILogger logger;

        public MyHostedService(ILoggerFactory loggerFactory)
        {
            this.logger = loggerFactory.CreateLogger<MyHostedService>();
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Service started!!!!!");
            Console.WriteLine("Service started!!!!!");
            await Task.Delay(1000);

        }
        public Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Stopping bus");
            return Task.CompletedTask;
        }


    }
}