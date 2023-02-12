using System;
using Microsoft.Extensions.DependencyInjection;

namespace TinyNamedServiceResolver.Tests;

public interface INamedImplementation
{
}

public interface IOtherNamedImplementation
{
}

public class DefaultOtherImplementation : IOtherNamedImplementation
{
}

public class FooNamedOtherImplementation : IOtherNamedImplementation
{
}

public class FooNamedImplementation : INamedImplementation
{
}

public class BarNamedImplementation : INamedImplementation
{
}

public class DefaultImplementation : INamedImplementation
{
}

public class TestHelper
{
    private readonly ServiceCollection _services;

    public TestHelper()
    {
        _services = new ServiceCollection();
    }

    public TestHelper ConfigureServices(Action<IServiceCollection> f)
    {
        f(_services);
        return this;
    }

    public IServiceProvider CreateServiceProviderWithNameValue(string currentNameValue)
    {
        return new TinyNamedServiceProviderFactory(_ => currentNameValue).CreateServiceProvider(_services);
    }
}