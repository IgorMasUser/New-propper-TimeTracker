using MassTransit;
using MassTransit.ScheduleJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading.Tasks;
using MassTransit.ScheduleJobs;

namespace BackgroundService.Host.Extansions
{
    public static class ScheduledJobExtensions
    {
        public static IBusRegistrationConfigurator AddScheduledJobs<TScheduleJobOptions, TScheduleJobOptionsValidator>(this IBusRegistrationConfigurator configurator, IConfigurationSection configurationSection, Action<ScheduledJobsConfigurator<TScheduleJobOptions>> setup) where TScheduleJobOptions : class, new() where TScheduleJobOptionsValidator : class, IValidateOptions<TScheduleJobOptions>
        {
            configurator.Configure<TScheduleJobOptions>(configurationSection, delegate (BinderOptions opt)
            {
                opt.BindNonPublicProperties = true;
            });
            configurator.TryAddEnumerable(ServiceDescriptor.Singleton<IValidateOptions<TScheduleJobOptions>, TScheduleJobOptionsValidator>());
            ScheduledJobsConfigurator<TScheduleJobOptions> obj = new ScheduledJobsConfigurator<TScheduleJobOptions>(configurator);
            setup(obj);
            configurator.AddHostedService<ScheduledJobHostedService>();
            if (!HasAnyRegisteredScheduledJob(configurator))
            {
                throw new InvalidOperationException("Unable to find any registered schedule job in DI container, did you forget to call 'Register' method?");
            }

            return configurator;
        }

        private static bool HasAnyRegisteredScheduledJob(IServiceCollection services)
        {
            return services.Any((ServiceDescriptor x) => x.ServiceType == typeof(Func<IBus, Task<ScheduledRecurringMessage>>));
        }
    }
}
