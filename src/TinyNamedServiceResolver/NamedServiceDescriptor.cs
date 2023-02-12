using System;
using Microsoft.Extensions.DependencyInjection;

namespace TinyNamedServiceResolver;

public class NamedServiceDescriptor : ServiceDescriptor
{
    public string ServiceName { get; }

    public NamedServiceDescriptor(
        Type serviceType,
        Type implementationType,
        ServiceLifetime lifetime,
        string serviceName
    )
        : base(serviceType, implementationType, lifetime)
    {
        ServiceName = serviceName;
    }

    public NamedServiceDescriptor(
        Type serviceType,
        Func<IServiceProvider, object> factory,
        ServiceLifetime lifetime,
        string serviceName
    )
        : base(serviceType, factory, lifetime)
    {
        ServiceName = serviceName;
    }

    public NamedServiceDescriptor(
        Type serviceType,
        object instance,
        string serviceName
    )
        : base(serviceType, instance)
    {
        ServiceName = serviceName;
    }
}