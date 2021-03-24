using System;
using System.Collections.Generic;

namespace Arbor.AspNetCore.Host.Tests
{
    public class TestTimer : ITimer
    {
        private List<Action> _actions = new();

        public void Tick()
        {
            foreach (var action in _actions)
            {
                action();
            }
        }

        public void Dispose()
        {

        }

        public void Register(Action onTick)
        {
            _actions.Add(onTick);
        }
    }
}