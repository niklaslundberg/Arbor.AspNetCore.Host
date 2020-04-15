using System;
using MediatR;

namespace Arbor.AspNetCore.Host.Sample
{
    public class TestNotificationB : INotification
    {
        public Guid Id { get; }

        public TestNotificationB(Guid id) => Id = id;

        public override string ToString() => base.ToString() + " " + Id.ToString();
    }
}