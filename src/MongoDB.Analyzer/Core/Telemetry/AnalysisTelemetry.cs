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

internal record AnalysisTelemetry(
    int ExpressionsFound,
    int DataTypesCount,
    string DriverVersion,
    string Target,
    int MqlCount,
    int DriverExceptionCount,
    int InternalExceptionCount,
    long DurationMS)
{
    public static AnalysisTelemetry Invalid = new(-1, -1, null, null, -1, -1, -1, -1);
    public static AnalysisTelemetry Empty = new(0, 0, null, null, -1, -1, -1, -1);

    public (string, object)[] ToKeyValues() => new (string, object)[]
    {
            ("expressions_found_count", ExpressionsFound),
            ("data_types_count", DataTypesCount),
            ("driver_version", DriverVersion),
            ("mql_count", MqlCount),
            ("driver_exception_count", DriverExceptionCount),
            ("internal_exception_count", InternalExceptionCount),
            ("duration_ms", DurationMS)
    };
}
