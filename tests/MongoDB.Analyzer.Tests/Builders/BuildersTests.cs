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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Analyzer.Tests.Common.TestCases.Builders;
using MongoDB.Analyzer.Tests.Infrastructure;

namespace MongoDB.Analyzer.Tests.Builders;

[TestClass]
public sealed class BuildersTests : DiagnosticsTestCasesRunner
{
    [DataTestMethod]
    [CodeBasedTestCasesSource(typeof(BuildersArrays))]
    public Task Arrays(DiagnosticTestCase testCase) => VerifyTestCase(testCase);

    [DataTestMethod]
    [CodeBasedTestCasesSource(typeof(BuildersBasic))]
    public Task Basic(DiagnosticTestCase testCase) => VerifyTestCase(testCase);

    [DataTestMethod]
    [CodeBasedTestCasesSource(typeof(BuildersComplexDataModel))]
    public Task ComplexDataModel(DiagnosticTestCase testCase) => VerifyTestCase(testCase);

    [DataTestMethod]
    [CodeBasedTestCasesSource(typeof(BuildersConstantsReplacement))]
    public Task ConstantsReplacement(DiagnosticTestCase testCase) => VerifyTestCase(testCase);

    [DataTestMethod]
    [CodeBasedTestCasesSource(typeof(BuildersFluentApi))]
    public Task FluentApi(DiagnosticTestCase testCase) => VerifyTestCase(testCase);

    [DataTestMethod]
    [CodeBasedTestCasesSource(typeof(BuildersIgnoredExpressions))]
    public Task IgnoredExpressions(DiagnosticTestCase testCase) => VerifyTestCase(testCase);

    [DataTestMethod]
    [CodeBasedTestCasesSource(typeof(BuildersCollections))]
    public Task Collections(DiagnosticTestCase testCase) => VerifyTestCase(testCase);
}
