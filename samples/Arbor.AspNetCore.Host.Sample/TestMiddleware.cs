using System.Collections.Generic;
using System.Threading.Tasks;
using Arbor.KVConfiguration.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Arbor.AspNetCore.Host.Sample
{
    public class TestMiddleware
    {
        private readonly RequestDelegate _next;

        public TestMiddleware(RequestDelegate next) => _next = next;

        public async Task InvokeAsync(HttpContext context)
        {
            var logger = context.RequestServices.GetService<ILogger>();

            var services = context.RequestServices.GetService<IEnumerable<UrnBoundExample>>();

            foreach (var urnBoundExample in services)
            {
                logger.Debug("Found instance {Name}", urnBoundExample.Name);
            }

            var example = context.RequestServices.GetService<IKeyValueConfiguration>();


            logger.Debug("Configuration is {Type}", example.GetType().FullName);

            await _next(context);
        }
    }
}