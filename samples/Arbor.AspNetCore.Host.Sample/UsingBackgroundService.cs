using System;
using Serilog;

namespace Arbor.AspNetCore.Host.Sample
{
    public sealed class UsingBackgroundService
    {
        private readonly BackgroundTestHandler _backgroundTestHandler;
        private readonly Guid _id;

        public UsingBackgroundService(BackgroundTestHandler backgroundTestHandler, ILogger logger)
        {
            _backgroundTestHandler = backgroundTestHandler;
            _id = Guid.NewGuid();
            logger.Information("Created using test handler {Id}", ToString());
        }

        public override string ToString() => base.ToString() + " " + _id;

        public void CallOnBackgrounService() => _backgroundTestHandler.Test();
    }
}