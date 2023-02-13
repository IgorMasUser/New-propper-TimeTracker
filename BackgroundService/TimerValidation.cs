using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackgroundService.Host
{
    internal class TimerValidation : IValidateOptions<MyScheduledJobOptions>
    {
        public ValidateOptionsResult Validate(string name, MyScheduledJobOptions options)
        {
            if (options == null || name == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (string.IsNullOrEmpty(options.JobCronSetting))
            {
                return ValidateOptionsResult.Fail("I missed cron settings.");
            }

            return ValidateOptionsResult.Success;
        }
    }
}
