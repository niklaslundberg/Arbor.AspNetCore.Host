using System;
using MediatR;

namespace Arbor.AspNetCore.Host.Sample
{
    public class TestRequest : IRequest<Unit>
    {
        public TestRequest(Guid newGuid) => Id = newGuid;

        public override string ToString() => base.ToString() + " " + Id.ToString();

        public Guid Id { get; set; }
    }
}