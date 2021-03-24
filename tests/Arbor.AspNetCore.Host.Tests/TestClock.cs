using System;
using Arbor.App.Extensions.Time;

namespace Arbor.AspNetCore.Host.Tests
{
    public class TestClock : ICustomClock
    {
        private DateTimeOffset _dateTimeOffset;
        private int _millisecondsElapsed;

        public TestClock(DateTimeOffset dateTimeOffset) => _dateTimeOffset = dateTimeOffset;

        public DateTimeOffset UtcNow()
        {
            _dateTimeOffset = _dateTimeOffset.AddMilliseconds(1000);

            return _dateTimeOffset;
        }

        public DateTime LocalNow() => throw new NotSupportedException();

        public DateTime ToLocalTime(DateTime dateTimeUtc) => throw new NotSupportedException();
    }
}