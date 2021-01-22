using System;
using MediatR;

namespace Arbor.AspNetCore.Host.Sample
{
    public class TestNotificationA : INotification
    {
        public TestNotificationA(Guid id) => Id = id;
        public Guid Id { get; }


        public override string ToString() => base.ToString() + " " + Id;
    }
}