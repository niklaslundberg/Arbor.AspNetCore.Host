using Microsoft.Extensions.DependencyInjection;

namespace Arbor.AspNetCore.Host
{
    public static class ServiceDescriptorExtensions
    {
        public static string GetDescription(this ServiceDescriptor descriptor) =>
            $"{descriptor.ServiceType.FullName} {descriptor.ImplementationInstance ?? "(function)"} {descriptor.Lifetime}";
    }
}