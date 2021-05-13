using Microsoft.Extensions.Hosting;

namespace Arbor.AspNetCore.Host.Startup
{
    public interface IStartupTask : IHostedService
    {
        bool IsCompleted { get; }
    }
}