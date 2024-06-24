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

namespace MongoDB.Analyzer;

internal static class DiagnosticsRules
{
    private const string DocumentationRulesBaseLink = "https://www.mongodb.com/docs/mongodb-analyzer/current/rules";

    // Builders
    private const string BuilderCategory = "MongoDB.Analyzer.Builders";
    private const string DiagnosticIdBuilders2MQL = "MABuilders1001";
    private const string DiagnosticIdNotSupportedBuilderExpression = "MABuilders2001";

    //EF
    private const string DiagnosticIdEF2MQL = "MAEF1001";
    private const string DiagnosticIdNotSupportedEFExpression = "MAEF2001";
    private const string EFCategory = "MongoDB.Analyzer.EF";

    // LINQ
    private const string DiagnosticIdLinq2MQL = "MALinq1001";
    private const string DiagnosticIdNotSupportedLinqExpression = "MALinq2001";
    private const string DiagnosticIdNotSupportedLinq2Expression = "MALinq2002";
    private const string LinqCategory = "MongoDB.Analyzer.LINQ";

    // POCO
    private const string DiagnosticIdPoco2Json = "MAPoco1001";
    private const string DiagnosticIdNotSupportedPoco = "MAPoco2001";
    private const string PocoCategory = "MongoDB.Analyzer.Poco";

    // Builders
    public static DiagnosticDescriptor DiagnosticRuleBuilder2MQL { get; } = new DiagnosticDescriptor(
        id: DiagnosticIdBuilders2MQL,
        title: "Builders to MQL",
        messageFormat: "{0}",
        category: BuilderCategory,
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        helpLinkUri: GetRuleUrl(DiagnosticIdBuilders2MQL));

    public static DiagnosticDescriptor DiagnosticRuleNotSupportedBuilderExpression { get; } = new DiagnosticDescriptor(
        id: DiagnosticIdNotSupportedBuilderExpression,
        title: "Not supported builders expression",
        messageFormat: "{0}",
        category: BuilderCategory,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        helpLinkUri: GetRuleUrl(DiagnosticIdNotSupportedBuilderExpression));

    //EF
    public static DiagnosticDescriptor DiagnosticRuleEF2MQL { get; } = new DiagnosticDescriptor(
        id: DiagnosticIdEF2MQL,
        title: "EF to MQL",
        messageFormat: "{0}",
        category: EFCategory,
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        helpLinkUri: GetRuleUrl(DiagnosticIdEF2MQL));

    public static DiagnosticDescriptor DiagnosticRuleNotSupportedEFExpression { get; } = new DiagnosticDescriptor(
        id: DiagnosticIdNotSupportedEFExpression,
        title: "Not supported EF Expression",
        messageFormat: "{0}",
        category: EFCategory,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        helpLinkUri: GetRuleUrl(DiagnosticIdNotSupportedEFExpression));

    // LINQ
    public static DiagnosticDescriptor DiagnosticRuleLinq2MQL { get; } = new DiagnosticDescriptor(
        id: DiagnosticIdLinq2MQL,
        title: "LINQ to MQL",
        messageFormat: "{0}",
        category: LinqCategory,
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        helpLinkUri: GetRuleUrl(DiagnosticIdLinq2MQL));

    public static DiagnosticDescriptor DiagnosticRuleNotSupportedLinqExpression { get; } = new DiagnosticDescriptor(
        id: DiagnosticIdNotSupportedLinqExpression,
        title: "Not supported LINQ expression",
        messageFormat: "{0}",
        category: LinqCategory,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        helpLinkUri: GetRuleUrl(DiagnosticIdNotSupportedLinqExpression));

    public static DiagnosticDescriptor DiagnosticRuleNotSupportedLinq2Expression { get; } = new DiagnosticDescriptor(
         id: DiagnosticIdNotSupportedLinq2Expression,
         title: "Not supported LINQ2 expression",
         messageFormat: "Supported in LINQ3 only: {0}",
         category: LinqCategory,
         defaultSeverity: DiagnosticSeverity.Warning,
         isEnabledByDefault: true,
         helpLinkUri: GetRuleUrl(DiagnosticIdNotSupportedLinq2Expression));

    // POCO
    public static DiagnosticDescriptor DiagnosticRulePoco2Json { get; } = new DiagnosticDescriptor(
        id: DiagnosticIdPoco2Json,
        title: "POCO to Json",
        messageFormat: "{0}",
        category: PocoCategory,
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        helpLinkUri: GetRuleUrl(DiagnosticIdPoco2Json));

    public static DiagnosticDescriptor DiagnosticRuleNotSupportedPoco { get; } = new DiagnosticDescriptor(
        id: DiagnosticIdNotSupportedPoco,
        title: "Not supported POCO",
        messageFormat: "{0}",
        category: PocoCategory,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        helpLinkUri: GetRuleUrl(DiagnosticIdNotSupportedPoco));

    public static ImmutableArray<DiagnosticDescriptor> Rules { get; } = CollectionExtensions.CreateImmutableArray(new[]
        {
            // Builders
            DiagnosticRuleBuilder2MQL,
            DiagnosticRuleNotSupportedBuilderExpression,

            //EF
            DiagnosticRuleEF2MQL,
            DiagnosticRuleNotSupportedEFExpression,

            // LINQ
            DiagnosticRuleLinq2MQL,
            DiagnosticRuleNotSupportedLinqExpression,
            DiagnosticRuleNotSupportedLinq2Expression,

            // POCO
            DiagnosticRulePoco2Json,
            DiagnosticRuleNotSupportedPoco
        });

    private static string GetRuleUrl(string ruleId) => $"{DocumentationRulesBaseLink}/#{ruleId}";
}
