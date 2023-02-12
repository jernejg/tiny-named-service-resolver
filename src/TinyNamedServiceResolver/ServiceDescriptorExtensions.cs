using System;
using Microsoft.Extensions.DependencyInjection;

namespace TinyNamedServiceResolver;

internal static class ServiceDescriptorExtensions
{
    public static ServiceDescriptor WithImplementationFactory(this ServiceDescriptor descriptor,
        Func<IServiceProvider, object> implementationFactory) =>
        new ServiceDescriptor(descriptor.ServiceType, implementationFactory, descriptor.Lifetime);

    public static NamedServiceDescriptor WithImplementationFactory(this NamedServiceDescriptor descriptor,
        Func<IServiceProvider, object> implementationFactory) =>
        new NamedServiceDescriptor(descriptor.ServiceType, implementationFactory,
            descriptor.Lifetime, descriptor.ServiceName);
}