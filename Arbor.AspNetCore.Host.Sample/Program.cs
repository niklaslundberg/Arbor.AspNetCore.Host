using System.Threading.Tasks;
using Arbor.Primitives;

namespace Arbor.AspNetCore.Host.Sample
{
    internal class Program
    {
        private static async Task<int> Main(string[] args)
        {
            int exitCode = await AppStarter<Startup>.StartAsync(
                args,
                EnvironmentVariables.GetEnvironmentVariables().Variables);

            return exitCode;
        }
    }
}