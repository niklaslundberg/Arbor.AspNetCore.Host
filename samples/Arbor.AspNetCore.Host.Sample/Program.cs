using Arbor.AspNetCore.Host;
using Arbor.AspNetCore.Host.Sample;
using Arbor.Primitives;

await AppStarter<Startup>.StartAsync(
    args,
    EnvironmentVariables.GetEnvironmentVariables().Variables);