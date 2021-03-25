using System;
using System.Collections.Generic;

namespace Arbor.AspNetCore.Host.Tests
{
    public sealed class TestTimer : ITimer
    {
        private readonly List<Action> _actions = new();

        public void Tick()
        {
            foreach (var action in _actions)
            {
                action();
            }
        }

        public void Dispose()
        {
            // ignore
        }

        public void Register(Action onTick) => _actions.Add(onTick);
    }
}