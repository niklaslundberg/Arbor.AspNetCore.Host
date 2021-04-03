using System;

namespace Arbor.AspNetCore.Host.Scheduling
{
    public interface ISchedule
    {
        public DateTimeOffset? Next(DateTimeOffset currentTime);
    }
}