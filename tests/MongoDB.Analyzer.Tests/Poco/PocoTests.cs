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
using MongoDB.Analyzer.Tests.Common.TestCases.Poco;
using MongoDB.Analyzer.Tests.Infrastructure;

namespace MongoDB.Analyzer.Tests.Poco;

[TestClass]
public sealed class PocoTests : DiagnosticsTestCasesRunner
{
    [DataTestMethod]
    [CodeBasedTestCasesSource(typeof(PocoArrays))]
    public Task Arrays(DiagnosticTestCase testCase) => VerifyTestCase(testCase);

    [DataTestMethod]
    [CodeBasedTestCasesSource(typeof(PocoBasic))]
    public Task Basic(DiagnosticTestCase testCase) => VerifyTestCase(testCase);

    [DataTestMethod]
    [CodeBasedTestCasesSource(typeof(PocoBsonAttributes))]
    public Task BsonAttributes(DiagnosticTestCase testCase) => VerifyTestCase(testCase);

    [DataTestMethod]
    [CodeBasedTestCasesSource(typeof(PocoBsonTypes))]
    public Task BsonTypes(DiagnosticTestCase testCase) => VerifyTestCase(testCase);

    [DataTestMethod]
    [CodeBasedTestCasesSource(typeof(PocoCollections))]
    public Task Collections(DiagnosticTestCase testCase) => VerifyTestCase(testCase);

    [DataTestMethod]
    [CodeBasedTestCasesSource(typeof(PocoEnums))]
    public Task Enums(DiagnosticTestCase testCase) => VerifyTestCase(testCase);

    [DataTestMethod]
    [CodeBasedTestCasesSource(typeof(PocoFields))]
    public Task Fields(DiagnosticTestCase testCase) => VerifyTestCase(testCase);

    [DataTestMethod]
    [CodeBasedTestCasesSource(typeof(PocoIgnoredBsonAttributes))]
    public Task IgnoredBsonAttributes(DiagnosticTestCase testCase) => VerifyTestCase(testCase);

    [DataTestMethod]
    [CodeBasedTestCasesSource(typeof(PocoNestedTypes))]
    public Task NestedTypes(DiagnosticTestCase testCase) => VerifyTestCase(testCase);

    [DataTestMethod]
    [CodeBasedTestCasesSource(typeof(PocoSystemTypes))]
    public Task SystemTypes(DiagnosticTestCase testCase) => VerifyTestCase(testCase);

    [DataTestMethod]
    [CodeBasedTestCasesSource(typeof(PocoVerbosity))]
    public Task Verbosity(DiagnosticTestCase testCase) => VerifyTestCase(testCase);
}