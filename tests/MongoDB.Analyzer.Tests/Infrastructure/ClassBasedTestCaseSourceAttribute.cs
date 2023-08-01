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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Analyzer.Core;
using MongoDB.Analyzer.Tests.Common;
using NuGet.Versioning;

namespace MongoDB.Analyzer.Tests.Infrastructure;

public sealed class ClassBasedTestCasesSourceAttribute : Attribute, ITestDataSource
{
    private readonly Type _testCasesProviderType;

    public ClassBasedTestCasesSourceAttribute(Type testCasesProviderType)
    {
        _testCasesProviderType = testCasesProviderType;
    }

    public IEnumerable<object[]> GetData(MethodInfo methodInfo)
    {
        var testCasesMethods = _testCasesProviderType
            .GetMembers(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
            .Where(m => m.MemberType == MemberTypes.Method &&
                m.DeclaringType == _testCasesProviderType &&
                m.GetCustomAttributes().Any(a => a is JsonDiagnosticTestCaseAttribute));

        var fileName = PathUtilities.GetTestCaseFileFullPathFromName(_testCasesProviderType.FullName);

        var data = testCasesMethods.SelectMany(m =>
            CreateTestCases(m, fileName).Select(t => new object[] { t }).ToArray()).ToArray();

        return data;
    }

    public string GetDisplayName(MethodInfo methodInfo, object[] data)
    {
        var testCase = (DiagnosticTestCase)data[0];
        return $"v{testCase.Version}__{testCase.MethodName}";
    }

    private DiagnosticTestCase[] CreateTestCases(MemberInfo memberInfo, string fileName)
    {
        var testCasesAttributes = memberInfo
            .GetCustomAttributes()
            .OfType<JsonDiagnosticTestCaseAttribute>();

        var diagnosticsTestCases =
            from attribute in testCasesAttributes
            where EnvironmentUtilities.IsDriverTargetFrameworkSupported((Analyzer.Core.DriverTargetFramework)(int)attribute.TargetFramework)
            from version in DriverVersionHelper.FilterVersionForRange(attribute.Version)
            from location in attribute.Locations
            orderby
                location.StartLine >= 0 ? location.StartLine : 0,
                location.StartLine >= 0 ? attribute.Message : null
            group new DiagnosticRule(attribute.RuleId, $"{attribute.Message}_v{version.ToString("V", new VersionFormatter())}", location)
                by new { version, attribute.JsonAnalyzerVerbosity } into g
            select new DiagnosticTestCase(fileName, memberInfo.Name, g.Key.version.ToString(), Common.LinqVersion.Undefined, g.Key.JsonAnalyzerVerbosity, g.ToArray());

        return diagnosticsTestCases.ToArray();
    }
}