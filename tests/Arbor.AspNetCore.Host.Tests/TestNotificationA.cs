using System;

namespace Arbor.AspNetCore.Host.Tests
{
    public class TestNotificationA : IIdNotification
    {
        public TestNotificationA(Guid id) => Id = id;

        public Guid Id { get; }

        public override string ToString() => base.ToString() + " " + Id;
    }
}