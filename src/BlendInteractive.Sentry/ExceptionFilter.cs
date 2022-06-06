using System;
using System.Linq;
using System.Net.WebSockets;

#if NET48
using System.Web;
#endif

namespace BlendInteractive.Sentry
{
    public static class ExceptionFilter
    {
#if NET48
        // Lots of ignorable exceptions hide under the `HttpException` type.
        private static readonly string[] HttpExceptionMessages = new[]
        {
            "was not found on controller", // A public action method 'X' was not found on controller 'Y'.
            "Not Found", // Not Found. OR Not found: https://X/file.jpg
            "does not exist", // The file '/X.aspx' does not exist.
            "A potentially dangerous Request.Path", // A potentially dangerous Request.Path value was detected from the client (&).
            "This is an invalid webresource request",
            "In use notification already exists.",
            "Item Not Found"
        };
#endif

        public static bool ShouldFilterException(this Exception? exception)
        {
            if (exception == null)
                return false;

            if (exception is WebSocketException && exception.Message.IndexOf("The remote party closed the ", StringComparison.InvariantCultureIgnoreCase) >= 0)
                return true;

#if NET48
            if (exception is HttpRequestValidationException)
                return true;

            if (exception is HttpException)
            {
                if (HttpExceptionMessages.Any(x => exception.Message.IndexOf(x, StringComparison.InvariantCultureIgnoreCase) >= 0))
                    return true;
            }
#endif

            return false;
        }
    }
}
