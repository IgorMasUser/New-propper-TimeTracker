using MassTransit.ScheduleJobs;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackgroundService.Host.Extansions
{
    public class ScheduledJobsConfigurator<TScheduleJobOptions> where TScheduleJobOptions : class, new()
    {
        public IServiceCollection Services
        {
            get;
        }

        public ScheduledJobsConfigurator(IServiceCollection services)
        {
            Services = services;
        }

        public ScheduledJobRegistrar<TScheduleJobOptions, TMessage> AddJob<TMessage>() where TMessage : class
        {
            return new ScheduledJobRegistrar<TScheduleJobOptions, TMessage>(Services);
        }
    }
}
