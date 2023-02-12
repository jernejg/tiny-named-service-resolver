using System;
using System.Linq;
using TinyNamedServiceResolver;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static void AddTransient<TService, TImplementation>(this IServiceCollection services, string serviceName)
        where TService : class
        where TImplementation : class, TService
    {
        ArgumentNullException.ThrowIfNull(serviceName);
        services.Add(new NamedServiceDescriptor(
            typeof(TService),
            typeof(TImplementation),
            ServiceLifetime.Transient,
            serviceName)
        );
    }

    public static void AddTransient<TService>(this IServiceCollection services,
        Func<IServiceProvider, object> factory,
        string serviceName)
        where TService : class
    {
        ArgumentNullException.ThrowIfNull(serviceName);
        services.Add(new NamedServiceDescriptor(
            typeof(TService),
            factory,
            ServiceLifetime.Transient,
            serviceName)
        );
    }

    public static void AddSingleton<TService, TImplementation>(this IServiceCollection services,
        string serviceName)
        where TService : class
        where TImplementation : class, TService
    {
        ArgumentNullException.ThrowIfNull(serviceName);
        services.Add(new NamedServiceDescriptor(
            typeof(TService),
            typeof(TImplementation),
            serviceName)
        );
    }

    public static void AddSingleton<TService>(this IServiceCollection services,
        object instance,
        string serviceName)
        where TService : class
    {
        ArgumentNullException.ThrowIfNull(serviceName);
        services.Add(new NamedServiceDescriptor(
            typeof(TService),
            instance,
            serviceName)
        );
    }
}