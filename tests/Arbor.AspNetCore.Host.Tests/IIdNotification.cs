using System;
using MediatR;

namespace Arbor.AspNetCore.Host.Tests
{
    public interface IIdNotification : INotification
    {
        Guid Id { get; }
    }
}