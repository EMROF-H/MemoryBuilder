using MemoryBuilder;
using MemoryBuilder.Attributes;

namespace MemoryBuilder.Tests.TestCases.LinkedList;

[MemoryTarget("ListNode")]
public struct ListNodeTemplate
{
    public int Value;

    [Nullable]
    public Pointer<ListNodeTemplate> Next;
}
