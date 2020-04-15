using System;

namespace Arbor.AspNetCore.Host.Tests
{
    public class TestNotificationA : IIdNotification
    {
        public Guid Id { get; }

        public TestNotificationA(Guid id) => Id = id;


        public override string ToString() => base.ToString() + " " + Id.ToString();
    }

}