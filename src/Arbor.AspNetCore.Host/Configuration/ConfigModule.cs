using Arbor.App.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Arbor.AspNetCore.Host.Configuration
{
    public class ConfigModule : IModule
    {
        public IServiceCollection Register(IServiceCollection builder) => builder.AddSingleton<UserConfigUpdater>();
    }
}