using MemoryBuilder.Attributes;

namespace MemoryBuilder.Tests.TestCases.DuplicateName;

[MemoryTarget("TowerDup")]
public struct TowerTemplate1
{
    public int v1;
}

[MemoryTarget("Tower")]
public struct TowerTemplate2
{
    public int v1;
}

[MemoryTarget("TowerDup")]
public struct TowerTemplate3
{
    public int v1;
}
