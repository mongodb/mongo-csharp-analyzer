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

namespace MongoDB.Analyzer.Core.Linq;

internal static class LinqAnalyzer
{
    public static bool AnalyzeIMongoQueryable(MongoAnalyzerContext context)
    {
        var sw = Stopwatch.StartNew();
        var stats = AnalysisStats.Empty;
        ExpressionsAnalysis linqAnalysis = null;

        try
        {
            context.Logger.Log("Started LINQ analysis");

            linqAnalysis = LinqExpressionProcessor.ProcessSemanticModel(context);

            ReportInvalidExpressions(context, linqAnalysis);
            stats = ReportMqlOrInvalidExpressions(context, linqAnalysis);

            sw.Stop();
            context.Logger.Log($"LINQ analysis ended: with {stats.MqlCount} mql translations, {stats.DriverExceptionsCount} unsupported expressions, {stats.InternalExceptionsCount} internal exceptions in {sw.ElapsedMilliseconds}.");
        }
        catch (Exception ex)
        {
            sw.Stop();
            context.Logger.Log($"LINQ analysis ended after {sw.ElapsedMilliseconds}ms with exception {ex}");
        }

        var telemetry = AnalysisUtilities.GetTelemetry(linqAnalysis, stats, sw);
        if (telemetry.ExpressionsFound > 0)
        {
            context.Telemetry.LinqAnalysisResult(telemetry);
        }

        return telemetry.ExpressionsFound > 0;
    }

    private static void ReportInvalidExpressions(MongoAnalyzerContext context, ExpressionsAnalysis linqExpressionAnalysis)
    {
        var semanticContext = context.SemanticModelAnalysisContext;

        if (linqExpressionAnalysis.InvalidExpressionNodes.EmptyOrNull())
        {
            return;
        }

        var driverVersion = ReferencesProvider.GetMongoDBDriverVersion(semanticContext.SemanticModel.Compilation.References);
        var driverVersionString = driverVersion?.ToString(3);

        foreach (var invalidLinqNode in linqExpressionAnalysis.InvalidExpressionNodes)
        {
            var diagnostics = Diagnostic.Create(
                LinqDiagnosticsRules.DiagnosticRuleNotSupportedLinqExpression,
                invalidLinqNode.OriginalExpression.GetLocation(),
                DecorateMessage(invalidLinqNode.Errors.FirstOrDefault(), driverVersionString, context.Settings));

            semanticContext.ReportDiagnostic(diagnostics);
        }
    }

    private static AnalysisStats ReportMqlOrInvalidExpressions(MongoAnalyzerContext context, ExpressionsAnalysis linqExpressionAnalysis)
    {
        var semanticContext = context.SemanticModelAnalysisContext;

        if (linqExpressionAnalysis.AnalysisNodeContexts.EmptyOrNull())
        {
            return AnalysisStats.Empty;
        }

        var compilationResult = AnalysisCodeGenerator.Compile(context, linqExpressionAnalysis);
        if (!compilationResult.Success)
        {
            return AnalysisStats.Empty;
        }

        var driverVersion = compilationResult.LinqTestCodeExecutor.DriverVersion;
        var settings = context.Settings;
        int mqlCount = 0, internalExceptionsCount = 0, driverExceptionsCount = 0;

        foreach (var analysisContext in linqExpressionAnalysis.AnalysisNodeContexts)
        {
            var mqlResult = compilationResult.LinqTestCodeExecutor.GenerateMql(analysisContext.EvaluationMethodName);
            var location = analysisContext.Node.OriginalExpression.GetLocation();

            if (mqlResult.Mql != null)
            {
                var mql = analysisContext.Node.ConstantsRemapper.RemapConstants(mqlResult.Mql);

                var diagnostics = Diagnostic.Create(
                    mqlResult.Linq3Only ? LinqDiagnosticsRules.DiagnosticRuleNotSupportedLinq2Expression : LinqDiagnosticsRules.DiagnosticRuleLinq2MQL,
                    location,
                    DecorateMessage(mql, driverVersion, settings));

                semanticContext.ReportDiagnostic(diagnostics);
                mqlCount++;
            }
            else if (mqlResult.Exception != null)
            {
                var isDriverException = mqlResult.Exception.InnerException?.Source?.Contains("MongoDB.Driver") == true;

                if (isDriverException || settings.OutputInternalExceptions)
                {
                    var diagnostics = Diagnostic.Create(
                        LinqDiagnosticsRules.DiagnosticRuleNotSupportedLinqExpression,
                        location,
                        DecorateMessage(mqlResult.Exception.InnerException?.Message ?? "Unsupported LINQ expression", driverVersion, settings));

                    semanticContext.ReportDiagnostic(diagnostics);
                }

                if (!isDriverException)
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

        return new AnalysisStats(mqlCount, internalExceptionsCount, driverExceptionsCount, compilationResult.MongoDBDriverVersion.ToString(3), null);
    }

    private static string DecorateMessage(string message, string driverVersion, MongoDBAnalyzerSettings settings) =>
        settings.OutputDriverVersion ? $"{message}_v{driverVersion}" : message;
}
