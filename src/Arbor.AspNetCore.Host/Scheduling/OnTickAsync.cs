using System;
using System.Threading.Tasks;

namespace Arbor.AspNetCore.Host.Scheduling
{
    public delegate Task OnTickAsync(DateTimeOffset now);
}