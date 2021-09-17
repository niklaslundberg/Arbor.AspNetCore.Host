using System.Collections.Immutable;

namespace Arbor.AspNetCore.Host.Scheduling
{
    public interface IScheduler
    {
        public ImmutableArray<ISchedule> Schedules { get; }

        public bool Add(ISchedule schedule, OnTickAsync onTick);
    }
}