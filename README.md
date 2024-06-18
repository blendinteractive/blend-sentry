# BlendInteractive.Sentry

Blend's default Sentry configuration. This repo / nuget package is a simple way to ensure all Blend-related Sentry installations are consistent and have a default minimum exception filtering applied.

## Installation

For **.NET 6**:

1. Install the `BlendInteractive.Sentry` package.
2. In the Program.cs file, in the `CreateHostBuilder` method, call the the `UseBlendSentry` extension method from your `IWebHostBuilder` builder, optionally passing in a `Func<Exception?, bool>` method to add further filtering.

```
    return Host.CreateDefaultBuilder(args)
        .ConfigureCmsDefaults()
        .ConfigureAppConfiguration((ctx, builder) => {
            builder.AddConfiguration(_configuration);
        })
        .ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder
                // return true if the message should be filtered and removed. 
                .UseBlendSentry(ex => ex!.Message.Contains("this is the text in the exception message to filter", StringComparison.InvariantCultureIgnoreCase)) 
                .UseStartup<Startup>();
        });
```
