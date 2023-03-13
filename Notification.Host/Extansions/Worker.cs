namespace Notification.Host.Extansions
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using MassTransit;
    using Microsoft.Extensions.Hosting;


    public class Worker : BackgroundService
    {
        readonly IMessageScheduler _scheduler;
        private readonly ILogger<Worker> logger;

        public Worker(IMessageScheduler scheduler, ILogger<Worker> logger)
        {
            _scheduler = scheduler;
            this.logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("Worker service started!!!!!");

            await _scheduler.SchedulePublish<ScheduleNotification>(DateTime.UtcNow + TimeSpan.FromSeconds(30), new
            {
                EmailAddress = "frank@nul.org",
                Body = "Thank you for signing up for our awesome newsletter!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!Worker"
            }, stoppingToken);

        }
    }
}

