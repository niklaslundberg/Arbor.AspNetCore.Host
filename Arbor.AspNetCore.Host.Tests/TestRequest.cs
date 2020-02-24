using System;
using MediatR;

namespace Arbor.AspNetCore.Host.Tests
{
    public class TestRequest : IRequest<Unit>
    {
        public TestRequest(Guid newGuid) => Id = newGuid;

        public Guid Id { get; set; }

        public override string ToString() => base.ToString() + " " + Id;
    }
}