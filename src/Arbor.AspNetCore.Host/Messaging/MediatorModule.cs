using Arbor.App.Extensions.Application;
using Arbor.App.Extensions.DependencyInjection;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;

namespace Arbor.AspNetCore.Host.Messaging
{
    [UsedImplicitly]
    public class MediatorModule : IModule
    {
        private readonly IApplicationAssemblyResolver _applicationAssemblyResolver;

        public MediatorModule(IApplicationAssemblyResolver applicationAssemblyResolver) =>
            _applicationAssemblyResolver = applicationAssemblyResolver;

        public IServiceCollection Register(IServiceCollection builder) =>
            MediatorRegistrationHelper.Register(builder, _applicationAssemblyResolver.GetAssemblies(), this);
    }
}