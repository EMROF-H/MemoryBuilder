using MemoryBuilder;
using MemoryBuilder.Attributes;

#nullable enable

namespace MemoryBuilder.Tests.TestCases.ValidStatus;

public partial class Status
{
    public int v1;
    public Pointer v2;

    private Status(int v1, Pointer v2)
    {
        this.v1 = v1;
        this.v2 = v2;
    }

    public static Status? TryBuildFromMemory(Handle handle, Pointer pointer)
    {
        if (!pointer.TryRead<StatusTemplate>(handle, out var self))
        {
            return null;
        }

        var v1 = self.v1;
        var v2 = self.v2;

        return new(v1, v2);
    }
}
