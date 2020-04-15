using Arbor.App.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Arbor.AspNetCore.Host.Sample
{
    public class TestModule : IModule
    {
        public IServiceCollection Register(IServiceCollection builder) => builder.AddSingleton<UsingBackgroundService>();
    }
}