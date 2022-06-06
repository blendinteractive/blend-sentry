# BlendInteractive.Sentry

Blend's default Sentry configuration. This repo / nuget package is a simple way to ensure all Blend-related Sentry installations are consistent and have a default minimum exception filtering applied.

Note: This 

## Installation

For **.NET 4.8**:

1. Install the `BlendInteractive.Sentry` package.
2. In your `global.asax.cs ` file, in the `Application_Start` method, call `ApplicationStartupExtensions.InitializeSentry`, optionally passing in a `Func<Exception?, bool>` method to add further filtering. Keep the returned `IDisposable` value in a member.
3. In the `Application_Error` method, get the last server error and pass it to `ApplicationStartupExtensions.LogException`.
4. In the `Application_End` method, dispose of the `IDisposable` instance returned by `ApplicationStartupExtensions.InitializeSentry`.

```
    private static IDisposable? _sentry = null;

    protected void Application_Start()
    {
        _sentry = ApplicationStartupExtensions.InitializeSentry();
    }

    protected void Application_Error()
    {
        var exception = Server.GetLastError();
        ApplicationStartupExtensions.LogException(exception);
    }

    protected void Application_End()
    {
        _sentry?.Dispose();
    }
```

For **.NET 5**:

1. Install the `BlendInteractive.Sentry` package.
2. In the Program.cs file, in the `CreateHostBuilder` method, call the the `UseSentry` extension method from your `IWebHostBuilder` builder, optionally passing in a `Func<Exception?, bool>` method to add further filtering. 

```
    return Host.CreateDefaultBuilder(args)
        .ConfigureCmsDefaults()
        .ConfigureAppConfiguration((ctx, builder) => {
            builder.AddConfiguration(_configuration);
        })
        .ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder
                .UseSentry()
                .UseStartup<Startup>();
        });
```
