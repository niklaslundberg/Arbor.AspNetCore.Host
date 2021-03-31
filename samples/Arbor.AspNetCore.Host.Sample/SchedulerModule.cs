using Arbor.App.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Arbor.AspNetCore.Host.Sample
{
    public class SchedulerModule : IModule
    {
        public IServiceCollection Register(IServiceCollection builder)
        {
            builder.AddSingleton<ITimer, SystemTimer>();
            builder.AddSingleton<IScheduler, Scheduler>();
            builder.AddSingleton<ScheduledService, ScheduledLog>();

            return builder;
        }
    }
}