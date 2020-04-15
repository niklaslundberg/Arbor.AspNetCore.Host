﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Arbor.App.Extensions;
using Arbor.App.Extensions.Application;
using Arbor.KVConfiguration.Core;
using Arbor.KVConfiguration.Microsoft.Extensions.Configuration.Urns;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog.Extensions.Logging;
using ILogger = Serilog.ILogger;

namespace Arbor.AspNetCore.Host.Hosting
{
    public static class CustomHostBuilder<T> where T : class
    {
        public static IHostBuilder GetHostBuilder(EnvironmentConfiguration environmentConfiguration,
            IKeyValueConfiguration configuration,
            ServiceProviderHolder serviceProviderHolder,
            ILogger logger,
            string[] commandLineArgs,
            Action<IServiceCollection>? onRegistration = null)
        {
            string contentRoot = environmentConfiguration?.ContentBasePath ?? Directory.GetCurrentDirectory();

            logger.Debug("Using content root {ContentRoot}", contentRoot);

            var kestrelServerOptions = new List<KestrelServerOptions>();

            IHostBuilder hostBuilder = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(commandLineArgs);

            hostBuilder
                .ConfigureLogging((context, builder) => { builder.AddProvider(new SerilogLoggerProvider(logger)); })
                .ConfigureServices(services =>
                {
                    foreach (var serviceDescriptor in serviceProviderHolder.ServiceCollection)
                    {
                        logger.Verbose("Adding service descriptor {Descriptor}", serviceDescriptor.GetDescription());
                        services.Add(serviceDescriptor);
                    }

                    services.AddSingleton(environmentConfiguration);
                    services.AddHttpClient();

                    onRegistration?.Invoke(services);
                })
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddKeyValueConfigurationSource(configuration);

                    hostingContext.Configuration =
                        new ConfigurationWrapper((IConfigurationRoot)hostingContext.Configuration,
                            serviceProviderHolder);

                    string applicationName = configuration[HostConfigurationConstants.ApplicationName];

                    if (!string.IsNullOrWhiteSpace(applicationName))
                    {
                        hostingContext.HostingEnvironment.ApplicationName = applicationName;
                    }
                });

            if (environmentConfiguration?.HttpEnabled ?? true)
            {
                hostBuilder.ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                        .UseKestrel(options =>
                        {
                            if (kestrelServerOptions.Contains(options))
                            {
                                logger.Debug("Kestrel options has already been configured");
                                return;
                            }

                            if (environmentConfiguration is {})
                            {
                                if (environmentConfiguration.UseExplicitPorts)
                                {
                                    logger.Debug("Environment configuration is set to use explicit ports");

                                    if (environmentConfiguration.HttpPort.HasValue)
                                    {
                                        logger.Information("Listening on http port {Port}",
                                            environmentConfiguration.HttpPort.Value);

                                        options.Listen(IPAddress.Any,
                                            environmentConfiguration.HttpPort.Value);
                                    }

                                    if (environmentConfiguration.HttpsPort.HasValue
                                        && environmentConfiguration.PfxFile.HasValue()
                                        && environmentConfiguration.PfxPassword.HasValue())
                                    {
                                        logger.Information("Listening on https port {Port}",
                                            environmentConfiguration.HttpsPort.Value);

                                        options.Listen(IPAddress.Any,
                                            environmentConfiguration.HttpsPort.Value,
                                            listenOptions =>
                                            {
                                                listenOptions.UseHttps(environmentConfiguration.PfxFile,
                                                    environmentConfiguration.PfxPassword);
                                            });
                                    }
                                }
                            }
                            else
                            {
                                logger.Debug("Environment configuration is not set, using defaults");
                            }

                            kestrelServerOptions.Add(options);
                        })
                        .UseContentRoot(contentRoot)
                        .ConfigureAppConfiguration((hostingContext, config) => { config.AddEnvironmentVariables(); })
                        .UseIISIntegration()
                        .UseDefaultServiceProvider((context, options) =>
                        {
                            options.ValidateScopes = context.HostingEnvironment.IsDevelopment();
                        })
                        .UseStartup<T>();

                    if (environmentConfiguration?.EnvironmentName is {})
                    {
                        webBuilder.UseEnvironment(environmentConfiguration.EnvironmentName);
                    }
                });
            }
            else
            {
                logger.Debug("Environment has http-enabled explicitly set to false, http server will not be available");
            }

            var webHostBuilderWrapper = new HostBuilderWrapper(hostBuilder);

            return webHostBuilderWrapper;
        }
    }
}