# tiny-named-service-resolver

## Demo
```csharp
using var host = Host.CreateDefaultBuilder(args)
  .UseServiceProviderFactory(new TinyNamedServiceProviderFactory(provider => "foo"))
  .ConfigureServices(services =>
  {
      services.AddTransient<ISomeService, DefaultService>();
      services.AddTransient<ISomeService, FooService>("foo");
      services.AddTransient<ISomeService, BarService>("bar");
  })
  .Build();

Debug.Assert(host.Services.GetRequiredService<ISomeService>() is FooService);
```

## Features

1. Provide the name used for resolving using a custom `Func<IServiceProvider, string>`:
```csharp
// just an example of how to select the implementation type 
// based on a request header value
.UseServiceProviderFactory(new TinyNamedServiceProviderFactory(provider =>
{
    return provider.GetRequiredService<IHttpContextAccessor>()
                   .HttpContext.Request.Headers["header_name"][0];
}))
```

2. Each named service needs to be registered together with the default implementation, otherwise you won't be able to create the `IServiceProvider`:
```csharp
// Registration of ISomeService without a name, 
// represent the default implementation
services.AddTransient<ISomeService, DefaultService>();
services.AddTransient<ISomeService, FooService>("foo");
services.AddTransient<ISomeService, BarService>("bar");
```

3. Arbitrary order of registration:

It doesn't matter in what order you register services. `TinyNamedServiceProviderFactory` makes sure the last service descriptor for a type contains the necessary logic.

4. Named services of the same service type need to be registered with the same `ServiceLifetime`. The following will fail when building `IServiceProvider`:

```csharp
// Needs to be transient, or the last two scoped.
services.AddScoped<ISomeService, DefaultService>();
services.AddTransient<ISomeService, FooService>("foo");
services.AddTransient<ISomeService, BarService>("bar");
```

## How it works
Just before `IHostBuilder` initializes the `IServiceProvider` a slight modification is made to the last `NamedServiceDescriptor` for every named service type inside `IServiceCollection`. It introduces (or replaces) the `Func<IServiceProvider, TService> implementationFactory` with one that knows which implementation type to pick.

<p align="center">
<img src="https://user-images.githubusercontent.com/5320517/218338012-9e008b4b-9444-41a0-8ac2-f5d3b2be2f4d.png">
</p>
