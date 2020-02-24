using System;
using MediatR;

namespace Arbor.AspNetCore.Host.Sample
{
    public class TestNotificationA : INotification
    {
        public Guid Id { get; }

        public TestNotificationA(Guid id)
        {
            Id = id;
        }


        public override string ToString() => base.ToString() + " " + Id.ToString();
    }
}