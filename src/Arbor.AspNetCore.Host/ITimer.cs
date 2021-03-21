using System;
using System.Threading.Tasks;

namespace Arbor.AspNetCore.Host
{
    public interface ITimer
    {
        public Task Add(ISchedule schedule, Func<DateTimeOffset, Task> task);
    }
}