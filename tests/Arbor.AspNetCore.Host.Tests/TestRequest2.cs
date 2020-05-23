using System;
using MediatR;

namespace Arbor.AspNetCore.Host.Tests
{
    public class TestRequest2 : IRequest<Unit>
    {
        public TestRequest2(Guid newGuid) => Id = newGuid;

        public Guid Id { get; set; }

        public override string ToString() => base.ToString() + " " + Id;
    }
}