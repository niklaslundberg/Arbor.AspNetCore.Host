using System.Collections.Generic;
using System.Threading.Tasks;
using Arbor.Primitives;

namespace Arbor.AspNetCore.Host.NoHttpSample
{
    internal static class Program
    {
        private static async Task<int> Main(string[] args)
        {
            int exitCode = await AppStarter<Startup>.StartAsync(
                args,
#pragma warning disable S1905 // Redundant casts should not be used
                (IReadOnlyDictionary<string, string?>)EnvironmentVariables.GetEnvironmentVariables().Variables);
#pragma warning restore S1905 // Redundant casts should not be used

            return exitCode;
        }
    }
}