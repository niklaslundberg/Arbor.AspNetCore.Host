using System;

namespace Arbor.AspNetCore.Host.Scheduling
{
    public interface ITimer : IDisposable
    {
        void Register(Action onTick);
    }
}