namespace Arbor.AspNetCore.Host
{
    public interface IScheduler
    {
        public bool Add(ISchedule schedule, OnTickAsync onTick);
    }
}