using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Arbor.AspNetCore.Host.Hosting
{
    public static class CustomWebHostExtensions
    {
        public static async Task WaitForShutdownAsync(this IHost host)
        {
            var applicationLifetime = host.Services.GetRequiredService<IHostApplicationLifetime>();

            var waitForStop = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);

            applicationLifetime.ApplicationStopping.Register(obj =>
                {
                    if (obj is TaskCompletionSource<object> tcs)
                    {
                        tcs.TrySetResult(new object());
                    }
                },
                waitForStop);

            await waitForStop.Task.ConfigureAwait(false);

            await host.StopAsync().ConfigureAwait(false);
        }
    }
}