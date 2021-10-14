using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using MongoDB.Analyzer.Core.Linq;
using System.Collections.Immutable;

namespace MongoDB.Analyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MongoDBDiagnosticAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "MongoDB.Analyzer";

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(LinqDiagnosticsRules.DiagnosticsRules);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterSemanticModelAction(LinqAnalyzer.AnalyzeIMongoQueryable);
        }
    }
}
