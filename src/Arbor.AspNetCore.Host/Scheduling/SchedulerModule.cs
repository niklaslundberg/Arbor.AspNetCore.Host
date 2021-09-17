using System.Linq;
using Arbor.App.Extensions.Application;
using Arbor.App.Extensions.DependencyInjection;
using Arbor.App.Extensions.ExtensionMethods;
using Microsoft.Extensions.DependencyInjection;

namespace Arbor.AspNetCore.Host.Scheduling
{
    public class SchedulerModule : IModule
    {
        private readonly IApplicationAssemblyResolver _applicationAssemblyResolver;

        public SchedulerModule(IApplicationAssemblyResolver applicationAssemblyResolver) =>
            _applicationAssemblyResolver = applicationAssemblyResolver;

        public IServiceCollection Register(IServiceCollection builder)
        {
            builder.AddSingleton<ITimer, SystemTimer>();
            builder.AddSingleton<IScheduler, Scheduler>();

            var assemblies = _applicationAssemblyResolver.GetAssemblies();

            var loadablePublicConcreteTypesImplementing =
                assemblies.GetLoadablePublicConcreteTypesImplementing<ScheduledService>();

            foreach (var loadable in loadablePublicConcreteTypesImplementing.Where(t =>
                t.GetConstructors().Length == 1 &&
                !t.GetConstructors()[0].GetParameters().Any(p => p.ParameterType == typeof(ISchedule))))
            {
                builder.AddSingleton(typeof(ScheduledService), loadable, this);
            }

            return builder;
        }
    }
}