using System;
using MassTransit.Scheduling;

namespace MassTransit.ScheduleJobs
{
    /// <summary>Represents class for scheduled job definition.</summary>
    public class ScheduledJob
    {
        /// <summary>Initializes new instance of <see cref="ScheduledJob"/>.</summary>
        /// <param name="destinationAddress">Destination queue address.</param>
        /// <param name="periodicSchedule">Periodic schedule details.s</param>
        /// <param name="messagePayload">Payload of message.</param>
        public ScheduledJob(
            string destinationAddress, 
            RecurringSchedule periodicSchedule, 
            object messagePayload)
        {
            DestinationAddress = destinationAddress ?? throw new ArgumentNullException(nameof(destinationAddress));
            PeriodicSchedule = periodicSchedule ?? throw new ArgumentNullException(nameof(periodicSchedule));
            MessagePayload = messagePayload;
        }

        /// <summary>Gets destination address.</summary>
        public string DestinationAddress { get; private set; }

        /// <summary>Gets Recurring schedule trigger object.</summary>
        public RecurringSchedule PeriodicSchedule { get; private set; }

        /// <summary>Gets the message will be received from scheduler.</summary>
        public object MessagePayload { get; private set; }
    }
}