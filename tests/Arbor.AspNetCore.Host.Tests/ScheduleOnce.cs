using System;

namespace Arbor.AspNetCore.Host.Tests
{
    public class ScheduleOnce : ISchedule
    {
        private readonly DateTimeOffset _dateTimeOffset;

        private bool _fired;

        private int invokedCount;

        private object lockObject = new ();

        public ScheduleOnce(DateTimeOffset dateTimeOffset) => _dateTimeOffset = dateTimeOffset;

        public DateTimeOffset? Next(DateTimeOffset currentTime)
        {
            if (_fired)
            {
                return null;
            }

            ++invokedCount;

            Console.WriteLine($"{nameof(ScheduleOnce)} invoked {invokedCount}");

            if (currentTime > _dateTimeOffset)
            {
                if (_fired)
                {
                    return null;
                }

                lock (lockObject)
                {
                    if (_fired)
                    {
                        return null;
                    }

                    _fired = true;

                    Console.WriteLine($"Schedule once fired {invokedCount}");

                    return _dateTimeOffset;
                }
            }
            else
            {
                Console.WriteLine($"{nameof(ScheduleOnce)} not yet ready {invokedCount}");
            }

            return _dateTimeOffset;
        }
    }
}