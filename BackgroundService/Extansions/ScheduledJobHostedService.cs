using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MassTransit.ScheduleJobs
{
    /// <summary>Represents implementation of <see cref="IHostedService"/> for starting and cancelling scheduled jobs.</summary>
    public class ScheduledJobHostedService : IHostedService
    {
        private readonly IBus bus;
        private readonly ILogger<ScheduledJobHostedService> logger;
        private readonly IEnumerable<Func<IBus, Task<ScheduledRecurringMessage>>> scheduledJobSetups;
        private readonly IList<ScheduledRecurringMessage> registeredJobs;

        /// <summary>Initializes new instance of <see cref="ScheduledJobHostedService"/>.</summary>
        /// <param name="bus">Instance of <see cref="IBus"/>.</param>
        /// <param name="logger">Instance of <see cref="ILogger{RecurringScheduleHostedService}"/>.</param>
        /// <param name="scheduledJobs">Instance registered scheduled job delegates.</param>
        public ScheduledJobHostedService(
            IBus bus,
            ILogger<ScheduledJobHostedService> logger,
            IEnumerable<Func<IBus, Task<ScheduledRecurringMessage>>> scheduledJobs)
        {
            this.bus = bus ?? throw new ArgumentNullException(nameof(bus));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.scheduledJobSetups = scheduledJobs ?? throw new ArgumentNullException(nameof(scheduledJobs));
            this.registeredJobs = new List<ScheduledRecurringMessage>();
        }

        /// <summary>Triggered when the application host is ready to start the service.</summary>
        /// <param name="cancellationToken">Indicates that the start process has been aborted.</param>
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation($"Initializing requested {scheduledJobSetups.Count()} recurring scheduled job(s).");

            try
            {
                foreach (var jobSetup in scheduledJobSetups)
                {
                    var createdRecurringSchedule = await jobSetup(bus);
                    registeredJobs.Add(createdRecurringSchedule);

                    logger.LogInformation("Scheduled job is initialized. {@createdRecurringSchedule}", createdRecurringSchedule);
                }

                logger.LogInformation("Initialization of recurring scheduled job(s) are completed.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to initialize recurring scheduled job(s).");
                throw;
            }
        }

        /// <summary>Triggered when the application host is performing a graceful shutdown.</summary>
        /// <param name="cancellationToken">Indicates that the shutdown process should no longer be graceful.</param>
        public async Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation($"Cancelling registered {registeredJobs.Count} recurring scheduled job(s).");

            try
            {
                foreach (var job in registeredJobs)
                {
                    await bus.CancelScheduledRecurringSend(job.Schedule.ScheduleId, job.Schedule.ScheduleGroup);
                }

                logger.LogInformation("Recurring scheduled job(s) are cancelled.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to cancel recurring scheduled job(s).");
                throw;
            }
        }
    }
}