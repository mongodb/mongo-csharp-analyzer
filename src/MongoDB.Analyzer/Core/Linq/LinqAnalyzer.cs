using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Linq;

namespace MongoDB.Analyzer.Core.Linq
{
    public static class LinqAnalyzer
    {
        public static void AnalyzeIMongoQueryable(SemanticModelAnalysisContext context)
        {
            var linqAnalysis = LinqExpressionProcessor.ProcessSemanticModel(context);

            ReportInvalidExpressions(context, linqAnalysis);
            ReportMqlOrInvalidExpressions(context, linqAnalysis);
        }

        private static void ReportInvalidExpressions(SemanticModelAnalysisContext context, ExpressionsAnalysis linqExpressionAnalysis)
        {
            if (linqExpressionAnalysis.InvalidExpressionNodes.AnySafe())
            {
                foreach (var invalidLinqNode in linqExpressionAnalysis.InvalidExpressionNodes)
                {
                    var diagnostics = Diagnostic.Create(
                      LinqDiagnosticsRules.DiagnosticRuleNotSupportedLinqExpression,
                      invalidLinqNode.OriginaExpression.GetLocation(),
                      invalidLinqNode.Errors.FirstOrDefault());

                    context.ReportDiagnostic(diagnostics);
                }
            }
        }

        private static void ReportMqlOrInvalidExpressions(SemanticModelAnalysisContext context, ExpressionsAnalysis linqExpressionAnalysis)
        {
            if (linqExpressionAnalysis.AnalysisNodeContexts.EmptyOrNull())
                return;

            var semanticModel = context.SemanticModel;
            var parseOptions = new CSharpParseOptions(((CSharpParseOptions)semanticModel.SyntaxTree.Options).LanguageVersion);

            var compilationResult = AnalysisCodeGenerator.Compile(semanticModel, linqExpressionAnalysis, parseOptions);

            if (!compilationResult.Success)
                return;

            foreach (var analysisContext in linqExpressionAnalysis.AnalysisNodeContexts)
            {
                var (exception, mql) = compilationResult.LinqTestCodeExecutor.ExecuteMethod(analysisContext.EvaluationMethodName);
                var location = analysisContext.Node.OriginaExpression.GetLocation();

                if (exception != null)
                {
                    var diagnostics = Diagnostic.Create(
                       LinqDiagnosticsRules.DiagnosticRuleNotSupportedLinqExpression,
                       location,
                       (exception?.InnerException?.Message ?? "Unsupported linq expression"));

                    context.ReportDiagnostic(diagnostics);
                }
                else if (mql != null)
                {
                    var diagnostics = Diagnostic.Create(
                           LinqDiagnosticsRules.DiagnosticRuleLinq2MQL,
                           location,
                           mql);

                    context.ReportDiagnostic(diagnostics);
                }
            }
        }
    }
}
