using Arbor.App.Extensions.Application;
using Arbor.App.Extensions.DependencyInjection;
using Arbor.App.Extensions.ExtensionMethods;
using Microsoft.Extensions.DependencyInjection;

namespace Arbor.AspNetCore.Host
{
    public class PreStartRegistrationModule : IModule
    {
        private readonly IApplicationAssemblyResolver _applicationAssemblyResolver;

        public PreStartRegistrationModule(IApplicationAssemblyResolver applicationAssemblyResolver) =>
            _applicationAssemblyResolver = applicationAssemblyResolver;

        public IServiceCollection Register(IServiceCollection builder)
        {
            var filteredAssemblies = _applicationAssemblyResolver.GetAssemblies();

            var loadablePublicConcreteTypesImplementing =
                filteredAssemblies.GetLoadablePublicConcreteTypesImplementing<IPreStartModule>();

            foreach (var loadable in loadablePublicConcreteTypesImplementing)
            {
                builder.AddSingleton(typeof(IPreStartModule), loadable, this);
            }

            return builder;
        }
    }
}