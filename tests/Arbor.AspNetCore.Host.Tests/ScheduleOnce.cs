using System;

namespace Arbor.AspNetCore.Host.Tests
{
    public class ScheduleOnce : ISchedule
    {
        private readonly DateTimeOffset _dateTimeOffset;

        private bool _fired;

        public ScheduleOnce(DateTimeOffset dateTimeOffset) => _dateTimeOffset = dateTimeOffset;

        public DateTimeOffset? Next(DateTimeOffset now)
        {
            Console.WriteLine("Schedule once invokation");

            if (_fired)
            {
                return null;
            }

            if (now > _dateTimeOffset)
            {
                _fired = true;
            }

            return _dateTimeOffset;

        }
    }
}