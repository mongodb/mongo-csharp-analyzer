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

namespace MongoDB.Analyzer.Core.Builders;

internal static class BuildersAnalyzer
{
    public static bool AnalyzeBuilders(MongoAnalysisContext context)
    {
        var sw = Stopwatch.StartNew();
        var stats = AnalysisStats.Empty;
        ExpressionsAnalysis buildersAnalysis = null;

        try
        {
            context.Logger.Log("Started builders analysis");

            buildersAnalysis = BuilderExpressionProcessor.ProcessSemanticModel(context);

            stats = ReportMqlOrInvalidExpressions(context, buildersAnalysis);

            sw.Stop();
            context.Logger.Log($"Builders analysis ended: with {stats.MqlCount} mql translations, {stats.DriverExceptionsCount} unsupported expressions, {stats.InternalExceptionsCount} internal exceptions in {sw.ElapsedMilliseconds}ms.");
        }
        catch (Exception ex)
        {
            sw.Stop();
            context.Logger.Log($"Builders analysis ended after {sw.ElapsedMilliseconds}ms with exception {ex}");
        }

        var telemetry = AnalysisUtilities.GetTelemetry(buildersAnalysis, stats, sw);
        if (telemetry.ExpressionsFound > 0)
        {
            context.Telemetry.BuildersAnalysisResult(AnalysisUtilities.GetTelemetry(buildersAnalysis, stats, sw));
        }

        return telemetry.ExpressionsFound > 0;
    }

    private static AnalysisStats ReportMqlOrInvalidExpressions(MongoAnalysisContext context, ExpressionsAnalysis buildersAnalysis)
    {
        var semanticContext = context.SemanticModelAnalysisContext;
        if (buildersAnalysis.AnalysisNodeContexts.EmptyOrNull())
        {
            return AnalysisStats.Empty;
        }

        var compilationResult = AnalysisCodeGenerator.Compile(context, buildersAnalysis);
        if (!compilationResult.Success)
        {
            return AnalysisStats.Empty;
        }

        var driverVersion = compilationResult.BuildersTestCodeExecutor.DriverVersion;
        var settings = context.Settings;
        int mqlCount = 0, internalExceptionsCount = 0, driverExceptionsCount = 0;
        var typesMapper = new TypesMapper(
            MqlGeneratorSyntaxElements.Builders.MqlGeneratorNamespace,
            context.TypesProcessor.GeneratedTypeToOriginalTypeMapping);

        foreach (var analysisContext in buildersAnalysis.AnalysisNodeContexts)
        {
            var mqlResult = compilationResult.BuildersTestCodeExecutor.GenerateMql(analysisContext.EvaluationMethodName);
            var locations = analysisContext.Node.Locations;

            if (mqlResult.Mql != null)
            {
                var mql = analysisContext.Node.ConstantsRemapper.RemapConstants(mqlResult.Mql);
                var diagnosticDescriptor = DiagnosticsRules.DiagnosticRuleBuilder2MQL;
                var decoratedMessage = AnalysisUtilities.DecorateMessage(mql, driverVersion, context.Settings);
                semanticContext.ReportDiagnostics(diagnosticDescriptor, decoratedMessage, locations);
                mqlCount++;
            }
            else if (mqlResult.Exception != null)
            {
                var isDriverOrBsonException = IsDriverOrBsonException(mqlResult);

                if (isDriverOrBsonException || settings.OutputInternalExceptions)
                {
                    var diagnosticDescriptor = DiagnosticsRules.DiagnosticRuleNotSupportedBuilderExpression;
                    var message = AnalysisUtilities.GetExceptionMessage(mqlResult.Exception, typesMapper, AnalysisType.Builders);
                    var decoratedMessage = AnalysisUtilities.DecorateMessage(message, driverVersion, context.Settings);

                    semanticContext.ReportDiagnostics(diagnosticDescriptor, decoratedMessage, locations);
                }

                if (!isDriverOrBsonException)
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

    private static bool IsDriverOrBsonException(MQLResult mqlResult)
    {
        var source = mqlResult.Exception.InnerException?.Source;
        return source.IsNotEmpty() && (source.Contains("MongoDB.Driver") ||
            source.Contains("MongoDB.Bson"));
    }
}
