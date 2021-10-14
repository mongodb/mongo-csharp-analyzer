using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;
using System.Collections.Immutable;
using System.IO;
using System.Threading.Tasks;

namespace MongoDB.Analyzer.Tests.Helpers
{
    public static class MongoDBAnalyzerVerifier
    {
        public static async Task<ImmutableArray<Diagnostic>> Analyze(string testCodeFilename, ReferenceAssemblies referenceAssemblies)
        {
            var packages = ImmutableArray.Create(new PackageIdentity("MongoDB.Driver", "2.12.2"));

            var allReferences = referenceAssemblies.
                AddPackages(packages).
                AddAssemblies(ImmutableArray.Create(PathUtils.TestDataModelAssemblyPathCasesPath));

            var metadataReferences = await allReferences.ResolveAsync(LanguageNames.CSharp, default);

            var testCodeText = File.ReadAllText(testCodeFilename);
            var testCodeSyntaxTree = CSharpSyntaxTree.ParseText(testCodeText);

            var compilation = CSharpCompilation.Create(
                "TestAssembly",
                new[] { testCodeSyntaxTree },
                metadataReferences);

            var mongodbAnalyzer = new MongoDBDiagnosticAnalyzer();

            var compilationWithAnalyzers = compilation.WithAnalyzers(ImmutableArray.Create<DiagnosticAnalyzer>(mongodbAnalyzer));
            var diagnostics = await compilationWithAnalyzers.GetAllDiagnosticsAsync();

            return diagnostics;
        }
    }
}
