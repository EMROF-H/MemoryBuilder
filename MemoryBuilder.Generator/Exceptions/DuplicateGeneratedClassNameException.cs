using System;

namespace MemoryBuilder.Exceptions;

internal class DuplicateGeneratedClassNameException : Exception
{
    public DuplicateGeneratedClassNameException(string name)
        : base($"A class with the name '{name}' has already been generated.") { }
}
