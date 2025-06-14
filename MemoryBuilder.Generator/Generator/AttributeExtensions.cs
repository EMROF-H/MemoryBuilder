using System;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace MemoryBuilder.Generator;

#nullable enable

internal static class AttributeExtensions
{
    private static bool DefaultEquals(this INamedTypeSymbol? s1, INamedTypeSymbol? s2) =>
        SymbolEqualityComparer.Default.Equals(s1, s2);

    public static INamedTypeSymbol GetAttributeSymbol<T>(this Compilation compilation) where T : Attribute
    {
        var name = typeof(T).FullName!;
        var symbol = compilation.GetTypeByMetadataName(name);
        if (symbol is null)
        {
            throw new InvalidOperationException($"Cannot find attribute symbol: {typeof(T).FullName}");
        }
        return symbol;
    }

    public static bool HasAttribute(this ISymbol symbol, INamedTypeSymbol attributeSymbol) => symbol
        .GetAttributes()
        .Any(attr => attr.AttributeClass.DefaultEquals(attributeSymbol));

    public static AttributeData? GetAttribute(this ISymbol symbol, INamedTypeSymbol attributeSymbol) => symbol
        .GetAttributes()
        .FirstOrDefault(attr => attr.AttributeClass.DefaultEquals(attributeSymbol));
}
