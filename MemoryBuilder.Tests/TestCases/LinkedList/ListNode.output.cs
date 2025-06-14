using MemoryBuilder;
using MemoryBuilder.Attributes;

#nullable enable

namespace MemoryBuilder.Tests.TestCases.LinkedList;

public partial class ListNode
{
    public int Value;
    public ListNode? Next;

    private ListNode(int Value, ListNode? Next)
    {
        this.Value = Value;
        this.Next = Next;
    }

    public static ListNode? TryBuildFromMemory(Handle handle, Pointer pointer)
    {
        if (!pointer.TryRead<ListNodeTemplate>(handle, out var self))
        {
            return null;
        }

        var Value = self.Value;
        var Next = ListNode.TryBuildFromMemory(handle, self.Next);

        return new(Value, Next);
    }
}
