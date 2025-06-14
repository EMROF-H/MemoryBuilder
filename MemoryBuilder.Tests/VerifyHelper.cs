using MemoryBuilder.Generator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;

namespace MemoryBuilder.Tests;

public static class VerifyHelper
{
    public class Test : CSharpSourceGeneratorTest<MemoryTargetGenerator, XUnitVerifier>
    {
        public Test()
        {
            ReferenceAssemblies = ReferenceAssemblies.NetStandard.NetStandard20;
            AddAssembly("MemoryBuilder.dll");
            AddAssembly("MemoryBuilder.Attributes.dll");
        }

        private void AddAssembly(string assemblyName)
        {
            var dllPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, assemblyName);
            if (!File.Exists(dllPath))
            {
                throw new FileNotFoundException($"Failed to find {assemblyName}", dllPath);
            }
            TestState.AdditionalReferences.Add(MetadataReference.CreateFromFile(dllPath));
        }
    }
}