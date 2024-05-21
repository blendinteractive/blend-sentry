using Sentry.Extensibility;
using System;
using System.Linq;
using System.Net.WebSockets;

namespace BlendInteractive.Sentry;


public delegate bool ExceptionFilterDelegate(Exception ex);

public delegate bool RequestFilterDelegate(Exception ex);

public class DelegatesExceptionFilter(ExceptionFilterDelegate[] delegates) : IExceptionFilter
{
    public bool Filter(Exception ex)
    {
        foreach (var delegates in delegates)
        {
            if (delegates(ex))
                return true;
        }

        return false;
    }
}
