using System;
using Cronos;

namespace Arbor.AspNetCore.Host.Scheduling
{
    public class CronSchedule : ISchedule
    {
        private readonly CronExpression _cronExpression;

        public CronSchedule(CronExpression cronExpression) => _cronExpression = cronExpression;

        public DateTimeOffset? Next(DateTimeOffset currentTime)
        {
            var adjusted = currentTime;

            if (currentTime.Millisecond == 0)
            {
                adjusted = currentTime.UtcDateTime.AddMilliseconds(-1);
            }

            return _cronExpression.GetNextOccurrence(adjusted.UtcDateTime);
        }
    }
}