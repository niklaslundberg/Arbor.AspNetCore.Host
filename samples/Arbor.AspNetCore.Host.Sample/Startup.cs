using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Arbor.AspNetCore.Host.Sample
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services) => services.AddControllersWithViews();

        public void Configure(IApplicationBuilder app)
        {
            app.UseDeveloperExceptionPage();

            app.UseMiddleware<TestMiddleware>();

            app.UseRouting();

            app.UseEndpoints(options => options.MapControllers());
        }
    }
}