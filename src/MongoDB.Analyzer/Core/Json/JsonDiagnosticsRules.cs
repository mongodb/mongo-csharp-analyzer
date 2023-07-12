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

namespace MongoDB.Analyzer.Core.Json;

public static class JsonDiagnosticsRules
{
    private const string DiagnosticIdJson = "MAJson1001";
    private const string DiagnosticIdNotSupportedJson = "MAJson2001";
    private const string Category = "MongoDB.Analyzer.Json";

    public static readonly DiagnosticDescriptor DiagnosticRuleJson = new(
        id: DiagnosticIdJson,
        title: "POCO to Json",
        messageFormat: "{0}",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        helpLinkUri: "");

    public static readonly DiagnosticDescriptor DiagnosticRuleNotSupportedJson = new(
        id: DiagnosticIdNotSupportedJson,
        title: "Not supported POCO",
        messageFormat: "{0}",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        helpLinkUri: "");


    public static DiagnosticDescriptor[] DiagnosticsRules { get; } = new[]
    {
        DiagnosticRuleJson,
        DiagnosticRuleNotSupportedJson
    };
}