using System;
using System.Threading.Tasks;
using MassTransit;
using MassTransit.Scheduling;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace MassTransit.ScheduleJobs
{
    public class ScheduledJobRegistrar<TScheduledJobOptions, TMessage>
        where TScheduledJobOptions : class, new()
        where TMessage : class
    {
        private readonly IServiceCollection services;
        private Func<TScheduledJobOptions, IEndpointNameFormatter, string> destinationAddressSetup;
        private Func<TScheduledJobOptions, RecurringSchedule> periodicScheduleSetup;
        private object payload;

        /// <summary>Initializes new instance of <see cref="ScheduledJobRegistrar{TScheduledJobOptions,TMessage}"/>.</summary>
        /// <param name="services">Instance of <see cref="IServiceCollection"/>.</param>
        public ScheduledJobRegistrar(IServiceCollection services)
        {
            this.services = services ?? throw new ArgumentNullException(nameof(services));
        }

        /// <summary>Sets destination address of consumer by naming convention.</summary>
        /// <typeparam name="TConsumer">Type of consumer.</typeparam>
        /// <returns>Instance of <see cref="ScheduledJobRegistrar{TScheduledJobOptions,TMessage}"/>.</returns>
        public ScheduledJobRegistrar<TScheduledJobOptions, TMessage> ConsumedBy<TConsumer>()
            where TConsumer : class, IConsumer<TMessage>
        {
            this.destinationAddressSetup = (options, formatter) => formatter.Consumer<TConsumer>();
            return this;
        }

        /// <summary>Sets destination address of consumer via delegate.</summary>
        /// <param name="setup">Delegate for setting destination address.</param>
        /// <returns>Instance of <see cref="ScheduledJobRegistrar{TScheduledJobOptions,TMessage}"/>.</returns>
        public ScheduledJobRegistrar<TScheduledJobOptions, TMessage> ConsumedAt(
            Func<TScheduledJobOptions, IEndpointNameFormatter, string> setup)
        {
            this.destinationAddressSetup = setup;
            return this;
        }

        /// <summary>Sets periodic schedule details of job.</summary>
        /// <param name="setup">Delegate for periodic schedule setup.</param>
        /// <returns>Instance of <see cref="ScheduledJobRegistrar{TScheduledJobOptions,TMessage}"/>.</returns>
        public ScheduledJobRegistrar<TScheduledJobOptions, TMessage> WithPeriodicSchedule(Func<TScheduledJobOptions, RecurringSchedule> setup)
        {
            this.periodicScheduleSetup = setup;
            return this;
        }

        /// <summary>Sets message payload of scheduled job.</summary>
        /// <param name="messagePayload">Payload of scheduled message.</param>
        /// <returns>Instance of <see cref="ScheduledJobRegistrar{TScheduledJobOptions,TMessage}"/>.</returns>
        public ScheduledJobRegistrar<TScheduledJobOptions, TMessage> WithPayload(object messagePayload)
        {
            this.payload = messagePayload;
            return this;
        }

        /// <summary>Registers scheduled job to <see cref="IServiceCollection"/>.</summary>
        public void Register()
        {
            services.AddTransient<Func<IBus, Task<ScheduledRecurringMessage>>>(provider =>
            {
                var endpointFormatter = provider.GetRequiredService<IEndpointNameFormatter>();
                var scheduleJobOptions = provider.GetRequiredService<IOptions<TScheduledJobOptions>>().Value;

                var scheduledJob = new ScheduledJob(
                    destinationAddressSetup(scheduleJobOptions, endpointFormatter), 
                    periodicScheduleSetup(scheduleJobOptions), 
                    payload);

                async Task<ScheduledRecurringMessage> ScheduleSendDelegate(IBus bus) => await bus
                    .ScheduleRecurringSend<TMessage>(
                        new Uri($"queue:{scheduledJob.DestinationAddress}"),
                        scheduledJob.PeriodicSchedule,
                        scheduledJob.MessagePayload ?? new { });

                return ScheduleSendDelegate;
            });
        }
    }
}