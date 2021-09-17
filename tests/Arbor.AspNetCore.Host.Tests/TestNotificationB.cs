using System;

namespace Arbor.AspNetCore.Host.Tests
{
    public class TestNotificationB : IIdNotification
    {
        public TestNotificationB(Guid id) => Id = id;

        public Guid Id { get; }

        public override string ToString() => base.ToString() + " " + Id;
    }
}