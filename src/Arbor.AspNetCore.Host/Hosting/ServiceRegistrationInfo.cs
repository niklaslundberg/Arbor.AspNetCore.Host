﻿using System;
using Arbor.App.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Arbor.AspNetCore.Host.Hosting
{
    public sealed class ServiceRegistrationInfo
    {
        private ServiceRegistrationInfo(Type serviceDescriptorServiceType,
            Type serviceDescriptorImplementationType,
            object? serviceDescriptorImplementationInstance,
            ServiceLifetime serviceDescriptorLifetime,
            Func<IServiceProvider, object>? factory,
            Type? module)
        {
            ServiceDescriptorServiceType = serviceDescriptorServiceType;
            ServiceDescriptorImplementationType = serviceDescriptorImplementationType;
            ServiceDescriptorImplementationInstance = serviceDescriptorImplementationInstance;
            ServiceDescriptorLifetime = serviceDescriptorLifetime;
            Factory = factory;
            Module = module;
        }

        public Type ServiceDescriptorServiceType { get; }

        public Type ServiceDescriptorImplementationType { get; }

        public object? ServiceDescriptorImplementationInstance { get; }

        public ServiceLifetime ServiceDescriptorLifetime { get; }

        public Func<IServiceProvider, object>? Factory { get; }

        public Type? Module { get; }

        public static ServiceRegistrationInfo Create(ServiceDescriptor serviceDescriptor)
        {
            Type? module = null;

            if (serviceDescriptor is ExtendedServiceDescriptor extendedServiceDescriptor)
            {
                module = extendedServiceDescriptor.ModuleType;
            }

            return new ServiceRegistrationInfo(serviceDescriptor.ServiceType,
                serviceDescriptor.ImplementationType!,
                serviceDescriptor.ImplementationInstance,
                serviceDescriptor.Lifetime,
                serviceDescriptor.ImplementationFactory,
                module);
        }
    }
}