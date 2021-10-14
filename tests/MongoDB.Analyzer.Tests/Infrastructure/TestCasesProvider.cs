
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Testing;
using MongoDB.Analyzer.Core.Linq;
using MongoDB.Analyzer.Tests.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MongoDB.Analyzer.Tests.Infrastructure
{
    public static class TestCasesProvider
    {
        public static async Task<IDictionary<string, DiagnosticsTestCase>> ExtractTestCaseFromFile(string testcasesFilename)
        {
            var testClassLocation = PathUtils.GetTestCaseFileFullPath(testcasesFilename);

#if NET472
            var netReferences = ReferenceAssemblies.NetFramework.Net472.Default;
#elif NETCOREAPP2_1
            var netReferences = ReferenceAssemblies.NetCore.NetCoreApp21;
#elif NETCOREAPP3_1
            var netReferences = ReferenceAssemblies.NetCore.NetCoreApp31;
#endif
            var diagnostics = await MongoDBAnalyzerVerifier.Analyze(testClassLocation, netReferences);

            var diagnosticsAndMethodNodes = diagnostics.
                Where(d => d.Location.IsInSource && d.Severity < DiagnosticSeverity.Error).
                Select(d =>
                    (Diagnostic: d,
                     MethodNode: d.Location.SourceTree.
                        GetRoot().
                        FindNode(d.Location.SourceSpan).
                        Ancestors().
                        OfType<MethodDeclarationSyntax>().
                        First())).
                ToArray();

            var result = diagnosticsAndMethodNodes.
                GroupBy(pair => pair.MethodNode.Identifier.Text).
                ToDictionary(
                    group => group.Key,
                    group => new DiagnosticsTestCase(
                        group.Key,
                        group.Select(pair => pair.Diagnostic).ToArray(),
                        ExtractDiagnostics(group.First().MethodNode)));

            Diagnostic[] ExtractDiagnostics(MethodDeclarationSyntax methodDeclarationSyntax)
            {
                var commentTrivias = methodDeclarationSyntax.
                    GetLeadingTrivia().
                    Where(t => t.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.SingleLineCommentTrivia)).
                    ToArray();

                var diagnostic = Diagnostic.Create(LinqDiagnosticsRules.DiagnosticRuleLinq2MQL, Location.None, commentTrivias[1].ToFullString().Trim('/', ' '));
                var result = new[]
                {
                    diagnostic
                };

                return result;
            }

            return result;
        }
    }
}
