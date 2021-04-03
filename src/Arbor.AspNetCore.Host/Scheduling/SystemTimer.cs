using System;
using System.Collections.Generic;
using System.Threading;

namespace Arbor.AspNetCore.Host.Scheduling
{
    public sealed class SystemTimer : ITimer
    {
        private readonly List<Action> _actions = new();

        private Timer? _timer;
        public SystemTimer() => _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(10));

        public void Register(Action onTick)
        {
            if (_timer is null)
            {
                throw new ObjectDisposedException(GetType().Name);
            }

            _actions.Add(onTick);
        }

        public void Dispose()
        {
            _timer?.Dispose();
            _timer = null;
        }

        private void DoWork(object? state)
        {
            if (_actions.Count == 0)
            {
                return;
            }

            foreach (var action in _actions)
            {
                action.Invoke();
            }
        }
    }
}