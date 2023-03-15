using MassTransit;
using MassTransitSchedulingTest;
using Notification.Host;
using Notification.Host.Extensions;
using Quartz;

namespace Notification.Host
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
           Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                services.AddQuartz(q =>
                {
                    q.UseMicrosoftDependencyInjectionJobFactory();
                });

                services.AddMassTransit(x =>
                {
                    x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("test", false));
                    x.AddPublishMessageScheduler();
                    x.AddQuartzConsumers();
                    x.AddConsumer<MessageConsumer>();
                    x.UsingRabbitMq((cxt, cfg) =>
                    {
                        string hostName = "localhost";
                        Console.WriteLine(hostName);

                        cfg.Host(hostName, "/", h =>
                        {
                            h.Username("guest");
                            h.Password("guest");
                        });

                        cfg.ConfigureEndpoints(cxt);
                    });

                });

                services.Configure<MassTransitHostOptions>(opt =>
                {
                    opt.WaitUntilStarted = true;
                });

                services.AddQuartzHostedService(options =>
                {
                    options.StartDelay = TimeSpan.FromSeconds(5);
                    options.WaitForJobsToComplete = true;
                });

                services.AddHostedService<SchedulerInitService>();
            });
    }
}