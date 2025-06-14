using MemoryBuilder.Attributes;

#nullable enable

namespace MemoryBuilder.Tests.TestCases.DuplicateName;

public partial class Tower
{
    public int v1;

    private Tower(int v1)
    {
        this.v1 = v1;
    }

    public static Tower? TryBuildFromMemory(Handle handle, Pointer pointer)
    {
        if (!pointer.TryRead<TowerTemplate2>(handle, out var self))
        {
            return null;
        }

        var v1 = self.v1;

        return new(v1);
    }
}
