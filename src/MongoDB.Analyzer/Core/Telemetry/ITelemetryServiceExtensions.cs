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

namespace MongoDB.Analyzer.Core;

internal static class ITelemetryServicExtensions
{
    private static readonly string s_version;

    static ITelemetryServicExtensions()
    {
        try
        {
            s_version = Assembly.GetExecutingAssembly().GetName().Version.ToString().Replace('.', '_');
        }
        catch
        {
            s_version = "Unknown";
        }
    }

    public static void AnalysisStarted(
        this ITelemetryService telemetryService,
        SemanticModelAnalysisContext semanticModelAnalysisContext,
        MongoDBAnalyzerSettings settings)
    {
        var csharpCompilation = semanticModelAnalysisContext.SemanticModel.Compilation as CSharpCompilation;
        var csharpOptions = csharpCompilation.Options;

        var data = new (string, object)[]
        {
                ("output_kind", (csharpOptions?.OutputKind)?.ToString() ?? "Unknown"),
                ("output_platform", (csharpOptions?.Platform)?.ToString() ?? "Unknown"),
                ("lang_version", (csharpCompilation?.LanguageVersion)?.ToString() ?? "Unknown"),
                ("syntax_tree_length", semanticModelAnalysisContext.SemanticModel.SyntaxTree.Length),
                ("linq_version", settings.DefaultLinqVersion.ToString()),
                ("logs_enabled", settings.OutputInternalLogsToFile),
                ("analyzer_version", s_version)
        };

        telemetryService.Event("Analysis Started", data);
    }

    public static void LinqAnalysisResult(this ITelemetryService telemetryService, AnalysisTelemetry analysisStatistics) =>
        telemetryService.Event("LINQ analyzed", analysisStatistics.ToKeyValues());

    public static void BuildersAnalysisResult(this ITelemetryService telemetryService, AnalysisTelemetry analysisStatistics) =>
        telemetryService.Event("Builders analyzed", analysisStatistics.ToKeyValues());
}
