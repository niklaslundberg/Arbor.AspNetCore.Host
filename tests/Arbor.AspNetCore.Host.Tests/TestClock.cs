using System;
using Arbor.App.Extensions.Time;

namespace Arbor.AspNetCore.Host.Tests
{
    public class TestClock : ICustomClock
    {
        private DateTimeOffset _dateTimeOffset;
        private readonly TimeSpan _tickDuration;

        public TestClock(DateTimeOffset dateTimeOffset, TimeSpan tickDuration)
        {
            _dateTimeOffset = dateTimeOffset;
            _tickDuration = tickDuration;
        }

        public DateTimeOffset UtcNow()
        {
            _dateTimeOffset = _dateTimeOffset.Add(_tickDuration);

            return _dateTimeOffset;
        }

        public DateTime LocalNow() => throw new NotSupportedException();

        public DateTime ToLocalTime(DateTime dateTimeUtc) => throw new NotSupportedException();
        public TimeZoneInfo DefaultTimeZone { get; } = TimeZoneInfo.Utc;
    }
}