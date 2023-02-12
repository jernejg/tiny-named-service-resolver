using System;

namespace TinyNamedServiceResolver;

public class MissingDefaultImplementationException : Exception
{
    public MissingDefaultImplementationException(string? message)
        : base(message)
    {
    }
}

public class ServiceLifetimeMismatchException : Exception
{
    public ServiceLifetimeMismatchException(string? message)
        : base(message)
    {
    }
}