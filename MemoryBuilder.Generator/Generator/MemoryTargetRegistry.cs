using System.Collections.Generic;
using System.Linq;
using MemoryBuilder.Attributes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MemoryBuilder.Generator;

internal class MemoryTargetRegistry
{
    public IReadOnlyDictionary<string, INamedTypeSymbol> Targets { get; }

    private MemoryTargetRegistry(Dictionary<string, INamedTypeSymbol> targets)
    {
        Targets = targets;
    }
    
    public static MemoryTargetRegistry Analyze(GeneratorExecutionContext context, MemoryTargetSyntaxReceiver receiver)
    {
        var compilation = context.Compilation;
        var memoryTargetAttr = compilation.GetAttributeSymbol<MemoryTargetAttribute>();

        var nameToSymbols = new Dictionary<string, List<(StructDeclarationSyntax syntax, INamedTypeSymbol symbol)>>();
        var finalTargets = new Dictionary<string, INamedTypeSymbol>();

        foreach (var structSyntax in receiver.Candidates)
        {
            var model = compilation.GetSemanticModel(structSyntax.SyntaxTree);
            var symbol = model.GetDeclaredSymbol(structSyntax) as INamedTypeSymbol;
            if (symbol is null)
            {
                MemoryDiagnostics.Report(context, structSyntax, MemoryDiagnostics.UnableToResolveSymbol);
                continue;
            }

            var attr = symbol.GetAttribute(memoryTargetAttr);
            if (attr is null)
            {
                continue;
            }

            var targetName = attr.ConstructorArguments[0].Value as string;
            if (string.IsNullOrWhiteSpace(targetName))
            {
                MemoryDiagnostics.Report(context, structSyntax, MemoryDiagnostics.MissingName, symbol.Name);
                continue;
            }

            if (!symbol.IsUnmanagedType)
            {
                MemoryDiagnostics.Report(context, structSyntax, MemoryDiagnostics.NotUnmanaged, symbol.Name);
                continue;
            }

            if (!nameToSymbols.ContainsKey(targetName))
            {
                nameToSymbols[targetName] = new();
            }

            nameToSymbols[targetName].Add((structSyntax, symbol));
        }

        // 挑选只保留没有重复的 targetName，报告重复项
        foreach (var kvp in nameToSymbols)
        {
            var list = kvp.Value;
            if (list.Count == 1)
            {
                finalTargets[kvp.Key] = list[0].symbol;
            }
            else
            {
                foreach (var (syntax, _) in list)
                {
                    MemoryDiagnostics.Report(context, syntax, MemoryDiagnostics.DuplicateName, kvp.Key);
                }
            }
        }

        return new MemoryTargetRegistry(finalTargets);
    }
}
