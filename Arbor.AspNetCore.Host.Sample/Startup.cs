using System.Globalization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Arbor.AspNetCore.Host.Sample
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseMiddleware<TestMiddleware>();

            app.UseRouting();

            app.UseEndpoints(options => options.MapControllers());
        }
    }
}