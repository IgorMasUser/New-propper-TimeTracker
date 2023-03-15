using MassTransit;
using Notification.Host;
using Notification.Host.Extansions;
using Notification.Host.Extensions;

namespace Notification.Host
{
    public class Program
    {
        public static async Task<int> Main(string[] args)
        {

            try
            {
                await CreateHostBuilder(args)
                    .Build()
                    .RunAsync();

                return 0;
            }
            catch (Exception ex)
            {
                return 1;
            }

        }

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args)
                .ConfigureServices(services =>
                {
                    services.AddMassTransit(x =>
                    {
                        Uri schedulerEndpoint = new Uri("queue:scheduler");
                        Console.WriteLine($"queue {nameof(schedulerEndpoint)}");
                        x.AddMessageScheduler(schedulerEndpoint);

                        x.AddConsumer<ScheduledMessageConsumer>();

                        x.UsingRabbitMq((context, cfg) =>
                        {
                            cfg.UseDelayedMessageScheduler();
                            cfg.UseMessageScheduler(schedulerEndpoint);
                            cfg.ConfigureEndpoints(context);
                        });

                    });

                    //services.AddHostedService<Worker>();
                    services.AddHostedService<RemindingService>();
                });

        }
    }
}


//IHost host = Host.CreateDefaultBuilder(args)
//    .ConfigureServices(services =>
//    {
//        services.AddMassTransit(x =>
//        {
//            Uri schedulerEndpoint = new Uri("queue:scheduler");

//            x.AddMessageScheduler(schedulerEndpoint);

//            x.AddConsumer<ScheduledMessageConsumer>();

//            x.UsingRabbitMq((context, cfg) =>
//            {
//                cfg.UseDelayedMessageScheduler();
//                cfg.ConfigureEndpoints(context);
//            });

//        });

//        //services.AddHostedService<RemindingService>();
//        services.AddHostedService<Worker>();
//    })
//    .Build();

//await host.RunAsync();

