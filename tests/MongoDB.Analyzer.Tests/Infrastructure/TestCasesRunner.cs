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
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MongoDB.Analyzer.Tests.Common;

namespace MongoDB.Analyzer.Tests.Infrastructure;

internal static class TestCasesRunner
{
    private record TestsBundleKey(string TestFileName, string DriverVersion, PocoAnalysisVerbosity PocoAnalysisVerbosity);

    private static readonly IDictionary<TestsBundleKey, IDictionary<string, DiagnosticTestCaseResult>>
        s_testResults = new Dictionary<TestsBundleKey, IDictionary<string, DiagnosticTestCaseResult>>();

    public static async Task<DiagnosticTestCaseResult> RunTestCase(DiagnosticTestCase diagnosticTestCase)
    {
        var testCollectionKey = new TestsBundleKey(diagnosticTestCase.FileName, diagnosticTestCase.Version, diagnosticTestCase.JsonAnalyzerVerbosity);

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
        var diagnostics = await DiagnosticsAnalyzer.Analyze(
            testsBundleKey.TestFileName,
            testsBundleKey.DriverVersion,
            testsBundleKey.PocoAnalysisVerbosity);

        var diagnosticsAndMethodNodes = diagnostics
            .Where(d => DiagnosticRulesConstants.AllRules.Contains(d.Descriptor.Id))
            .Select(d => (Diagnostic: d, MethodNode: FindMethodNode(d)))
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

    private static MethodDeclarationSyntax FindMethodNode(Diagnostic diagnostic)
    {
        var syntaxTreeRoot = diagnostic.Location.SourceTree.GetRoot();
        var diagnosticLocationSyntaxNode = syntaxTreeRoot.FindNode(diagnostic.Location.SourceSpan);

        if (diagnosticLocationSyntaxNode is ClassDeclarationSyntax poco)
        {
            return syntaxTreeRoot.DescendantNodes().OfType<MethodDeclarationSyntax>().FirstOrDefault(method => method.Identifier.ValueText == poco.Identifier.ValueText);
        }

        return diagnosticLocationSyntaxNode.Ancestors().OfType<MethodDeclarationSyntax>().FirstOrDefault();
    }
}
