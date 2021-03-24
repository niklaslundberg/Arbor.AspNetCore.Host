using System;

namespace Arbor.AspNetCore.Host
{
    public interface ITimer : IDisposable
    {
        void Register(Action onTick);
    }
}