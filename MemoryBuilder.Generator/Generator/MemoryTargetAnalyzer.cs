using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MemoryBuilder.Generator;

/// <summary>
/// Registers the diagnostic rules defined in <see cref="MemoryDiagnostics"/> so they can be recognized by the IDE and .editorconfig.
/// Note: This analyzer does not perform any analysis; it exists solely to expose diagnostics for configuration purposes.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class MemoryTargetAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        MemoryDiagnostics.AllDiagnostics.ToImmutableArray();

    public override void Initialize(AnalysisContext context)
    {
        // No analysis is performed. This analyzer only exists to declare supported diagnostics.
    }
}
