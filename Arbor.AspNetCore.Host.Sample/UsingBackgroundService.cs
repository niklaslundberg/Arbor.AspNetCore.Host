using System;
using Serilog;

namespace Arbor.AspNetCore.Host.Sample
{
    public class UsingBackgroundService
    {
        private readonly BackgroundTestHandler _backgroundTestHandler;
        private Guid _id;
        private ILogger _logger;

        public UsingBackgroundService(BackgroundTestHandler backgroundTestHandler, ILogger logger)
        {
            _backgroundTestHandler = backgroundTestHandler;
            _id = Guid.NewGuid();
            _logger = logger;
            _logger.Information("Created using test handler {Id}", ToString());
        }

        public override string ToString() => base.ToString() + " " + _id;

        public void CallOnBackgrounService()
        {
            _backgroundTestHandler.Test();
        }
    }
}