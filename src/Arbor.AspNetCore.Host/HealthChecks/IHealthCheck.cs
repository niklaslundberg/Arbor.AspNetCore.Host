using System.Threading;
using System.Threading.Tasks;

namespace Arbor.AspNetCore.Host.HealthChecks
{
    public interface IHealthCheck
    {
        int TimeoutInSeconds { get; }

        string Description { get; }

        Task<HealthCheckResult> CheckHealthAsync(CancellationToken cancellationToken);
    }
}