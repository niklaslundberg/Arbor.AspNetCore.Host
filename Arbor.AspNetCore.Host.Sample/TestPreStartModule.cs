using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Serilog;

namespace Arbor.AspNetCore.Host.Sample
{
    [UsedImplicitly]
    public class TestPreStartModule : IPreStartModule
    {
        private readonly ILogger _logger;

        public TestPreStartModule(ILogger logger) => _logger = logger;

        public Task RunAsync(CancellationToken cancellationToken)
        {
            _logger.Information("Test");

            return Task.CompletedTask;
        }
    }
}