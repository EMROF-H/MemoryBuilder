using MemoryBuilder;
using MemoryBuilder.Attributes;

#nullable enable

namespace MemoryBuilder.Tests.TestCases.LinkedList;

public partial class LinkedList
{
    public int Count;
    public ListNode? Head;

    private LinkedList(int Count, ListNode? Head)
    {
        this.Count = Count;
        this.Head = Head;
    }

    public static LinkedList? TryBuildFromMemory(Handle handle, Pointer pointer)
    {
        if (!pointer.TryRead<LinkedListTemplate>(handle, out var self))
        {
            return null;
        }

        var Count = self.Count;
        var Head = ListNode.TryBuildFromMemory(handle, self.Head);

        return new(Count, Head);
    }
}
