#if NET48
using Sentry;
using Sentry.AspNet;
using System;
using System.Configuration;
using RequestSize = Sentry.Extensibility.RequestSize;
#endif

#if NET50
using Sentry;
using System;
using Microsoft.AspNetCore.Hosting;
#endif

namespace BlendInteractive.Sentry
{
    public static class ApplicationStartupExtensions
    {

#if NET50
        public static IWebHostBuilder UseBlendSentry(this IWebHostBuilder builder, Func<Exception?, bool>? additionalFiltering = null)
        {
            builder.UseSentry(op =>
            {
                op.BeforeSend = (sentryEvent) =>
                {
                    if (ExceptionFilter.ShouldFilterException(sentryEvent.Exception))
                        return null;

                    if (additionalFiltering != null && additionalFiltering(sentryEvent.Exception))
                        return null;

                    return sentryEvent;
                };
            });

            return builder;
        }
#endif

#if NET48
        public static IDisposable? InitializeSentry(Func<Exception?, bool>? additionalFiltering = null)
        {
            var dsn = ConfigurationManager.AppSettings["sentry:Dsn"];
            IDisposable? sentry = null;

            if (!string.IsNullOrEmpty(dsn))
            {
                sentry = SentrySdk.Init(config =>
                {
                    config.Dsn = dsn;
                    config.Environment = ConfigurationManager.AppSettings["sentry:Environment"];
                    config.AddAspNet(RequestSize.Medium);
                    config.BeforeSend = (sentryEvent) =>
                    {
                        if (ExceptionFilter.ShouldFilterException(sentryEvent.Exception))
                            return null;

                        if (additionalFiltering != null && additionalFiltering(sentryEvent.Exception))
                            return null;

                        return sentryEvent;
                    };
                });
            }

            return sentry;
        }

#endif
        public static void LogException(this Exception? exception)
        {
            if (exception == null)
                return;

            SentrySdk.CaptureException(exception);
        }

    }
}
