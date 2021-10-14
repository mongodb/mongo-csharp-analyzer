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

namespace MongoDB.Analyzer.Core.Linq;

public static class LinqDiagnosticsRules
{
    private const string DiagnosticIdLinq2MQL = "MALinq1001";
    private const string DiagnosticIdNotSupportedLinqExpression = "MALinq2001";
    private const string DiagnosticIdNotSupportedLinq2Expression = "MALinq2002";
    private const string Category = "MongoDB.Analyzer.LINQ";

    public static readonly DiagnosticDescriptor DiagnosticRuleLinq2MQL = new(
        id: DiagnosticIdLinq2MQL,
        title: "LINQ to MQL",
        messageFormat: "{0}",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        helpLinkUri: "http://dochub.mongodb.org/core/mongodb-analyzer-diagnostic-rule-linq-2-mql-v-1-0");

    public static readonly DiagnosticDescriptor DiagnosticRuleNotSupportedLinqExpression = new(
        id: DiagnosticIdNotSupportedLinqExpression,
        title: "Not supported LINQ expression",
        messageFormat: "{0}",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        helpLinkUri: "http://dochub.mongodb.org/core/mongodb-analyzer-diagnostic-rule-not-supported-linq-expression-v-1-0");

    public static readonly DiagnosticDescriptor DiagnosticRuleNotSupportedLinq2Expression = new(
         id: DiagnosticIdNotSupportedLinq2Expression,
         title: "Not supported LINQ2 expression",
         messageFormat: "Supported in LINQ3 only: {0}",
         category: Category,
         defaultSeverity: DiagnosticSeverity.Warning,
         isEnabledByDefault: true,
         helpLinkUri: "http://dochub.mongodb.org/core/mongodb-analyzer-diagnostic-rule-not-supported-linq-2-expression-v-1-0");

    public static DiagnosticDescriptor[] DiagnosticsRules { get; } = new[]
    {
        DiagnosticRuleLinq2MQL,
        DiagnosticRuleNotSupportedLinqExpression,
        DiagnosticRuleNotSupportedLinq2Expression,
    };
}
