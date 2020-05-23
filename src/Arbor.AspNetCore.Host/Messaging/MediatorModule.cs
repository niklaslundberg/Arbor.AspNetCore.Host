using Arbor.App.Extensions.Application;
using Arbor.App.Extensions.DependencyInjection;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;

namespace Arbor.AspNetCore.Host.Messaging
{
    [UsedImplicitly]
    public class MediatorModule : IModule
    {
        public IServiceCollection Register(IServiceCollection builder) =>
            MediatorRegistrationHelper.Register(builder, ApplicationAssemblies.FilteredAssemblies(), this);
    }
}