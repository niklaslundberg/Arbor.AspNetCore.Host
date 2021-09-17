using System;

namespace Arbor.AspNetCore.Host.Validation
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Parameter)]
    public sealed class NoValidationAttribute : Attribute
    {
    }
}