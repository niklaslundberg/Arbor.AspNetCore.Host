using System;

namespace Arbor.AspNetCore.Host.Tests
{
    public class ScheduleEvery3Second : ISchedule
    {
        private DateTimeOffset _dateTimeOffset;

        public ScheduleEvery3Second(DateTimeOffset start) => _dateTimeOffset = start;

        public DateTimeOffset? Next(DateTimeOffset now)
        {
            if (now > _dateTimeOffset)
            {
                var current = _dateTimeOffset;
                _dateTimeOffset = _dateTimeOffset.AddSeconds(3);
                return current;
            }

            return _dateTimeOffset;
        }
    }
}