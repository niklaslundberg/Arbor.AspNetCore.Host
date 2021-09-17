using System.Threading;
using System.Threading.Tasks;

namespace Arbor.AspNetCore.Host
{
    public interface IPreStartModule
    {
        int Order { get; }

        Task RunAsync(CancellationToken cancellationToken);
    }
}