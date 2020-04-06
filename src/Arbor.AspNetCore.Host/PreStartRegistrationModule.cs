using Arbor.App.Extensions;
using Arbor.App.Extensions.Application;
using Arbor.App.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Arbor.AspNetCore.Host
{
    public class PreStartRegistrationModule : IModule
    {
        public IServiceCollection Register(IServiceCollection builder)
        {
            var filteredAssemblies = ApplicationAssemblies.FilteredAssemblies();

            var loadablePublicConcreteTypesImplementing = filteredAssemblies.GetLoadablePublicConcreteTypesImplementing<IPreStartModule>();

            foreach (var loadable in loadablePublicConcreteTypesImplementing)
            {
                builder.AddSingleton(typeof(IPreStartModule), loadable, this);
            }

            return builder;
        }
    }
}