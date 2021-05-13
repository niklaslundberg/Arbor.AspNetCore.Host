using JetBrains.Annotations;

namespace Arbor.AspNetCore.Host.HealthChecks
{
    public class HealthCheckResult
    {
        public HealthCheckResult(bool succeeded) => Succeeded = succeeded;

        [PublicAPI]
        public bool Succeeded { get; }
    }
}