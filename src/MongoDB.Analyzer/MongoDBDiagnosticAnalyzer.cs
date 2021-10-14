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

using MongoDB.Analyzer.Core;
using MongoDB.Analyzer.Core.Builders;
using MongoDB.Analyzer.Core.Linq;

namespace MongoDB.Analyzer;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class MongoDBDiagnosticAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => CollectionExtensions.CreateImmutableArray(
        LinqDiagnosticsRules.DiagnosticsRules,
        BuidersDiagnosticsRules.DiagnosticsRules);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterSemanticModelAction(SemanticModelAction);
    }

    public static void SemanticModelAction(SemanticModelAnalysisContext context)
    {
        var settings = SettingsHelper.GetSettings(context.Options);
        var correlationId = CorrelationId.Next();

        using var telemetryService = SettingsHelper.CreateTelemetryService(settings, correlationId);
        using var logger = SettingsHelper.CreateLogger(settings, correlationId);

        var mongoAnalyzerContext = new MongoAnalyzerContext(context, settings, logger, telemetryService);
        var flushTelemetry = false;

        try
        {
            telemetryService.AnalysisStarted(context, settings);
            logger.Log($"Analysis started, analyzer version: {Assembly.GetExecutingAssembly().GetName().Version}, file: {GetFilePath(context)}");

            telemetryService.AnalysisStarted(context, settings);

            flushTelemetry |= LinqAnalyzer.AnalyzeIMongoQueryable(mongoAnalyzerContext);
            flushTelemetry |= BuildersAnalyzer.AnalyzeBuilders(mongoAnalyzerContext);
        }
        catch (Exception ex)
        {
            logger.Log($"Unhandled exception {ex}");
            flushTelemetry = true;
        }
        finally
        {
            if (flushTelemetry)
            {
                try
                {
                    telemetryService.Flush();
                }
                catch (Exception ex)
                {
                    logger.Log($"Telemetry flushing failed {ex}");
                }
            }
        }
    }

    private static string GetFilePath(SemanticModelAnalysisContext context)
    {
        try
        {
            return Path.GetFileName(context.SemanticModel.SyntaxTree.FilePath);
        }
        catch
        {
            return "Unknown";
        }
    }
}
