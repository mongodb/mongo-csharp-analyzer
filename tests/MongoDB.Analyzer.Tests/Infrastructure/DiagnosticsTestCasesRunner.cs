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
            Assert.IsNull(testCaseResult, "No diagnostics is expected");
        }
        else
        {
            Assert.AreEqual(testCase.DiagnosticRules.Length, testCaseResult.Diagnostics.Length);

            for (int i = 0; i < testCase.DiagnosticRules.Length; i++)
            {
                AssertDiagnostic(testCase.DiagnosticRules[i], testCaseResult.Diagnostics[i]);
            }
        }
    }

    private void AssertDiagnostic(DiagnosticRule diagnosticRule, Diagnostic diagnostic)
    {
        Assert.AreEqual(diagnosticRule.RuleId, diagnostic.Id);
        Assert.AreEqual(diagnosticRule.Message, diagnostic.GetMessage());
    }
}
