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

namespace MongoDB.Analyzer.Core.Poco;

public static class PocoDiagnosticRules
{
    private const string DiagnosticIdPoco2Json = "MAPoco1001";
    private const string DiagnosticIdNotSupportedPoco = "MAPoco2001";
    private const string Category = "MongoDB.Analyzer.Poco";

    public static readonly DiagnosticDescriptor DiagnosticRulePoco2Json = new(
        id: DiagnosticIdPoco2Json,
        title: "Poco to Json",
        messageFormat: "{0}",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        helpLinkUri: "");

    public static readonly DiagnosticDescriptor DiagnosticRuleNotSupportedPoco = new(
        id: DiagnosticIdNotSupportedPoco,
        title: "Not supported Poco",
        messageFormat: "{0}",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        helpLinkUri: "");

    public static DiagnosticDescriptor[] DiagnosticsRules { get; } = new[]
    {
        DiagnosticRulePoco2Json,
        DiagnosticRuleNotSupportedPoco
    };
}