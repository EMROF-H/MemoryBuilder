using MemoryBuilder;
using MemoryBuilder.Attributes;

namespace MemoryBuilder.Tests.TestCases.LinkedList;

[MemoryTarget("LinkedList")]
public struct LinkedListTemplate
{
    public int Count;
    
    [Nullable]
    public Pointer<ListNodeTemplate> Head;
}
