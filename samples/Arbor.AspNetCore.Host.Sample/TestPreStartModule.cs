using System.Threading;
using System.Threading.Tasks;
using Arbor.App.Extensions.Application;
using JetBrains.Annotations;
using Serilog;

namespace Arbor.AspNetCore.Host.Sample
{
    [UsedImplicitly]
    public class TestPreStartModule : IPreStartModule
    {
        private readonly ILogger _logger;
        private readonly IApplicationAssemblyResolver _applicationAssemblyResolver;

        public TestPreStartModule(ILogger logger, IApplicationAssemblyResolver applicationAssemblyResolver)
        {
            _logger = logger;
            _applicationAssemblyResolver = applicationAssemblyResolver;
        }

        public Task RunAsync(CancellationToken cancellationToken)
        {
            _logger.Information("Assembly resolver is {AssemblyResolver}", _applicationAssemblyResolver.GetType().FullName);

            return Task.CompletedTask;
        }
    }
}