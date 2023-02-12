using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace TinyNamedServiceResolver.Tests;

public class HappyTests
{
    [Fact]
    public void Happy_Test()
    {
        var sut = new TestHelper()
            .ConfigureServices(services =>
            {
                services.AddTransient<INamedImplementation, DefaultImplementation>();
                services.AddTransient<INamedImplementation, BarNamedImplementation>("bar");
                services.AddTransient<INamedImplementation, FooNamedImplementation>("foo");
            })
            .CreateServiceProviderWithNameValue("foo");

        sut.GetRequiredService<INamedImplementation>()
            .ShouldBeOfType<FooNamedImplementation>();
    }

    [Fact]
    public void Happy_Test_That_Shows_Registration_Order_Doesnt_Matter()
    {
        var sut = new TestHelper()
            .ConfigureServices(services =>
            {
                services.AddTransient<INamedImplementation, FooNamedImplementation>("foo");
                services.AddTransient<INamedImplementation, BarNamedImplementation>("bar");
                services.AddTransient<INamedImplementation, DefaultImplementation>();
            })
            .CreateServiceProviderWithNameValue("foo");

        sut.GetRequiredService<INamedImplementation>()
            .ShouldBeOfType<FooNamedImplementation>();
    }

    [Fact]
    public void If_Named_Service_Doesnt_Exist_Default_Implementation_Is_Selected()
    {
        var sut = new TestHelper()
            .ConfigureServices(services =>
            {
                services.AddTransient<INamedImplementation, FooNamedImplementation>("foo");
                services.AddTransient<INamedImplementation, DefaultImplementation>();
            })
            .CreateServiceProviderWithNameValue("bar");

        sut.GetRequiredService<INamedImplementation>()
            .ShouldBeOfType<DefaultImplementation>();
    }

    [Fact]
    public void If_Named_Service_Doesnt_Exist_Default_Implementation_Is_Selected_And_Order_Doesnt_Matter()
    {
        var sut = new TestHelper()
            .ConfigureServices(services =>
            {
                services.AddTransient<INamedImplementation, DefaultImplementation>();
                services.AddTransient<INamedImplementation, FooNamedImplementation>("foo");
            })
            .CreateServiceProviderWithNameValue("bar");

        sut.GetRequiredService<INamedImplementation>()
            .ShouldBeOfType<DefaultImplementation>();
    }

    [Fact]
    public void When_Multiple_Default_Implementations_Exist_Last_One_Is_Selected()
    {
        var sut = new TestHelper()
            .ConfigureServices(services =>
            {
                services.AddTransient<INamedImplementation, DefaultImplementation>();
                services.AddTransient<INamedImplementation, FooNamedImplementation>("foo");
                services.AddTransient<INamedImplementation, BarNamedImplementation>();
            })
            .CreateServiceProviderWithNameValue("xxx");

        sut.GetRequiredService<INamedImplementation>()
            .ShouldBeOfType<BarNamedImplementation>();
    }


    [Fact]
    public void When_Multiple_Implementations_Have_The_Same_Name_Last_One_Is_Selected()
    {
        var sut = new TestHelper()
            .ConfigureServices(services =>
            {
                services.AddTransient<INamedImplementation, DefaultImplementation>();
                services.AddTransient<INamedImplementation, BarNamedImplementation>("foo");
                services.AddTransient<INamedImplementation, FooNamedImplementation>("foo");
            })
            .CreateServiceProviderWithNameValue("foo");

        sut.GetRequiredService<INamedImplementation>()
            .ShouldBeOfType<FooNamedImplementation>();
    }

    [Fact]
    public void Name_Resolution_Is_Case_Sensitive()
    {
        var sut = new TestHelper()
            .ConfigureServices(services =>
            {
                services.AddTransient<INamedImplementation, DefaultImplementation>();
                services.AddTransient<INamedImplementation, FooNamedImplementation>("Foo");
            })
            .CreateServiceProviderWithNameValue("foo");

        sut.GetRequiredService<INamedImplementation>()
            .ShouldBeOfType<DefaultImplementation>();
    }

    [Fact]
    public void Named_Service_Can_Be_Created_Using_A_Factory()
    {
        var sut = new TestHelper()
            .ConfigureServices(services =>
            {
                services.AddTransient<INamedImplementation>(_ => new FooNamedImplementation(), "foo");
                services.AddTransient<INamedImplementation, DefaultImplementation>();
            })
            .CreateServiceProviderWithNameValue("foo");

        sut.GetRequiredService<INamedImplementation>()
            .ShouldBeOfType<FooNamedImplementation>();
    }

    [Fact]
    public void Named_Service_Can_Be_Created_From_Existing_Instance()
    {
        var sut = new TestHelper()
            .ConfigureServices(services =>
            {
                services.AddSingleton<INamedImplementation>(new FooNamedImplementation(), "foo");
                services.AddSingleton<INamedImplementation, DefaultImplementation>();
            })
            .CreateServiceProviderWithNameValue("foo");

        sut.GetRequiredService<INamedImplementation>()
            .ShouldBeOfType<FooNamedImplementation>();
    }

    [Fact]
    public void Default_Service_Can_Be_Created_Using_A_Factory()
    {
        var sut = new TestHelper()
            .ConfigureServices(services =>
            {
                services.AddTransient<INamedImplementation>(_ => new DefaultImplementation());
                services.AddTransient<INamedImplementation, FooNamedImplementation>("foo");
            })
            .CreateServiceProviderWithNameValue("bar");

        sut.GetRequiredService<INamedImplementation>()
            .ShouldBeOfType<DefaultImplementation>();
    }

    [Fact]
    public void Default_Service_Can_Be_Created_From_Existing_Instance()
    {
        var sut = new TestHelper()
            .ConfigureServices(services =>
            {
                services.AddSingleton<INamedImplementation, FooNamedImplementation>("foo");
                services.AddSingleton<INamedImplementation>(new DefaultImplementation());
            })
            .CreateServiceProviderWithNameValue("bar");

        sut.GetRequiredService<INamedImplementation>()
            .ShouldBeOfType<DefaultImplementation>();
    }
    
    [Fact]
    public void Trying_To_Resolve_Using_Null_Will_Result_In_Default_Service()
    {
        var sut = new TestHelper()
            .ConfigureServices(services =>
            {
                services.AddTransient<INamedImplementation, DefaultImplementation>();
                services.AddTransient<INamedImplementation, FooNamedImplementation>("foo");
            })
            .CreateServiceProviderWithNameValue(null!);
        
        sut.GetRequiredService<INamedImplementation>()
            .ShouldBeOfType<DefaultImplementation>();
    }
}