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
using MongoDB.Analyzer.Tests.Common.TestCases.Json;
using MongoDB.Analyzer.Tests.Infrastructure;

namespace MongoDB.Analyzer.Tests.Json;

[TestClass]
public sealed class JsonTests : DiagnosticsTestCasesRunner
{
    //[DataTestMethod]
    //[ClassBasedTestCasesSourceAttribute(typeof(JsonArrays))]
    //public Task Arrays(DiagnosticTestCase testCase) => VerifyTestCase(testCase);

    //[DataTestMethod]
    //[ClassBasedTestCasesSourceAttribute(typeof(JsonBasic))]
    //public Task Basic(DiagnosticTestCase testCase) => VerifyTestCase(testCase);

    //[DataTestMethod]
    //[ClassBasedTestCasesSource(typeof(JsonBsonAttributes))]
    //public Task BsonAttributes(DiagnosticTestCase testCase) => VerifyTestCase(testCase);

    //[DataTestMethod]
    //[ClassBasedTestCasesSourceAttribute(typeof(JsonCollections))]
    //public Task Collections(DiagnosticTestCase testCase) => VerifyTestCase(testCase);

    //[DataTestMethod]
    //[ClassBasedTestCasesSourceAttribute(typeof(JsonEnums))]
    //public Task Enums(DiagnosticTestCase testCase) => VerifyTestCase(testCase);

    //[DataTestMethod]
    //[ClassBasedTestCasesSourceAttribute(typeof(JsonIgnoredBsonAttributes))]
    //public Task IgnoredBsonAttributes(DiagnosticTestCase testCase) => VerifyTestCase(testCase);

    //[DataTestMethod]
    //[ClassBasedTestCasesSourceAttribute(typeof(JsonNestedTypes))]
    //public Task NestedTypes(DiagnosticTestCase testCase) => VerifyTestCase(testCase);

    //[DataTestMethod]
    //[ClassBasedTestCasesSourceAttribute(typeof(JsonSystemTypes))]
    //public Task SystemTypes(DiagnosticTestCase testCase) => VerifyTestCase(testCase);

    [DataTestMethod]
    [ClassBasedTestCasesSource(typeof(JsonEmpiricalTest))]
    public Task EmpiricalTest(DiagnosticTestCase testCase) => VerifyTestCase(testCase);
}