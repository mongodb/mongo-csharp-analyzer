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

using System.ComponentModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MongoDB.Analyzer.Core;

[JsonConverter(typeof(StringEnumConverter))]
internal enum LinqVersion
{
    V2,
    V3
}

internal record MongoDBAnalyzerSettings(
    [DefaultValue(false)] bool OutputDriverVersion = false,
    [DefaultValue(false)] bool OutputInternalExceptions = false,
    [DefaultValue(false)] bool OutputInternalLogsToFile = false,
    [DefaultValue(null)] string LogFileName = null,
    [DefaultValue(true)] bool SendTelemetry = true,
    [DefaultValue(LinqVersion.V2)] LinqVersion DefaultLinqVersion = LinqVersion.V2)
{
}
