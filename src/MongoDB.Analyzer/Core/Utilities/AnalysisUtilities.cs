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

internal static class AnalysisUtilities
{
    public static AnalysisTelemetry GetTelemetry(ExpressionsAnalysis expressionsAnalysis, AnalysisStats analysisStats, Stopwatch sw)
    {
        var expressionsCount = (expressionsAnalysis?.InvalidExpressionNodes?.Length ?? 0) + (expressionsAnalysis?.AnalysisNodeContexts?.Length ?? 0);
        var typesCount = expressionsAnalysis?.TypesDeclarations?.Length ?? 0;

        return new AnalysisTelemetry(
            expressionsCount,
            typesCount,
            analysisStats.DriverVersion,
            analysisStats.TargetFramework,
            analysisStats.MqlCount,
            analysisStats.DriverExceptionsCount,
            analysisStats.InternalExceptionsCount,
            sw.ElapsedMilliseconds);
    }
}
