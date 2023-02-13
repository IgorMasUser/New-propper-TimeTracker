using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MassTransit;
using TimeTracker;
using BackgroundService.Host.Extansions;

namespace BackgroundService.Host.Extensions
{
    public static class MassTransitServiceCollectionExtensions
    {
        public static IServiceCollection AddMassTransitServices(this IServiceCollection services, IConfiguration configuration)
        {
            Uri schedulerEndpoint = new Uri("queue:scheduler");
            services.AddMassTransit(cfg =>
            {
                cfg.AddMessageScheduler(schedulerEndpoint);

                cfg.SetKebabCaseEndpointNameFormatter();

                cfg.AddMessageScheduler(schedulerEndpoint);
                cfg.AddScheduledJobs<MyScheduledJobOptions, TimerValidation>(
                configuration.GetSection("TimerConfiguration"),

                jobCfg =>
                {
                    jobCfg.AddJob<IScheduledMessage>()
                    .ConsumedBy<ScheduledMessageConsumer>()
                    .WithPeriodicSchedule(opt => new TaskScheduler(opt.JobCronSetting))
                    .WithPayload(new { scheduledMessage = "It's been 2 minutes, perform the task !!!!!" })
                    .Register();
                });
                cfg.UsingRabbitMq((context, cfg) =>
                {
                    cfg.UseMessageScheduler(schedulerEndpoint);
                    cfg.Host("localhost", "/", h =>
                        {
                            h.Username("guest");
                            h.Password("guest");
                        });
                    cfg.ConfigureEndpoints(context);
                });

                services.AddHostedService<MyMassTransitHostedService>();
                services.AddHostedService<RequestService>();
            });


            return services;
        }
    }

}
