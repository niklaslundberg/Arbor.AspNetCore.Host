using System;
using System.Threading.Tasks;

namespace Arbor.AspNetCore.Host
{
    public delegate Task OnTickAsync(DateTimeOffset now);
}