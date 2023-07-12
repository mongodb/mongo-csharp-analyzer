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

using MongoDB.Analyzer.Core.Utilities;
namespace MongoDB.Analyzer.Core.Json;

internal static class JsonAnalyzer
{
    public static bool AnalyzeJson(MongoAnalysisContext context)
    {
        var sw = Stopwatch.StartNew();
        var stats = JsonAnalysisStats.Empty;
        JsonExpressionAnalysis jsonAnalysis = null;

        try
        {
            context.Logger.Log("Started JSON analysis");

            jsonAnalysis = JsonExpressionProcessor.ProcessSemanticModel(context);
            stats = ReportJsonOrInvalidExpressions(context, jsonAnalysis);

            sw.Stop();
            context.Logger.Log($"JSON analysis ended: with {stats.JsonCount} JSON translations, {stats.DriverExceptionsCount} unsupported expressions, {stats.InternalExceptionsCount} internal exceptions in {sw.ElapsedMilliseconds}.");
        }
        catch (Exception ex)
        {
            sw.Stop();
            context.Logger.Log($"JSON analysis ended after {sw.ElapsedMilliseconds}ms with exception {ex}");
        }

        var telemetry = AnalysisUtilities.GetJsonTelemetry(jsonAnalysis, stats, sw);
        if (telemetry.ExpressionsFound > 0)
        {
            context.Telemetry.JsonAnalysisResult(AnalysisUtilities.GetJsonTelemetry(jsonAnalysis, stats, sw));
        }

        return telemetry.ExpressionsFound > 0;
    }

    private static JsonAnalysisStats ReportJsonOrInvalidExpressions(MongoAnalysisContext context, JsonExpressionAnalysis jsonAnalysis)
    {
        var semanticContext = context.SemanticModelAnalysisContext;
        if (jsonAnalysis.AnalysisNodeContexts.EmptyOrNull())
        {
            return JsonAnalysisStats.Empty;
        }

        var compilationResult = AnalysisCodeGenerator.Compile(context, jsonAnalysis);
        if (!compilationResult.Success)
        {
            return JsonAnalysisStats.Empty;
        }

        var driverVersion = compilationResult.JsonTestCodeExecutor.DriverVersion;
        var settings = context.Settings;
        int jsonCount = 0, internalExceptionsCount = 0, driverExceptionsCount = 0;

        foreach (var analysisContext in jsonAnalysis.AnalysisNodeContexts)
        {
            var jsonResult = compilationResult.JsonTestCodeExecutor.GenerateJson(analysisContext.EvaluationMethodName);
            var locations = analysisContext.Node.Locations;

            if (jsonResult.Json != null)
            {
                var json = jsonResult.Json;
                var diagnosticDescriptor = JsonDiagnosticsRules.DiagnosticRuleJson;
                var decoratedMessage = DecorateMessage(json, driverVersion, context.Settings);
                semanticContext.ReportDiagnostics(diagnosticDescriptor, decoratedMessage, locations);
                jsonCount++;
            }
            else if (jsonResult.exception != null)
            {
                var isDriverException = jsonResult.exception.InnerException?.Source?.Contains("MongoDB.Driver") == true;
                var isBsonException = jsonResult.exception.InnerException?.Source?.Contains("MongoDB.Bson") == true;

                if (isBsonException || isDriverException || settings.OutputInternalExceptions)
                {
                    var diagnosticDescriptor = JsonDiagnosticsRules.DiagnosticRuleNotSupportedJson;
                    var decoratedMessage = DecorateMessage(jsonResult.exception.InnerException?.Message ?? "Unsupported JSON expression", driverVersion, context.Settings);
                    semanticContext.ReportDiagnostics(diagnosticDescriptor, decoratedMessage, locations);
                }

                if (!isDriverException)
                {
                    context.Logger.Log($"Exception while analyzing {analysisContext.Node}: {jsonResult.exception.InnerException?.Message}");
                    internalExceptionsCount++;
                }
                else
                {
                    driverExceptionsCount++;
                }
            }
        }

        return new JsonAnalysisStats(jsonCount, internalExceptionsCount, driverExceptionsCount, compilationResult.MongoDBDriverVersion.ToString(3), null);
    }

    private static string DecorateMessage(string message, string driverVersion, MongoDBAnalyzerSettings settings) =>
        settings.OutputDriverVersion ? $"{message}_v{driverVersion}" : message;
}