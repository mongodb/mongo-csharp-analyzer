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

namespace MongoDB.Analyzer.Core.Poco;

internal static class PocoAnalyzer
{
    public static bool AnalyzePoco(MongoAnalysisContext context)
    {
        var sw = Stopwatch.StartNew();
        var stats = AnalysisStats.Empty;
        ExpressionsAnalysis pocoAnalysis = null;

        try
        {
            context.Logger.Log("Started JSON analysis");

            pocoAnalysis = PocoExpressionProcessor.ProcessSemanticModel(context);
            stats = ReportJsonOrInvalidExpressions(context, pocoAnalysis);

            sw.Stop();
            context.Logger.Log($"JSON analysis ended: with {stats.JsonCount} JSON translations, {stats.DriverExceptionsCount} unsupported expressions, {stats.InternalExceptionsCount} internal exceptions in {sw.ElapsedMilliseconds}ms.");
        }
        catch (Exception ex)
        {
            sw.Stop();
            context.Logger.Log($"JSON analysis ended after {sw.ElapsedMilliseconds}ms with exception {ex}");
        }

        var telemetry = AnalysisUtilities.GetTelemetry(pocoAnalysis, stats, sw);
        if (telemetry.ExpressionsFound > 0)
        {
            context.Telemetry.PocoAnalysisResult(AnalysisUtilities.GetTelemetry(pocoAnalysis, stats, sw));
        }

        return telemetry.ExpressionsFound > 0;
    }

    private static AnalysisStats ReportJsonOrInvalidExpressions(MongoAnalysisContext context, ExpressionsAnalysis pocoAnalysis)
    {
        var semanticContext = context.SemanticModelAnalysisContext;
        if ((pocoAnalysis?.AnalysisNodeContexts).EmptyOrNull())
        {
            return AnalysisStats.Empty;
        }

        var compilationResult = AnalysisCodeGenerator.Compile(context, pocoAnalysis);
        if (!compilationResult.Success)
        {
            return AnalysisStats.Empty;
        }

        var driverVersion = compilationResult.PocoTestCodeExecutor.DriverVersion;
        var settings = context.Settings;
        int jsonCount = 0, internalExceptionsCount = 0, driverExceptionsCount = 0;
        var typesMapper = new TypesMapper(
            JsonSyntaxElements.Poco.JsonGeneratorNamespace,
            context.TypesProcessor.GeneratedTypeToOriginalTypeMapping);

        foreach (var analysisContext in pocoAnalysis.AnalysisNodeContexts)
        {
            var jsonResult = compilationResult.PocoTestCodeExecutor.GenerateJson(analysisContext.EvaluationMethodName);
            var locations = analysisContext.Node.Locations;

            if (jsonResult.Json != null)
            {
                var decoratedMessage = AnalysisUtilities.DecorateMessage(jsonResult.Json, driverVersion, context.Settings);
                semanticContext.ReportDiagnostics(PocoDiagnosticRules.DiagnosticRulePoco2Json, decoratedMessage, locations);
                jsonCount++;
            }
            else if (jsonResult.Exception != null)
            {
                var isDriverOrBsonException = IsDriverOrBsonException(jsonResult);

                if (isDriverOrBsonException || settings.OutputInternalExceptions)
                {
                    var diagnosticDescriptor = PocoDiagnosticRules.DiagnosticRuleNotSupportedPoco;
                    var message = AnalysisUtilities.GetExceptionMessage(jsonResult.Exception, typesMapper, AnalysisType.Poco);
                    var decoratedMessage = AnalysisUtilities.DecorateMessage(message, driverVersion, context.Settings);

                    semanticContext.ReportDiagnostics(diagnosticDescriptor, decoratedMessage, locations);
                }

                if (!isDriverOrBsonException)
                {
                    context.Logger.Log($"Exception while analyzing {analysisContext.Node}: {jsonResult.Exception.InnerException?.Message}");
                    internalExceptionsCount++;
                }
                else
                {
                    driverExceptionsCount++;
                }
            }
        }

        return new AnalysisStats(0, jsonCount, internalExceptionsCount, driverExceptionsCount, compilationResult.MongoDBDriverVersion.ToString(3), null);
    }

    private static bool IsDriverOrBsonException(JsonResult jsonResult)
    {
        var source = jsonResult.Exception.InnerException?.Source;
        return source.IsNotEmpty() && (source.Contains("MongoDB.Driver") ||
            source.Contains("MongoDB.Bson"));
    }
}
