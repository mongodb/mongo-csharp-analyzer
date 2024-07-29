﻿// Copyright 2021-present MongoDB Inc.
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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Analyzer.Tests.Common.TestCases.EF;
using MongoDB.Analyzer.Tests.Infrastructure;
using System.Threading.Tasks;

namespace MongoDB.Analyzer.Tests.EF;

[TestClass]
public sealed class EFNotSupportedExpressionsTests : DiagnosticsTestCasesRunner
{
    [DataTestMethod]
    [CodeBasedTestCasesSource(typeof(NotSupportedEFExpressions))]
    public Task NotSupportedExpressions(DiagnosticTestCase testCase) => VerifyTestCase(testCase);
}
