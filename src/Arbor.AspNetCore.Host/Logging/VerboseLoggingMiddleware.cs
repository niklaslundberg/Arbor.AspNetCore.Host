﻿using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Serilog;
using Serilog.Events;

namespace Arbor.AspNetCore.Host.Logging
{
    public class VerboseLoggingMiddleware
    {
        private readonly ILogger _logger;
        private readonly RequestDelegate _next;

        public VerboseLoggingMiddleware(ILogger logger, RequestDelegate next)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _next = next;
        }

        [UsedImplicitly]
        public async Task InvokeAsync(HttpContext context)
        {
            bool loggingEnabled = _logger.IsEnabled(LogEventLevel.Verbose);

            string? commonRequestInfo = null;

            if (loggingEnabled)
            {
                commonRequestInfo =
                    $"{context.Request.GetDisplayUrl()} from remote IP {context.Connection.RemoteIpAddress}";

                _logger.Verbose("Starting request {RequestInfo}", commonRequestInfo);
            }

            await _next.Invoke(context).ConfigureAwait(false);

            if (loggingEnabled)
            {
                _logger.Verbose("Ending request {RequestInfo}, status code {StatusCode}",
                    commonRequestInfo,
                    context.Response.StatusCode);
            }
        }
    }
}