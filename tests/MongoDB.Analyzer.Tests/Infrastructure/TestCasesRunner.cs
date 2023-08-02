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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MongoDB.Analyzer.Core.Linq;
using MongoDB.Analyzer.Tests.Common;

namespace MongoDB.Analyzer.Tests.Infrastructure;

internal static class TestCasesRunner
{
    private static readonly string s_poco2JsonTestPath = Path.Combine("tests", "MongoDB.Analyzer.Tests.Common.TestCases", "Poco");

    private record TestsBundleKey(string TestFileName, string DriverVersion, LinqVersion LinqVersion, JsonAnalyzerVerbosity JsonAnalyzerVerbosity);

    private static readonly IDictionary<TestsBundleKey, IDictionary<string, DiagnosticTestCaseResult>>
        s_testResults = new Dictionary<TestsBundleKey, IDictionary<string, DiagnosticTestCaseResult>>();

    private static readonly HashSet<string> s_rulesAttributesNames = new(new[]
    {
            nameof(MQLAttribute),
            nameof(InvalidLinqAttribute),
            nameof(MQLAttribute).Replace(nameof(Attribute), string.Empty),
            nameof(InvalidLinqAttribute).Replace(nameof(Attribute), string.Empty),
        });

    private static readonly IDictionary<string, DiagnosticDescriptor> s_idToRuleMapping =
        LinqDiagnosticsRules.DiagnosticsRules.ToDictionary(r => r.Id);

    public static async Task<DiagnosticTestCaseResult> RunTestCase(DiagnosticTestCase diagnosticTestCase)
    {
        var testCollectionKey = new TestsBundleKey(diagnosticTestCase.FileName, diagnosticTestCase.Version, diagnosticTestCase.LinqVersion, diagnosticTestCase.JsonAnalyzerVerbosity);

        if (!s_testResults.TryGetValue(testCollectionKey, out var testCasesResults))
        {
            testCasesResults = await ExecuteTestCases(testCollectionKey);

            s_testResults[testCollectionKey] = testCasesResults;
        }

        testCasesResults.TryGetValue(diagnosticTestCase.MethodName, out var result);
        return result;
    }

    private static async Task<IDictionary<string, DiagnosticTestCaseResult>> ExecuteTestCases(TestsBundleKey testsBundleKey)
    {
        var isPoco2JsonTest = testsBundleKey.TestFileName.Contains(s_poco2JsonTestPath);

        var diagnostics = await DiagnosticsAnalyzer.Analyze(
            testsBundleKey.TestFileName,
            testsBundleKey.DriverVersion,
            testsBundleKey.LinqVersion,
            testsBundleKey.JsonAnalyzerVerbosity);

        var diagnosticsAndMethodNodes = diagnostics
            .Where(d => DiagnosticRulesConstants.AllRules.Contains(d.Descriptor.Id))
            .Select(d => (Diagnostic: d, MethodNode: FindMethodNode(d, isPoco2JsonTest)))
            .Where(d => d.MethodNode != null)
            .ToArray();

        var result = diagnosticsAndMethodNodes
            .GroupBy(pair => pair.MethodNode.Identifier.Text)
            .ToDictionary(
                group => group.Key,
                group => new DiagnosticTestCaseResult(
                    group.Key,
                    GetMethodLocation(group.FirstOrDefault().MethodNode),
                    group.Select(pair => pair.Diagnostic).ToArray()));

        int GetMethodLocation(MethodDeclarationSyntax node) =>
            node.DescendantNodes().OfType<BlockSyntax>().First().GetLocation().GetLineSpan().StartLinePosition.Line;

        return result;
    }

    private static MethodDeclarationSyntax FindMethodNode(Diagnostic diagnostic, bool isPoco2JsonTest)
    {
        var syntaxRoot = diagnostic.Location.SourceTree.GetRoot();
        var diagnosticLocationSyntaxNode = syntaxRoot.FindNode(diagnostic.Location.SourceSpan);

        if (isPoco2JsonTest)
        {
            var classNode = diagnosticLocationSyntaxNode as ClassDeclarationSyntax;
            return syntaxRoot.DescendantNodes().OfType<MethodDeclarationSyntax>().FirstOrDefault(method => method.Identifier.ValueText == classNode.Identifier.ValueText);
        }

        return diagnosticLocationSyntaxNode.Ancestors().OfType<MethodDeclarationSyntax>().FirstOrDefault();
    }
}
