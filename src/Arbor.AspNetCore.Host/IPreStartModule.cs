using System.Threading;
using System.Threading.Tasks;

namespace Arbor.AspNetCore.Host
{
    public interface IPreStartModule
    {
        Task RunAsync(CancellationToken cancellationToken);

        int Order { get; }
    }
}