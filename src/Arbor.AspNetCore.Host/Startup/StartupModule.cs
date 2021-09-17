using Arbor.App.Extensions.DependencyInjection;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Arbor.AspNetCore.Host.Startup
{
    [UsedImplicitly]
    public class StartupModule : IModule
    {
        public IServiceCollection Register(IServiceCollection builder) => builder.AddSingleton(context =>
                new StartupTaskContext(context.GetServices<IStartupTask>(), context.GetRequiredService<ILogger>()),
            this);
    }
}