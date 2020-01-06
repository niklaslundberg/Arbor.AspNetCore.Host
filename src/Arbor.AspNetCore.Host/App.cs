﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Arbor.App.Extensions;
using Arbor.App.Extensions.Application;
using Arbor.App.Extensions.Configuration;
using Arbor.App.Extensions.DependencyInjection;
using Arbor.App.Extensions.IO;
using Arbor.App.Extensions.Logging;
using Arbor.AspNetCore.Host.Application;
using Arbor.AspNetCore.Host.Configuration;
using Arbor.AspNetCore.Host.Hosting;
using Arbor.KVConfiguration.Core;
using Arbor.KVConfiguration.Urns;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace Arbor.AspNetCore.Host
{
    [UsedImplicitly]
    public sealed class App<T> : IDisposable where T : class
    {
        private readonly Guid _instanceId;
        private bool _disposed;
        private bool _disposing;

        public App(
            [NotNull] IHostBuilder webHost,
            [NotNull] CancellationTokenSource cancellationTokenSource,
            [NotNull] ILogger appLogger,
            MultiSourceKeyValueConfiguration configuration,
            ConfigurationInstanceHolder configurationInstanceHolder)
        {
            CancellationTokenSource = cancellationTokenSource ??
                                      throw new ArgumentNullException(nameof(cancellationTokenSource));
            Logger = appLogger ?? throw new ArgumentNullException(nameof(appLogger));
            Configuration = configuration;
            ConfigurationInstanceHolder = configurationInstanceHolder;
            HostBuilder = webHost ?? throw new ArgumentNullException(nameof(webHost));
            _instanceId = Guid.NewGuid();
            ApplicationName = configuration.GetApplicationName();
            AppInstance = ApplicationName + " " + _instanceId;
        }

        public string? ApplicationName { get; }

        public string AppInstance { get; }

        public CancellationTokenSource CancellationTokenSource { get; }

        public ILogger Logger { get; private set; }

        public MultiSourceKeyValueConfiguration Configuration { get; private set; }

        public ConfigurationInstanceHolder ConfigurationInstanceHolder { get; }

        [PublicAPI]
        public IHostBuilder HostBuilder { get; private set; }

        public IHost? Host { get; private set; }

        public void Dispose()
        {
            if (_disposed || _disposing)
            {
                return;
            }

            if (!_disposing)
            {
                Stop();
                _disposing = true;
            }

            Logger?.Debug("Disposing application {Application} {Instance}",
                ApplicationName,
                _instanceId);
            Logger?.Verbose("Disposing web host {Application} {Instance}",
                ApplicationName,
                _instanceId);
            Host?.SafeDispose();
            Logger?.Verbose("Disposing Application root scope {Application} {Instance}",
                ApplicationName,
                _instanceId);
            Logger?.Verbose("Disposing configuration {Application} {Instance}",
                ApplicationName,
                _instanceId);
            Configuration?.SafeDispose();

            Logger?.Debug("Application disposal complete, disposing logging {Application} {Instance}",
                ApplicationName,
                _instanceId);

            if (Logger is IDisposable disposable)
            {
                Logger?.Verbose("Disposing Logger {Application} {Instance}",
                    ApplicationName,
                    _instanceId);
                disposable.SafeDispose();
            }
            else
            {
                Logger?.Debug("Logger is not disposable {Application} {Instance}",
                    ApplicationName,
                    _instanceId);
            }

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Configuration = null;
            Logger = null;
            Host = null;
            HostBuilder = null;
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            _disposed = true;
            _disposing = false;
        }

        private static Task<App<T>> BuildAppAsync(
            CancellationTokenSource cancellationTokenSource,
            string[] commandLineArgs,
            IReadOnlyDictionary<string, string> environmentVariables,
            params object[] instances)
        {
            var scanAssemblies = ApplicationAssemblies.FilteredAssemblies().ToArray();

            MultiSourceKeyValueConfiguration startupConfiguration =
                ConfigurationInitialization.InitializeStartupConfiguration(
                    commandLineArgs,
                    environmentVariables,
                    scanAssemblies);

            ConfigurationInstanceHolder configurationInstanceHolder =
                GetConfigurationRegistrations(startupConfiguration, scanAssemblies);

            configurationInstanceHolder.AddInstance(configurationInstanceHolder);

            foreach (object instance in instances.Where(item => item is {}))
            {
                configurationInstanceHolder.AddInstance(instance);
            }

            var loggingLevelSwitch = new LoggingLevelSwitch();
            configurationInstanceHolder.AddInstance(loggingLevelSwitch);
            configurationInstanceHolder.AddInstance(cancellationTokenSource);

            ApplicationPaths paths =
                configurationInstanceHolder.GetInstances<ApplicationPaths>().SingleOrDefault().Value ??
                new ApplicationPaths();

            AppPathHelper.SetApplicationPaths(paths, commandLineArgs);

            if (paths.BasePath is null)
            {
                throw new InvalidOperationException("Base path is not set");
            }

            var startupLoggerConfigurationHandlers = ApplicationAssemblies.FilteredAssemblies()
                .GetLoadablePublicConcreteTypesImplementing<IStartupLoggerConfigurationHandler>()
                .Select(type => configurationInstanceHolder.Create(type) as IStartupLoggerConfigurationHandler)
                .Where(item => item != null)
                .ToImmutableArray();

            var startupLogger =
                SerilogApiInitialization.InitializeStartupLogging(
                    file => GetBaseDirectoryFile(paths.BasePath, file),
                    environmentVariables,
                    startupLoggerConfigurationHandlers);

            MultiSourceKeyValueConfiguration appConfiguration;
            try
            {
                appConfiguration =
                    ConfigurationInitialization.InitializeConfiguration(
                        file => GetBaseDirectoryFile(paths.BasePath, file),
                        paths.ContentBasePath,
                        scanAssemblies,
                        commandLineArgs,
                        environmentVariables,
                        startupConfiguration);

                configurationInstanceHolder.AddInstance(appConfiguration);
            }
            catch (Exception ex) when (!ex.IsFatal())
            {
                startupLogger.Fatal(ex, "Could not initialize configuration");
                throw;
            }

            ConfigurationInstanceHolder appInstanceHolder =
                GetConfigurationRegistrations(appConfiguration, scanAssemblies);

            foreach (var registeredType in appInstanceHolder.RegisteredTypes)
            {
                var appInstances = appInstanceHolder.GetInstances(registeredType);

                foreach (var appInstance in appInstances)
                {
                    if (configurationInstanceHolder.Get(registeredType, appInstance.Key) is var item && item is {})
                    {
                        configurationInstanceHolder.TryRemove(appInstance.Key, appInstance.Value.GetType(),
                            out object? _);
                    }

                    configurationInstanceHolder.Add(new NamedInstance<object>(appInstance.Value, appInstance.Key));
                }
            }

            startupLogger.Information("Configuration done using chain {Chain}",
                appConfiguration.SourceChain);

            startupLogger.Verbose("Configuration values {KeyValues}",
                appConfiguration.AllValues.Select(pair =>
                    $"\"{pair.Key}\": \"{pair.Value.MakeAnonymous(pair.Key, $"{ApplicationStringExtensions.DefaultAnonymousKeyWords.ToArray()}\"")}"));

            App<T> app;
            try
            {
                startupLogger.Verbose("Trying to create application");

                TempPathHelper.SetTempPath(appConfiguration, startupLogger);

                SetLoggingLevelSwitch(loggingLevelSwitch, appConfiguration);

                startupLogger.Verbose("Log level: {Level}", loggingLevelSwitch.MinimumLevel);

                IEnumerable<ILoggerConfigurationHandler> loggerConfigurationHandlers = ApplicationAssemblies.FilteredAssemblies()
                    .GetLoadablePublicConcreteTypesImplementing<ILoggerConfigurationHandler>()
                    .Select(type => configurationInstanceHolder.Create(type) as ILoggerConfigurationHandler)
                    .Where(item => item != null)!;

                ILogger appLogger;
                try
                {
                    startupLogger.Verbose("Creating application logger");
                    appLogger =
                        SerilogApiInitialization.InitializeAppLogging(
                            appConfiguration,
                            startupLogger,
                            loggerConfigurationHandlers,
                            loggingLevelSwitch);

                    configurationInstanceHolder.AddInstance(appLogger);

                    startupLogger.Verbose("Application logger created");
                    appLogger.Verbose("Application logger is created");
                }
                catch (Exception ex)
                {
                    appLogger = startupLogger;
                    startupLogger.Error(ex, "Could not create app logger");
                }

                LogCommandLineArgs(commandLineArgs, appLogger);

                var environmentConfiguration = new EnvironmentConfiguration
                {
                    ApplicationBasePath = paths.BasePath,
                    ContentBasePath = paths.ContentBasePath,
                    CommandLineArgs = commandLineArgs.ToImmutableArray(),
                    EnvironmentName = environmentVariables.ValueOrDefault(ApplicationConstants.AspNetEnvironment)
                };

                var serviceCollection = new ServiceCollection();

                try
                {
                    configurationInstanceHolder.AddInstance(environmentConfiguration);

                    IReadOnlyList<IModule> modules =
                        GetConfigurationModules(configurationInstanceHolder, scanAssemblies);

                    ModuleRegistration.RegisterModules(modules, serviceCollection, appLogger);
                }
                catch (Exception ex)
                {
                    appLogger.Fatal(ex, "Could not initialize container registrations");

                    Thread.Sleep(TimeSpan.FromMilliseconds(2500));
                    throw;
                }

                configurationInstanceHolder.AddInstance(new ApplicationEnvironmentConfigurator(appConfiguration));

                EnvironmentConfigurator.ConfigureEnvironment(configurationInstanceHolder);

                foreach (Type registeredType in configurationInstanceHolder.RegisteredTypes)
                {
                    var interfaces = registeredType.GetInterfaces();
                    var all = configurationInstanceHolder.GetInstances(registeredType);

                    if (all.Count > 1)
                    {
                        foreach (var item in all)
                        {
                            foreach (var @interface in interfaces)
                            {
                                serviceCollection.AddSingleton(@interface, context => item.Value);
                            }

                            var serviceType = item.Value.GetType();
                            serviceCollection.AddSingleton(serviceType, item.Value);
                        }
                    }
                    else
                    {
                        object instance = all.Single().Value;

                        foreach (var @interface in interfaces)
                        {
                            serviceCollection.AddSingleton(@interface, context => instance);
                        }

                        var serviceType = instance.GetType();
                        serviceCollection.AddSingleton(serviceType, instance);
                    }
                }

                var serviceProviderModules =
                    scanAssemblies.GetLoadablePublicConcreteTypesImplementing<IServiceProviderModule>();

                var serviceProviderHolder = new ServiceProviderHolder(serviceCollection.BuildServiceProvider(),
                    serviceCollection);

                foreach (var module in serviceProviderModules)
                {
                    try
                    {
                        if (Activator.CreateInstance(module) is IServiceProviderModule instance)
                        {
                            instance.Register(serviceProviderHolder);
                        }
                    }
                    catch (Exception ex) when (!ex.IsFatal())
                    {
                        appLogger.Error(ex, "Could not create instance of type {Type}", module.FullName);
                    }
                }

                var webHostBuilder = CustomWebHostBuilder<T>.GetWebHostBuilder(environmentConfiguration,
                    appConfiguration,
                    serviceProviderHolder,
                    appLogger,
                    commandLineArgs);

                app = new App<T>(
                    webHostBuilder,
                    cancellationTokenSource,
                    appLogger,
                    appConfiguration,
                    configurationInstanceHolder);
            }
            catch (Exception ex)
            {
                startupLogger.Fatal(ex, "Startup error");
                Thread.Sleep(TimeSpan.FromMilliseconds(2500));
                throw;
            }
            finally
            {
                startupLogger.Information("Closing startup logger");
                startupLogger.SafeDispose();
                Thread.Sleep(TimeSpan.FromMilliseconds(2500));
            }

            return Task.FromResult(app);
        }

        private static ConfigurationInstanceHolder GetConfigurationRegistrations(
            MultiSourceKeyValueConfiguration startupConfiguration,
            Assembly[] scanAssemblies)
        {
            ConfigurationRegistrations startupRegistrations = startupConfiguration.ScanRegistrations(scanAssemblies);

            if (startupRegistrations.UrnTypeRegistrations
                .Any(registrationErrors => !registrationErrors.ConfigurationRegistrationErrors.IsDefaultOrEmpty))
            {
                var errors = startupRegistrations.UrnTypeRegistrations
                    .Where(registrationErrors => registrationErrors.ConfigurationRegistrationErrors.Length > 0)
                    .SelectMany(registrationErrors => registrationErrors.ConfigurationRegistrationErrors)
                    .Select(registrationError => registrationError.ErrorMessage);

                throw new InvalidOperationException(
                    $"Error {string.Join(Environment.NewLine, errors)}"); // review exception
            }

            ConfigurationInstanceHolder configurationInstanceHolder = startupRegistrations.CreateHolder();
            return configurationInstanceHolder;
        }

        private static void LogCommandLineArgs(string[] commandLineArgs, ILogger appLogger)
        {
            if (commandLineArgs.Length > 0)
            {
                appLogger.Debug("Application started with command line args, {Args}",
                    commandLineArgs);
            }
            else if (appLogger.IsEnabled(LogEventLevel.Verbose))
            {
                appLogger.Verbose("Application started with no command line args");
            }
        }

        private static void SetLoggingLevelSwitch(LoggingLevelSwitch loggingLevelSwitch,
            MultiSourceKeyValueConfiguration configuration)
        {
            var defaultLevel = configuration[ConfigurationConstants.LogLevel]
                .ParseOrDefault(loggingLevelSwitch.MinimumLevel);

            loggingLevelSwitch.MinimumLevel = defaultLevel;
        }


        private static ImmutableArray<IModule> GetConfigurationModules(
            ConfigurationInstanceHolder holder,
            Assembly[] scanAssemblies)
        {
            var moduleTypes = scanAssemblies
                .SelectMany(assembly => assembly.GetLoadableTypes())
                .Where(type => type.IsPublicConcreteTypeImplementing<IModule>())
                .ToImmutableArray();

            ImmutableArray<IModule> modules = moduleTypes
                .Select(moduleType =>
                    holder.Create(moduleType) as IModule)
                .Where(instance => instance is {})
                .ToImmutableArray()!;

            return modules;
        }

        private static string GetBaseDirectoryFile(string basePath, string? fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return basePath;
            }

            return Path.Combine(basePath, fileName);
        }

        public static async Task<App<T>> CreateAsync(
            CancellationTokenSource cancellationTokenSource,
            string[] args,
            IReadOnlyDictionary<string, string> environmentVariables,
            params object[] instances)
        {
            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            try
            {
                var app = await BuildAppAsync(
                    cancellationTokenSource,
                    args,
                    environmentVariables,
                    instances);

                return app;
            }
            catch (Exception ex) when (!ex.IsFatal())
            {
                TempLogger.WriteLine("Error in startup, " + ex);
                throw;
            }
        }

        [PublicAPI]
        public void Stop()
        {
            if (!CancellationTokenSource.IsCancellationRequested)
            {
                try
                {
                    CancellationTokenSource.Cancel();
                }
                catch (ObjectDisposedException)
                {
                }
            }
        }

        public async Task<int> RunAsync([NotNull] params string[] args)
        {
            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            bool runAsService = args.Any(arg =>
                arg.Equals(ApplicationConstants.RunAsService, StringComparison.OrdinalIgnoreCase));

            try
            {
                if (runAsService)
                {
                    HostBuilder.UseWindowsService();
                }

                Host = HostBuilder.Build();
            }
            catch (Exception ex) when (!ex.IsFatal())
            {
                Logger.Fatal(ex, "Could not build web host {Application}", AppInstance);
                throw new InvalidOperationException($"Could not build web host in {AppInstance}", ex);
            }

            if (runAsService)
            {
                Logger.Information("Starting {AppInstance} as a Windows Service", AppInstance);

                try
                {
                    await Host.WaitForShutdownAsync();
                }
                catch (Exception ex) when (!ex.IsFatal())
                {
                    Logger.Fatal(ex, "Could not start web host as a Windows service, {AppInstance}", AppInstance);
                    throw new InvalidOperationException(
                        $"Could not start web host as a Windows service, configuration, {Configuration?.SourceChain} {AppInstance} ",
                        ex);
                }
            }
            else
            {
                Logger.Information("Starting as a Console Application, {AppInstance}", AppInstance);

                try
                {
                    await Host.StartAsync(CancellationTokenSource.Token);
                }
                catch (Exception ex) when (!ex.IsFatal())
                {
                    Logger.Fatal(ex, "Could not start web host, {AppInstance}", AppInstance);
                    throw new InvalidOperationException(
                        $"Could not start web host, configuration {Configuration?.SourceChain} {AppInstance}",
                        ex);
                }
            }

            return 0;
        }
    }
}