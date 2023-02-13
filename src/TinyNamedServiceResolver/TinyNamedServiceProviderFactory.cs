using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace TinyNamedServiceResolver;

public class TinyNamedServiceProviderFactory : IServiceProviderFactory<IServiceCollection>
{
    private readonly Func<IServiceProvider, string> _nameProvider;

    public TinyNamedServiceProviderFactory(Func<IServiceProvider, string> nameProvider)
    {
        _nameProvider = nameProvider;
    }

    public IServiceCollection CreateBuilder(IServiceCollection services)
    {
        return services;
    }

    public IServiceProvider CreateServiceProvider(IServiceCollection services)
    {
        var namedDescriptorsCache = new Dictionary<(string, Type), NamedServiceDescriptor>();
        var defaultDescriptorsCache = new Dictionary<Type, ServiceDescriptor>();
        var lastDescriptors = new HashSet<NamedServiceDescriptor>();

        foreach (var descriptor in services.OfType<NamedServiceDescriptor>())
        {
            namedDescriptorsCache[(descriptor.ServiceName, descriptor.ServiceType)] = descriptor;

            lastDescriptors.Add(
                services
                    .OfType<NamedServiceDescriptor>()
                    .Reverse()
                    .First(x => x.ServiceType == descriptor.ServiceType)
            );

            var defaultDescriptor = services
                .Where(x => x is not NamedServiceDescriptor)
                .Reverse()
                .FirstOrDefault(x => x.ServiceType == descriptor.ServiceType);

            if (defaultDescriptor is null)
            {
                throw new MissingDefaultImplementationException(
                    $"Missing default implementation for {nameof(descriptor.ServiceType)}"
                );
            }

            defaultDescriptorsCache.TryAdd(descriptor.ServiceType, defaultDescriptor);
        }

        for (var i = 0; i < services.Count; i++)
        {
            var currentDescriptor = services[i];
            if (currentDescriptor is NamedServiceDescriptor namedDescriptor &&
                lastDescriptors.Contains(currentDescriptor))
            {
                services[i] = namedDescriptor
                    .WithImplementationFactory(provider => Factory(provider, currentDescriptor.ServiceType));
            }
            else if (defaultDescriptorsCache.TryGetValue(currentDescriptor.ServiceType, out var defaultDescriptor))
            {
                if (services[i] == defaultDescriptor)
                {
                    services[i] = defaultDescriptor
                        .WithImplementationFactory(provider => Factory(provider, defaultDescriptor.ServiceType));
                }
            }
        }

        CheckLifetimeConsistency(services, defaultDescriptorsCache);

        object Factory(IServiceProvider provider, Type serviceType)
        {
            var result = InitInstance(provider,
                namedDescriptorsCache.TryGetValue((_nameProvider(provider), serviceType), out var namedDescriptor)
                    ? namedDescriptor
                    : defaultDescriptorsCache[serviceType]);
            
            return result;
        }

        return services.BuildServiceProvider();
    }

    private static object InitInstance(IServiceProvider provider, ServiceDescriptor descriptor)
    {
        if (descriptor.ImplementationType is not null)
        {
            return ActivatorUtilities.CreateInstance(provider, descriptor.ImplementationType);
        }
        
        return descriptor.ImplementationFactory is not null
            ? descriptor.ImplementationFactory(provider)
            // ImplementationType, ImplementationFactory or ImplementationInstance can't all be null
            : descriptor.ImplementationInstance!;
    }

    private static void CheckLifetimeConsistency(IServiceCollection services,
        IReadOnlyDictionary<Type, ServiceDescriptor> defaultImplementations)
    {
        var descriptors = new Dictionary<Type, HashSet<ServiceLifetime>>();

        foreach (var descriptor in services)
        {
            if (descriptor is not NamedServiceDescriptor namedDescriptor)
            {
                continue;
            }

            descriptors.TryAdd(descriptor.ServiceType, new HashSet<ServiceLifetime>());
            descriptors[descriptor.ServiceType].Add(namedDescriptor.Lifetime);
            descriptors[descriptor.ServiceType].Add(defaultImplementations[descriptor.ServiceType].Lifetime);

            if (descriptors[descriptor.ServiceType].Count > 1)
            {
                throw new ServiceLifetimeMismatchException(
                    $"Service {nameof(namedDescriptor)} is registered with mixed lifetimes");
            }
        }
    }
}