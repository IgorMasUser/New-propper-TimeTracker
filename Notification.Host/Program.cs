
using MassTransit;
using Notification.Host;
using Notification.Host.Extansions;
using Notification.Host.Extensions;

//var builder = WebApplication.CreateBuilder(args);
////builder.Services.AddHostedService<RemindingService>();
//builder.Services.AddHostedService<Worker>();

//builder.Services.AddMassTransit(x =>
//{
//    Uri schedulerEndpoint = new Uri("queue:scheduler");

//    x.AddMessageScheduler(schedulerEndpoint);

//    x.AddConsumer<ScheduleNotificationConsumer>();

//    x.UsingRabbitMq((context, cfg) =>
//    {
//        cfg.UseMessageScheduler(schedulerEndpoint);
//        cfg.ConfigureEndpoints(context);
//    });

//});

//var app = builder.Build();
//app.Run();

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddMassTransit(x =>
        {
            Uri schedulerEndpoint = new Uri("queue:scheduler");

            x.AddMessageScheduler(schedulerEndpoint);
            // x.AddDelayedMessageScheduler();

            x.AddConsumer<ScheduleNotificationConsumer>();

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.UseDelayedMessageScheduler();
                cfg.ConfigureEndpoints(context);
            });

        });

        services.AddHostedService<RemindingService>();
        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();

