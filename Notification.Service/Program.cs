using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Notification.Service.Consumers;
using Notification.Service.HostedServices;
using Quartz;

IHost host = Host.CreateDefaultBuilder(args)
 .ConfigureServices((hostContext, services) =>
 {
     services.AddQuartz(q =>
     {
         q.UseMicrosoftDependencyInjectionJobFactory();
     });
     services.AddMassTransit(x =>
     {
         x.AddConsumer<RequestConsumer>();
         x.AddQuartzConsumers();
         x.UsingRabbitMq((cxt, cfg) =>
         {
             cfg.ConfigureEndpoints(cxt);
         });
     });

     services.AddHostedService<RemindingService>();
 })
 .Build();

await host.RunAsync();