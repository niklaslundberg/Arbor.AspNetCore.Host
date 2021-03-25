using System;

namespace Arbor.AspNetCore.Host.Tests
{
    public class ScheduleOnce : ISchedule
    {
        private readonly DateTimeOffset _dateTimeOffset;

        private bool _fired;

        private int _invokedCount;

        private readonly object _lockObject = new ();

        public ScheduleOnce(DateTimeOffset dateTimeOffset) => _dateTimeOffset = dateTimeOffset;

        public DateTimeOffset? Next(DateTimeOffset currentTime)
        {
            if (_fired)
            {
                return null;
            }

            ++_invokedCount;

            Console.WriteLine($"{nameof(ScheduleOnce)} invoked {_invokedCount}");

            if (currentTime >= _dateTimeOffset)
            {
                if (_fired)
                {
                    return null;
                }

                lock (_lockObject)
                {
                    if (_fired)
                    {
                        return null;
                    }

                    _fired = true;

                    Console.WriteLine($"Schedule once fired {_invokedCount}");

                    return _dateTimeOffset;
                }
            }
            else
            {
                Console.WriteLine($"{nameof(ScheduleOnce)} not yet ready {_invokedCount}");
            }

            return _dateTimeOffset;
        }
    }
}