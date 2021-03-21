using System;

namespace Arbor.AspNetCore.Host
{
    public interface ISchedule
    {
        public DateTimeOffset? Next(DateTimeOffset currentTime);
    }
}