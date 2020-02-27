using Microsoft.Extensions.DependencyInjection;

namespace Arbor.AspNetCore.Host
{
    public static class ServiceDescriptorExtensions
    {
        public static string GetDescription(this ServiceDescriptor descriptor) => descriptor.ToString(); // TODO
    }
}