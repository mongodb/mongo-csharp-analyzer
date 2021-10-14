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

namespace MongoDB.Analyzer.Core.Builders;

public static class BuidersDiagnosticsRules
{
    private const string DiagnosticIdBuilders2MQL = "MABuilders1001";
    private const string DiagnosticIdNotSupportedBuilderExpression = "MABuilders2001";
    private const string Category = "MongoDB.Analyzer.Builders";

    public static readonly DiagnosticDescriptor DiagnosticRuleBuilder2MQL = new(
        id: DiagnosticIdBuilders2MQL,
        title: "Builders to MQL",
        messageFormat: "{0}",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        helpLinkUri: "http://dochub.mongodb.org/core/mongodb-analyzer-diagnostic-rule-builders-2-mql-v-1-0");

    public static readonly DiagnosticDescriptor DiagnosticRuleNotSupportedBuilderExpression = new(
        id: DiagnosticIdNotSupportedBuilderExpression,
        title: "Not supported builders expression",
        messageFormat: "{0}",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        helpLinkUri: "http://dochub.mongodb.org/core/mongodb-analyzer-diagnostic-rule-not-supported-builders-expression-v-1-0");

    public static DiagnosticDescriptor[] DiagnosticsRules { get; } = new[]
    {
        DiagnosticRuleBuilder2MQL,
        DiagnosticRuleNotSupportedBuilderExpression
    };
}
