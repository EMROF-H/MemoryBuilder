namespace MemoryBuilder.Tests;

public static class TestFileLoader
{
    public static string Load(params string[] pathParts)
    {
        var baseDir = Path.Combine(AppContext.BaseDirectory, "..", "..", "..");
        var testCaseDir = Path.Combine(baseDir, "TestCases");
        var fullPath = Path.Combine(testCaseDir, Path.Combine(pathParts));
        return File.ReadAllText(fullPath);
    }
}
