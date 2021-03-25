using System.Collections.Immutable;

namespace Arbor.AspNetCore.Host
{
    public interface IScheduler
    {
        public bool Add(ISchedule schedule, OnTickAsync onTick);

        public ImmutableArray<ISchedule> Schedules { get; }
    }
}