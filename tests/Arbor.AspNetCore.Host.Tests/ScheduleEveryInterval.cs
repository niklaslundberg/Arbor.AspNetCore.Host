using System;
using Arbor.AspNetCore.Host.Scheduling;

namespace Arbor.AspNetCore.Host.Tests
{
    public class ScheduleEveryInterval : ISchedule
    {
        private readonly TimeSpan _interval;
        private DateTimeOffset _nextInvokationTime;

        public ScheduleEveryInterval(TimeSpan interval, DateTimeOffset start)
        {
            _interval = interval;
            _nextInvokationTime = start;
        }

        public DateTimeOffset? Next(DateTimeOffset currentTime)
        {
            if (currentTime >= _nextInvokationTime)
            {
                var current = _nextInvokationTime;
                _nextInvokationTime = _nextInvokationTime.Add(_interval);
                return current;
            }

            return _nextInvokationTime;
        }
    }
}