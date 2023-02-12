using System;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace TinyNamedServiceResolver.Tests;

public class ExceptionsTests
{
    [Fact]
    public void Lifetime_Of_Named_Services_Needs_To_Match()
    {
        Should.Throw<ServiceLifetimeMismatchException>(() => new TestHelper()
            .ConfigureServices(services =>
            {
                services.AddSingleton<INamedImplementation, DefaultImplementation>();
                services.AddTransient<INamedImplementation>(_ => new FooNamedImplementation(), "foo");
                services.AddSingleton<INamedImplementation, BarNamedImplementation>("bar");
            })
            .CreateServiceProviderWithNameValue("bar")
        );
    }

    [Fact]
    public void Lifetime_Of_Default_Service_Type_Needs_To_Match()
    {
        Should.Throw<ServiceLifetimeMismatchException>(() => new TestHelper()
            .ConfigureServices(services =>
            {
                services.AddTransient<INamedImplementation>(_ => new FooNamedImplementation(), "foo");
                services.AddSingleton<INamedImplementation, BarNamedImplementation>();
            })
            .CreateServiceProviderWithNameValue("foo"));
    }

    [Fact]
    public void Fix()
    {
        Should.Throw<ServiceLifetimeMismatchException>(() => new TestHelper()
            .ConfigureServices(services =>
            {
                services.AddSingleton<IOtherNamedImplementation, DefaultOtherImplementation>();
                services.AddTransient<IOtherNamedImplementation, FooNamedOtherImplementation>("foo");

                services.AddTransient<INamedImplementation, DefaultImplementation>();
                services.AddTransient<INamedImplementation, BarNamedImplementation>("foo");
            })
            .CreateServiceProviderWithNameValue("foo"));
    }

    [Fact]
    public void Named_Services_Need_To_Have_Default_Implementation_Registered()
    {
        Should.Throw<MissingDefaultImplementationException>(() => new TestHelper()
            .ConfigureServices(services =>
            {
                services.AddTransient<INamedImplementation, FooNamedImplementation>("foo");
            })
            .CreateServiceProviderWithNameValue("foo"));
    }

    [Fact]
    public void Service_Name_Should_Not_Be_Null()
    {
        Should.Throw<ArgumentNullException>(() => new TestHelper()
            .ConfigureServices(services =>
            {
                services.AddTransient<INamedImplementation, DefaultImplementation>();
                services.AddTransient<INamedImplementation, FooNamedImplementation>((string) null);
            })
            .CreateServiceProviderWithNameValue("foo"));
    }
}