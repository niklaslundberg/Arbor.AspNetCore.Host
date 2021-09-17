using Arbor.App.Extensions.DependencyInjection;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;

namespace Arbor.AspNetCore.Host.Sample
{
    [UsedImplicitly]
    public class TestModule : IModule
    {
        public IServiceCollection Register(IServiceCollection builder) =>
            builder.AddSingleton<UsingBackgroundService>();
    }
}