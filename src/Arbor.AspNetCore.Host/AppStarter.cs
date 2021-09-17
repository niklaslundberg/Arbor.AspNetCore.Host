using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Arbor.App.Extensions.Application;
using Arbor.App.Extensions.Cli;
using Arbor.App.Extensions.Configuration;
using Arbor.App.Extensions.Logging;
using Arbor.KVConfiguration.Core.Extensions.BoolExtensions;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace Arbor.AspNetCore.Host
{
    public static class AppStarter<T> where T : class
    {
        public static async Task<int> StartAsync(string[] args,
            IReadOnlyDictionary<string, string> environmentVariables,
            CancellationTokenSource? cancellationTokenSource = null,
            IReadOnlyCollection<Assembly>? assemblies = null,
            object[]? instances = null)
        {
            try
            {
                args ??= Array.Empty<string>();

                if (args.Length > 0)
                {
                    TempLogger.WriteLine("Started with arguments:");

                    foreach (string arg in args)
                    {
                        TempLogger.WriteLine(arg);
                    }
                }

                bool shouldDisposeCancellationToken = cancellationTokenSource is null;

                if (cancellationTokenSource is null &&
                    int.TryParse(environmentVariables.GetValueOrDefault(ConfigurationConstants.RestartTimeInSeconds),
                        out int intervalInSeconds) &&
                    intervalInSeconds > 0)
                {
                    cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(intervalInSeconds));
                }
                else
                {
                    intervalInSeconds = 0;
                }

                cancellationTokenSource ??= new CancellationTokenSource();

                assemblies ??= ApplicationAssemblies.FilteredAssemblies();

                instances ??= Array.Empty<object>();

                try
                {
                    cancellationTokenSource.Token.Register(() =>
                        TempLogger.WriteLine("App cancellation token triggered"));

                    using var app = await App<T>
                                         .CreateAsync(cancellationTokenSource,
                                              args,
                                              environmentVariables,
                                              assemblies,
                                              instances).ConfigureAwait(false);

                    bool runAsService = app.Configuration.ValueOrDefault(ApplicationConstants.RunAsService) &&
                                        !Debugger.IsAttached;

                    app.Logger.Information("Starting application {Application}", app.AppInstance);

                    if (intervalInSeconds > 0)
                    {
                        app.Logger.Debug("Restart time is set to {RestartIntervalInSeconds} seconds for {App}",
                            intervalInSeconds,
                            app.AppInstance);
                    }
                    else if (app.Logger.IsEnabled(LogEventLevel.Verbose))
                    {
                        app.Logger.Verbose("Restart time is disabled");
                    }

                    string[] runArgs;

                    if (!args.Contains(ApplicationConstants.RunAsService) && runAsService)
                    {
                        runArgs = args.Concat(new[] { ApplicationConstants.RunAsService }).ToArray();
                    }
                    else
                    {
                        runArgs = args;
                    }

                    await app.RunAsync(runArgs).ConfigureAwait(false);

                    if (!runAsService)
                    {
                        app.Logger.Debug("Started {App}, waiting for web host shutdown", app.AppInstance);

                        await app.Host.WaitForShutdownAsync(cancellationTokenSource.Token).ConfigureAwait(false);
                    }

                    app.Logger.Information("Stopping application {Application}", app.AppInstance);
                }
                finally
                {
                    if (shouldDisposeCancellationToken)
                    {
                        cancellationTokenSource.Dispose();
                    }
                }

                if (int.TryParse(environmentVariables.GetValueOrDefault(ConfigurationConstants.ShutdownTimeInSeconds),
                        out int shutDownTimeInSeconds) &&
                    shutDownTimeInSeconds > 0)
                {
                    await Task.Delay(TimeSpan.FromSeconds(shutDownTimeInSeconds), CancellationToken.None)
                              .ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(2000)).ConfigureAwait(false);

                string? exceptionLogDirectory = args?.ParseParameter("exceptionDir");

                string logDirectory = exceptionLogDirectory ?? AppDomain.CurrentDomain.BaseDirectory!;

                string fatalLogFile = Path.Combine(logDirectory, "Fatal.log");

                var loggerConfiguration = new LoggerConfiguration().WriteTo.File(fatalLogFile,
                    flushToDiskInterval: TimeSpan.FromMilliseconds(50));

                if (environmentVariables.TryGetValue(LoggingConstants.SeqStartupUrl, out string? url) &&
                    Uri.TryCreate(url, UriKind.Absolute, out var uri))
                {
                    loggerConfiguration = loggerConfiguration.WriteTo.Seq(uri.AbsoluteUri);
                }

                var logger = loggerConfiguration.MinimumLevel.Verbose().CreateLogger();

                using (logger)
                {
                    logger.Fatal(ex, "Could not start application");
                    TempLogger.FlushWith(logger);

                    await Task.Delay(TimeSpan.FromMilliseconds(1000)).ConfigureAwait(false);
                }

                string exceptionLogFile = Path.Combine(logDirectory, "Exception.log");

                await File.WriteAllTextAsync(exceptionLogFile, ex.ToString(), Encoding.UTF8).ConfigureAwait(false);

                await Task.Delay(TimeSpan.FromMilliseconds(3000)).ConfigureAwait(false);

                return 1;
            }

            return 0;
        }
    }
}