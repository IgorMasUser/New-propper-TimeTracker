using Contracts;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Notification.Service;
using Notification.Service.ApprovalStateMachine;
using Notification.Service.HostedServices;
using Quartz;
using StackExchange.Redis;

IHost host = Host.CreateDefaultBuilder(args)
 .ConfigureServices((hostContext, services) =>
 {
     services.AddQuartz(q =>
     {
         q.UseMicrosoftDependencyInjectionJobFactory();
     });
     services.AddMassTransit(x =>
     {
         //const string configurationString = "127.0.0.1";

         x.AddConsumer<RequestConsumer>();
         //x.AddSagaStateMachine<ApprovalStateMachine, ApprovalState>().RedisRepository(configurationString);
         //x.AddSagaStateMachine<ApprovalStateMachine, ApprovalState>().InMemoryRepository();
         //x.SetInMemorySagaRepositoryProvider();
         //x.AddRequestClient<INewComerApprovalRequest>(new Uri($"exchange:{KebabCaseEndpointNameFormatter.Instance.Consumer<RequestConsumer>()}"));
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