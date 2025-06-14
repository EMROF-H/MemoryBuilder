global using static MemoryBuilder.Tests.GeneratorTestHelper;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace MemoryBuilder.Tests;

public static class GeneratorTestHelper
{
    public static (string filename, SourceText content) Expected<TGenerator>(string fileName, string content)
        where TGenerator : ISourceGenerator, new()
    {
        var generatorType = typeof(TGenerator);
        var filePath = $@"{generatorType.Namespace}\{generatorType.FullName}\{fileName}";
        return (filePath, SourceText.From(content, Encoding.UTF8));
    }
}
