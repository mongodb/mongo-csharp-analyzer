// Copyright 2021-present MongoDB Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License")
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using MongoDB.Analyzer.Core.HelperResources;
using MongoDB.Analyzer.Core.Utilities;

namespace MongoDB.Analyzer.Core.Linq;

internal static class EFAnalyzer
{
    public static bool AnalyzeEFQueryable(MongoAnalysisContext context)
    {
        var sw = Stopwatch.StartNew();
        var stats = AnalysisStats.Empty;
        ExpressionsAnalysis EFAnalysis = null;

        try
        {
            context.Logger.Log("Started EF analysis");

            EFAnalysis = LinqExpressionProcessor.ProcessSemanticModel(context, AnalysisType.EF);

            ReportInvalidExpressions(context, EFAnalysis);
            stats = ReportMqlOrInvalidExpressions(context, EFAnalysis);

            sw.Stop();
            context.Logger.Log($"EF analysis ended: with {stats.MqlCount} mql translations, {stats.DriverExceptionsCount} unsupported expressions, {stats.InternalExceptionsCount} internal exceptions in {sw.ElapsedMilliseconds}ms.");
        }
        catch (Exception ex)
        {
            sw.Stop();
            context.Logger.Log($"EF analysis ended after {sw.ElapsedMilliseconds} ms with exception {ex}");
        }

        var telemetry = AnalysisUtilities.GetTelemetry(EFAnalysis, stats, sw);
        if (telemetry.ExpressionsFound > 0)
        {
            context.Telemetry.EFAnalysisResult(telemetry);
        }

        return telemetry.ExpressionsFound > 0;
    }

    private static void ReportInvalidExpressions(MongoAnalysisContext context, ExpressionsAnalysis EFExpressionAnalysis)
    {
        var semanticContext = context.SemanticModelAnalysisContext;

        if (EFExpressionAnalysis.InvalidExpressionNodes.EmptyOrNull())
        {
            return;
        }

        var driverVersion = ReferencesProvider.GetMongoDBDriverVersion(semanticContext.SemanticModel.Compilation.References);
        var driverVersionString = driverVersion?.ToString(3);

        foreach (var invalidLinqNode in EFExpressionAnalysis.InvalidExpressionNodes)
        {
            var diagnostics = Diagnostic.Create(
                DiagnosticsRules.DiagnosticRuleNotSupportedLinqExpression,
                invalidLinqNode.OriginalExpression.GetLocation(),
                AnalysisUtilities.DecorateMessage(invalidLinqNode.Errors.FirstOrDefault(), driverVersionString, context.Settings));

            semanticContext.ReportDiagnostic(diagnostics);
        }
    }

    private static AnalysisStats ReportMqlOrInvalidExpressions(MongoAnalysisContext context, ExpressionsAnalysis EFExpressionAnalysis)
    {
        var semanticContext = context.SemanticModelAnalysisContext;

        if (EFExpressionAnalysis.AnalysisNodeContexts.EmptyOrNull())
        {
            return AnalysisStats.Empty;
        }

        var compilationResult = AnalysisCodeGenerator.Compile(context, EFExpressionAnalysis);
        if (!compilationResult.Success)
        {
            return AnalysisStats.Empty;
        }

        var driverVersion = compilationResult.LinqTestCodeExecutor.DriverVersion;
        var settings = context.Settings;
        int mqlCount = 0, internalExceptionsCount = 0, driverExceptionsCount = 0;
        var typesMapper = new TypesMapper(
          MqlGeneratorSyntaxElements.Linq.MqlGeneratorNamespace,
          context.TypesProcessor.GeneratedTypeToOriginalTypeMapping);

        foreach (var analysisContext in EFExpressionAnalysis.AnalysisNodeContexts)
        {
            var mqlResult = compilationResult.LinqTestCodeExecutor.GenerateMql(analysisContext.EvaluationMethodName);
            var locations = analysisContext.Node.Locations;

            if (mqlResult.Mql != null)
            {
                var mql = analysisContext.Node.ConstantsRemapper.RemapConstants(mqlResult.Mql);
                var diagnosticDescriptor = mqlResult.Linq3Only ? DiagnosticsRules.DiagnosticRuleNotSupportedEFExpression : DiagnosticsRules.DiagnosticRuleEF2MQL;
                var decoratedMessage = AnalysisUtilities.DecorateMessage(mql, driverVersion, settings);
                semanticContext.ReportDiagnostics(diagnosticDescriptor, decoratedMessage, locations);
                mqlCount++;
            }
            else if (mqlResult.Exception != null)
            {
                var isDriverOrLinqException = AnalysisUtilities.IsDriverOrLinqException(mqlResult);

                if (isDriverOrLinqException || settings.OutputInternalExceptions)
                {
                    var diagnosticDescriptor = DiagnosticsRules.DiagnosticRuleNotSupportedLinqExpression;
                    var message = AnalysisUtilities.GetExceptionMessage(mqlResult.Exception, typesMapper, AnalysisType.Linq);
                    var decoratedMessage = AnalysisUtilities.DecorateMessage(message, driverVersion, context.Settings);

                    semanticContext.ReportDiagnostics(diagnosticDescriptor, decoratedMessage, locations);
                }

                if (!isDriverOrLinqException)
                {
                    context.Logger.Log($"Exception while analyzing {analysisContext.Node}: {mqlResult.Exception.InnerException?.Message}");
                    internalExceptionsCount++;
                }
                else
                {
                    driverExceptionsCount++;
                }
            }
        }

        return new AnalysisStats(mqlCount, 0, internalExceptionsCount, driverExceptionsCount, compilationResult.MongoDBDriverVersion.ToString(3), null);
    }
}
