using System;

namespace Arbor.AspNetCore.Host.Tests
{
    public class TestNotificationB : IIdNotification
    {
        public Guid Id { get; }

        public TestNotificationB(Guid id) => Id = id;

        public override string ToString() => base.ToString() + " " + Id.ToString();
    }
}