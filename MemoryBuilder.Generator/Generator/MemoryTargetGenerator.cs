using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MemoryBuilder.Attributes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace MemoryBuilder.Generator;

[Generator(LanguageNames.CSharp)]
public class MemoryTargetGenerator : ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context)
    {
        context.RegisterForSyntaxNotifications(() => new MemoryTargetSyntaxReceiver());
    }

    public void Execute(GeneratorExecutionContext context)
    {
        var receiver = (MemoryTargetSyntaxReceiver)context.SyntaxReceiver!;
        var registry = MemoryTargetRegistry.Analyze(context, receiver);

        foreach (var p in registry.Targets)
        {
            var targetName = p.Key;
            var structSymbol = p.Value;
            var result = GenerateClass(structSymbol, targetName);
            context.AddSource($"{targetName}.g.cs", SourceText.From(result, Encoding.UTF8));
        }
    }

    private IEnumerable<string> GetUsingsFromSyntaxTree(SyntaxTree syntaxTree)
    {
        var root = syntaxTree.GetRoot();
        return root.DescendantNodes()
            .OfType<UsingDirectiveSyntax>()
            .Select(u => u.ToString())
            .Distinct();
    }

    private string GenerateClass(INamedTypeSymbol templateStruct, string className)
    {
        var sb = new StringBuilder();

        foreach (var usingLine in GetUsingsFromSyntaxTree(templateStruct.DeclaringSyntaxReferences[0].SyntaxTree))
        {
            sb.AppendLine(usingLine);
        }
        sb.AppendLine();
        sb.AppendLine($"#nullable enable");
        sb.AppendLine();
        
        sb.AppendLine($"namespace {templateStruct.ContainingNamespace.ToDisplayString()};");
        sb.AppendLine();
        sb.AppendLine($"public partial class {className}");
        sb.AppendLine($"{{");

        var fields = templateStruct.GetMembers().OfType<IFieldSymbol>().ToArray();

        var ctorParams = new List<string>();
        var assignments = new List<string>();
        var factoryBody = new List<string>();

        foreach (var field in fields)
        {
            var type = field.Type;
            var name = field.Name;
            var isNullable = field.GetAttributes().Any(a => a.AttributeClass?.Name == "NullableAttribute");

            string targetType = TranslateFieldType(type, name, out string readLogic, out bool isPointerTemplateClass, isNullable);

            sb.AppendLine($"    public {targetType} {name};");
            ctorParams.Add($"{targetType} {name}");
            assignments.Add($"this.{name} = {name};");
            factoryBody.Add(readLogic);
        }

        sb.AppendLine();

        sb.AppendLine($"    private {className}({string.Join(", ", ctorParams)})");
        sb.AppendLine($"    {{");
        foreach (var assignment in assignments)
        {
            sb.AppendLine($"        {assignment}");
        }
        sb.AppendLine($"    }}");

        sb.AppendLine();

        sb.AppendLine($"    public static {className}? TryBuildFromMemory(Handle handle, Pointer pointer)");
        sb.AppendLine($"    {{");
        sb.AppendLine($"        if (!pointer.TryRead<{templateStruct.Name}>(handle, out var self))");
        sb.AppendLine($"        {{");
        sb.AppendLine($"            return null;");
        sb.AppendLine($"        }}");
        sb.AppendLine();
        foreach (var line in factoryBody)
        {
            sb.AppendLine($"        {line}");
        }
        sb.AppendLine();
        sb.AppendLine($"        return new({string.Join(", ", fields.Select(f => f.Name))});");
        sb.AppendLine($"    }}");
        sb.AppendLine($"}}");

        return sb.ToString();
    }
    
    private string TranslateFieldType(
        ITypeSymbol type,
        string name,
        out string readLogic,
        out bool isPointerTemplateClass,
        bool isNullable)
    {
        readLogic = string.Empty;
        isPointerTemplateClass = false;

        // Handle raw Pointer
        if (type.OriginalDefinition.ToDisplayString() == "MemoryBuilder.Pointer")
        {
            readLogic = $"var {name} = self.{name};";
            return "Pointer";
        }

        // Handle Pointer<T>
        if (type is INamedTypeSymbol named &&
            named.OriginalDefinition.ToDisplayString().StartsWith("MemoryBuilder.Pointer<"))
        {
            var inner = named.TypeArguments[0];

            // If inner type has [MemoryTarget] => treat as template class
            if (HasMemoryTarget(inner))
            {
                var className = GetTargetNameFromAttribute(inner);
                var call = $"{className}.TryBuildFromMemory(handle, self.{name})";
                readLogic = isNullable
                    ? $"var {name} = {call};"
                    : $"var {name}Option = {call};\n        if ({name}Option == null) return null;\n        var {name} = {name}Option;";
                return isNullable ? $"{className}?" : className;
            }

            // Otherwise treat as raw unmanaged value
            var innerType = inner.ToDisplayString();
            var readExpr = $"self.{name}.Read(handle, out var {name})";
            readLogic = isNullable
                ? $"var {name} = self.{name}.Read(handle, out var _{name}) ? _{name} : null;"
                : $"if (!{readExpr}) return null;";
            return isNullable ? $"{innerType}?" : innerType;
        }

        // Fallback to plain field
        readLogic = $"var {name} = self.{name};";
        return type.ToDisplayString();
    }
    
    private string GetTargetNameFromAttribute(ITypeSymbol symbol)
    {
        foreach (var attr in symbol.GetAttributes())
        {
            if (attr.AttributeClass?.Name == "MemoryTargetAttribute" &&
                attr.ConstructorArguments.Length == 1 &&
                attr.ConstructorArguments[0].Value is string targetName)
            {
                return targetName;
            }
        }

        throw new InvalidOperationException(
            $"Type {symbol.Name} does not have a valid [MemoryTarget(\"...\")] attribute.");
    }
    
    private bool HasMemoryTarget(ITypeSymbol symbol)
    {
        return symbol.GetAttributes().Any(attr =>
            attr.AttributeClass?.Name == "MemoryTargetAttribute");
    }
}
