using MassTransit;
using Notification.Host.HostedServices;
using Quartz;

namespace Notification.Host
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            IConfigurationBuilder configBuilder;

            if (builder.Environment.IsDevelopment())
            {
                configBuilder = new ConfigurationBuilder().AddJsonFile("appsettings.Development_Host.json", false, true);
            }
            else
            {
                configBuilder = new ConfigurationBuilder().AddJsonFile("appsettings_Host.json", false, true);
            }

            IConfigurationRoot configuration = configBuilder.Build();

            CreateHostBuilder(args, configuration).Build().Run();
        }


        public static IHostBuilder CreateHostBuilder(string[] args, IConfiguration configuration) =>
           Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                services.AddQuartz(q =>
                {
                    q.UseMicrosoftDependencyInjectionJobFactory();
                });
                services.AddMassTransit(x =>
                {
                    x.AddQuartzConsumers();
                    x.UsingRabbitMq((cxt, cfg) =>
                    {
                        string hostName = configuration.GetSection("RabbitMQHost:HostName").Value;

                        cfg.Host(hostName, "/", h =>
                        {
                            h.Username("guest");
                            h.Password("guest");
                        });

                        cfg.ConfigureEndpoints(cxt);
                    });

                });

                services.AddHostedService<RemindingService>();
            });
    }
}