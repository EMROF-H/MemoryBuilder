using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MemoryBuilder.Generator;

internal class MemoryTargetSyntaxReceiver : ISyntaxReceiver
{
    public List<StructDeclarationSyntax> Candidates { get; } = new();

    public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
    {
        if (syntaxNode is not StructDeclarationSyntax { AttributeLists.Count: > 0 } structDecl)
        {
            return;
        }
        Candidates.Add(structDecl);
    }
}
