using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Arbor.AspNetCore.Host.Hosting;
using Arbor.AspNetCore.Host.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Arbor.AspNetCore.Host.Sample
{
    public class TestServiceProviderModule : IServiceProviderModule
    {
        public void Register(ServiceProviderHolder serviceProviderHolder)
        {
            var logger = serviceProviderHolder.ServiceProvider.GetRequiredService<ILogger>();

            ViewAssemblyLoader.LoadViewAssemblies(logger);

            var assemblies = new List<Assembly>();
            var builder = serviceProviderHolder.ServiceCollection
                .AddMvc()
                .AddApplicationPart(typeof(TestController).Assembly);

            var loaded = AppDomain.CurrentDomain.GetAssemblies()
                .Where(assembly => !assembly.IsDynamic)
                .Where(assembly =>
                    !assembly.GetName().Name!.StartsWith("Microsoft")
                    && assembly.GetName().Name!.Contains("Views", StringComparison.OrdinalIgnoreCase)).ToArray();

            assemblies.AddRange(loaded);

            logger.Information("Found view assemblies: {Count}", assemblies.Count);

            foreach (var assembly in assemblies)
            {
                logger.Information("Found view assembly: {Assembly}", assembly.GetName().Name);

                builder.AddApplicationPart(assembly);
            }
        }
    }
}