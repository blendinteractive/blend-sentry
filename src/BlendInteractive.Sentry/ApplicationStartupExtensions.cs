using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using Microsoft.AspNetCore.Hosting;
using Sentry;
using Sentry.AspNetCore;

namespace BlendInteractive.Sentry
{
    public static class ApplicationStartupExtensions
    {
        private static readonly ExceptionFilterDelegate FilterWebSocketExceptions =
            (exception) => exception is WebSocketException && exception.Message.IndexOf("The remote party closed the ", StringComparison.InvariantCultureIgnoreCase) >= 0;


        private static readonly ExceptionFilterDelegate[] DefaultExceptionFilters =
        [
            FilterWebSocketExceptions
        ];

        public static IWebHostBuilder UseBlendSentry(this IWebHostBuilder builder, params ExceptionFilterDelegate[] filters)
        {
            builder.UseSentry((SentryAspNetCoreOptions op) =>
            {
                op.SetBeforeSend((ev) =>
                {
                    // We don't want to send any errors that are a result of V11Y scanning. These errors are plentiful and 
                    // generally not useful.
                    string? userAgent = null;
                    bool hasUserAgent = (ev.Request?.Headers?.TryGetValue("User-Agent", out userAgent) ?? false);
                    if (hasUserAgent && !string.IsNullOrEmpty(userAgent))
                    {
                        bool isProbablyDetectify = userAgent.Contains("Detectify", StringComparison.InvariantCultureIgnoreCase);
                        if (isProbablyDetectify)
                            return null;
                    }

                    return ev;
                });

                if (filters.Length > 0)
                {
                    var finalList = new List<ExceptionFilterDelegate>(DefaultExceptionFilters);
                    finalList.AddRange(filters);
                    op.AddExceptionFilter(new DelegatesExceptionFilter(finalList.ToArray()));
                }
                else
                {
                    op.AddExceptionFilter(new DelegatesExceptionFilter(DefaultExceptionFilters));
                }
            });

            return builder;
        }

        public static void LogException(this Exception? exception)
        {
            if (exception == null)
                return;

            SentrySdk.CaptureException(exception);
        }

    }
}
