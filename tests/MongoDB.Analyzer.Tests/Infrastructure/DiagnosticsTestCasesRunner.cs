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

using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Analyzer.Tests.Common;

namespace MongoDB.Analyzer.Tests.Infrastructure;

public abstract class DiagnosticsTestCasesRunner
{
    protected async Task VerifyTestCase(DiagnosticTestCase testCase)
    {
        var testCaseResult = await TestCasesRunner.RunTestCase(testCase);

        if (testCase.DiagnosticRules[0].RuleId == DiagnosticRulesConstants.NoRule)
        {
            Assert.IsNull(testCaseResult, "No diagnostics is expected but found {0} diagnostic messages.", testCaseResult?.Diagnostics?.Length);
        }
        else
        {
            Assert.AreEqual(testCase.DiagnosticRules.Length, testCaseResult?.Diagnostics.Length ?? 0);

            var actualDiagnostics = testCaseResult.Diagnostics
                .OrderBy(d => d.Location.GetLineSpan().StartLinePosition.Line)
                .ThenBy(d => d.GetMessage())
                .ToArray();

            for (int i = 0; i < testCase.DiagnosticRules.Length; i++)
            {
                AssertDiagnostic(testCase.DiagnosticRules[i], actualDiagnostics[i], testCaseResult.TestCaseMethodStartLine);
            }
        }
    }

    private void AssertDiagnostic(DiagnosticRule expectedDiagnosticRule, Diagnostic actualDiagnostic, int baseStartLine)
    {
        var ruleData = expectedDiagnosticRule.ToString();
        Assert.AreEqual(expectedDiagnosticRule.RuleId, actualDiagnostic.Id, actualDiagnostic.GetMessage(), ruleData);
        Assert.AreEqual(expectedDiagnosticRule.Message, actualDiagnostic.GetMessage(), ruleData);

        var location = expectedDiagnosticRule.Location;
        if (location?.StartLine >= 0)
        {
            Assert.AreEqual(location.StartLine, actualDiagnostic.Location.GetLineSpan().StartLinePosition.Line - baseStartLine, ruleData);
        }
        if (location?.EndLine >= 0)
        {
            Assert.AreEqual(location.EndLine, actualDiagnostic.Location.GetLineSpan().EndLinePosition.Line - baseStartLine, ruleData);
        }
    }
}