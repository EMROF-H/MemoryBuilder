using System;

namespace MemoryBuilder.Attributes;

[AttributeUsage(AttributeTargets.Struct)]
public sealed class MemoryTargetAttribute(string targetName) : Attribute
{
    public string TargetName { get; } = targetName;
}
