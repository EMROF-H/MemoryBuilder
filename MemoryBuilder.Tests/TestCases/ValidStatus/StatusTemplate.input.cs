using MemoryBuilder;
using MemoryBuilder.Attributes;

namespace MemoryBuilder.Tests.TestCases.ValidStatus;

[MemoryTarget("Status")]
public struct StatusTemplate
{
    public int v1;
    public Pointer v2;
}
