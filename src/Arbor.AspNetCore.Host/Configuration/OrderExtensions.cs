using System.Reflection;
using Arbor.App.Extensions.Configuration;

namespace Arbor.AspNetCore.Host.Configuration
{
    public static class OrderExtensions
    {
        public static int GetRegistrationOrder(this object? instance, int defaultOrder)
        {
            if (instance is null)
            {
                return defaultOrder;
            }

            var attribute = instance.GetType().GetCustomAttribute<RegistrationOrderAttribute>();

            if (attribute is null)
            {
                return defaultOrder;
            }

            return attribute.Order;
        }
    }
}