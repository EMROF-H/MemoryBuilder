using System;

namespace MemoryBuilder.Exceptions;

internal class InvalidMemoryTargetException : Exception
{
    public InvalidMemoryTargetException(string structName, string reason)
        : base($"MemoryTarget '{structName}' is invalid: {reason}") { }
}
