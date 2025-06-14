using MemoryBuilder.Generator;
using Microsoft.CodeAnalysis.Testing;
using Verify = MemoryBuilder.Tests.VerifyHelper;

namespace MemoryBuilder.Tests;

public class MemoryTargetTests
{
    [Fact]
    public void SanityCheck() => Assert.Equal(1, 1);

    [Fact]
    public async Task Valid_MemoryTarget_Should_Generate_Status()
    {
        const string name = "ValidStatus";
        var input = TestFileLoader.Load(name, "StatusTemplate.input.cs");
        var output = TestFileLoader.Load(name, "Status.output.cs");
        var expected = Expected<MemoryTargetGenerator>("Status.g.cs", output);
        
        await new Verify.Test
        {
            TestState =
            {
                Sources = { input },
                GeneratedSources = { expected }
            }
        }.RunAsync();
    }

    [Fact]
    public async Task Valid_MemoryTarget_Should_Generate_LinkedList()
    {
        const string name = "LinkedList";
        var input1 = TestFileLoader.Load(name, "LinkedListTemplate.input.cs");
        var input2 = TestFileLoader.Load(name, "ListNodeTemplate.input.cs");
        var output1 = TestFileLoader.Load(name, "LinkedList.output.cs");
        var output2 = TestFileLoader.Load(name, "ListNode.output.cs");
        var expected1 = Expected<MemoryTargetGenerator>("LinkedList.g.cs", output1);
        var expected2 = Expected<MemoryTargetGenerator>("ListNode.g.cs", output2);
        
        await new Verify.Test
        {
            TestState =
            {
                Sources = { input1, input2 },
                GeneratedSources = { expected1, expected2 }
            }
        }.RunAsync();
    }
    
    [Fact]
    public async Task Duplicate_TargetName_Should_Report_Error()
    {
        const string name = "DuplicateName";
        var input = TestFileLoader.Load(name, "TowerTemplate.input.cs");
        var output = TestFileLoader.Load(name, "Tower.output.cs");

        var expected1 = DiagnosticResult
            .CompilerError("MT005")
            .WithSpan(5, 1, 9, 2)
            .WithMessage("Multiple [MemoryTarget] structs share the same targetName: 'TowerDup'");

        var expected2 = DiagnosticResult
            .CompilerError("MT005")
            .WithSpan(17, 1, 21, 2)
            .WithMessage("Multiple [MemoryTarget] structs share the same targetName: 'TowerDup'");
    
        await new Verify.Test
        {
            TestState =
            {
                Sources = { input },
                GeneratedSources = { Expected<MemoryTargetGenerator>("Tower.g.cs", output) },
                ExpectedDiagnostics = { expected1, expected2 }
            }
        }.RunAsync();
    }
}
