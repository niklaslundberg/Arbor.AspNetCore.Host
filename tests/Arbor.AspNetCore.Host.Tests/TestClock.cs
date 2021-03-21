using System;
using Arbor.App.Extensions.Time;

namespace Arbor.AspNetCore.Host.Tests
{
    public class TestClock : ICustomClock
    {
        private readonly DateTimeOffset _dateTimeOffset;
        private int _millisecondsElapsed;

        public TestClock(DateTimeOffset dateTimeOffset)
        {
            _dateTimeOffset = dateTimeOffset;
        }

        public DateTimeOffset UtcNow()
        {
            return _dateTimeOffset.AddMilliseconds(_millisecondsElapsed += 100);
        }

        public DateTime LocalNow() => throw new NotSupportedException();

        public DateTime ToLocalTime(DateTime dateTimeUtc) => throw new NotSupportedException();
    }
}