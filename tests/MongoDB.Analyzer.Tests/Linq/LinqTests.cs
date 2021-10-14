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
using MongoDB.Analyzer.Tests.Common.TestCases.Linq;
using MongoDB.Analyzer.Tests.Infrastructure;

namespace MongoDB.Analyzer.Tests.Linq;

[TestClass]
public sealed class LinqTests : DiagnosticsTestCasesRunner
{
    [DataTestMethod]
    [CodeBasedTestCasesSource(typeof(LinqArrays))]
    public Task Arrays(DiagnosticTestCase testCase) => VerifyTestCase(testCase);

    [DataTestMethod]
    [CodeBasedTestCasesSource(typeof(LinqBasic))]
    public Task Basic(DiagnosticTestCase testCase) => VerifyTestCase(testCase);

    [DataTestMethod]
    [CodeBasedTestCasesSource(typeof(LinqCollections))]
    public Task Collections(DiagnosticTestCase testCase) => VerifyTestCase(testCase);

    [DataTestMethod]
    [CodeBasedTestCasesSource(typeof(LinqComments))]
    public Task Comments(DiagnosticTestCase testCase) => VerifyTestCase(testCase);

    [DataTestMethod]
    [CodeBasedTestCasesSource(typeof(LinqComplexDataModel))]
    public Task ComplexDataModel(DiagnosticTestCase testCase) => VerifyTestCase(testCase);

    [DataTestMethod]
    [CodeBasedTestCasesSource(typeof(LinqConstantsReplacement))]
    public Task ConstantsReplacement(DiagnosticTestCase testCase) => VerifyTestCase(testCase);

    [DataTestMethod]
    [CodeBasedTestCasesSource(typeof(LinqEnums))]
    public Task Enums(DiagnosticTestCase testCase) => VerifyTestCase(testCase);

    [DataTestMethod]
    [CodeBasedTestCasesSource(typeof(LinqIgnoredExpressions))]
    public Task IgnoredExpressions(DiagnosticTestCase testCase) => VerifyTestCase(testCase);

    [DataTestMethod]
    [CodeBasedTestCasesSource(typeof(LinqIMongoQueryableSource))]
    public Task MongoQueryableSource(DiagnosticTestCase testCase) => VerifyTestCase(testCase);

    [DataTestMethod]
    [CodeBasedTestCasesSource(typeof(LinqSuffix))]
    public Task Suffixes(DiagnosticTestCase testCase) => VerifyTestCase(testCase);
}
