using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MemoryBuilder.Generator;

internal static class MemoryDiagnostics
{
    private const string Category = "MemoryGen";

    public static readonly DiagnosticDescriptor UnableToResolveSymbol = new(
        id: "MT001",
        title: "Cannot resolve struct symbol",
        messageFormat: "Unable to resolve symbol for struct at this location",
        category: Category,
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor AttributeNotResolved = new(
        id: "MT002",
        title: "MemoryTargetAttribute not resolved",
        messageFormat: "Struct '{0}' does not resolve a valid [MemoryTarget] attribute",
        category: Category,
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor MissingName = new(
        id: "MT003",
        title: "Missing name",
        messageFormat: "[MemoryTarget] on '{0}' has no valid name",
        category: Category,
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor NotUnmanaged = new(
        id: "MT004",
        title: "Type must be unmanaged",
        messageFormat: "Struct '{0}' marked with [MemoryTarget] must be unmanaged",
        category: Category,
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor DuplicateName = new(
        id: "MT005",
        title: "Duplicate target name",
        messageFormat: "Multiple [MemoryTarget] structs share the same targetName: '{0}'",
        category: Category,
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor RecommendSequentialLayout = new(
        id: "MT101",
        title: "Missing StructLayout",
        messageFormat: "Struct '{0}' should be explicitly marked with [StructLayout(LayoutKind.Sequential)] to ensure memory layout compatibility",
        category: Category,
        DiagnosticSeverity.Info,
        isEnabledByDefault: true);

    public static IEnumerable<DiagnosticDescriptor> AllDiagnostics => new[]
    {
        MissingName,
        NotUnmanaged,
        DuplicateName,
        RecommendSequentialLayout
    };
        
    public static void Report(GeneratorExecutionContext context, StructDeclarationSyntax syntax, DiagnosticDescriptor descriptor, params object[] args)
    {
        var location = syntax.GetLocation();
        var diagnostic = Diagnostic.Create(descriptor, location, args);
        context.ReportDiagnostic(diagnostic);
    }
}
