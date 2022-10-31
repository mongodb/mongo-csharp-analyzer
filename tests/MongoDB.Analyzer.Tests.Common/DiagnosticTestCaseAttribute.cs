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
using System.Linq;

namespace MongoDB.Analyzer.Tests.Common
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class DiagnosticRuleTestCaseAttribute : Attribute
    {
        public string RuleId { get; }
        public string Message { get; }
        public string Version { get; }
        public Location[] Locations { get; }
        public DriverTargetFramework TargetFramework { get; }
        public LinqVersion LinqProvider { get; }

        public DiagnosticRuleTestCaseAttribute(
            string ruleId,
            string message,
            string version = null,
            LinqVersion linqProvider = LinqVersion.V2,
            DriverTargetFramework targetFramework = DriverTargetFramework.All,
            Location[] locations = null)
        {
            RuleId = ruleId;
            Message = message;
            Version = version;
            LinqProvider = linqProvider;
            TargetFramework = targetFramework;
            Locations = locations ?? new[] { Location.Empty };
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class NoDiagnosticsAttribute : DiagnosticRuleTestCaseAttribute
    {
        public NoDiagnosticsAttribute() : base(DiagnosticRulesConstants.NoRule, null) { }
    }

    public class MQLAttribute : DiagnosticRuleTestCaseAttribute
    {
        public MQLAttribute(
            string message,
            params int[] codeLines) :
            this(message, null, LinqVersion.V2, DriverTargetFramework.All, codeLines)
        {
        }

        public MQLAttribute(
            string message,
            string version = null,
            LinqVersion linqProvider = LinqVersion.V2,
            DriverTargetFramework targetFramework = DriverTargetFramework.All,
            params int[] codeLines) :
            base(DiagnosticRulesConstants.MongoLinq2MQL,
                message,
                version,
                linqProvider,
                targetFramework,
                codeLines.Any() ? codeLines.Select(l => new Location(l, -1)).ToArray() : null)
        {
        }
    }

    public sealed class MQLLinq3Attribute : MQLAttribute
    {
        public MQLLinq3Attribute(
            string message,
            DriverTargetFramework targetFramework = DriverTargetFramework.All) :
            base(message, DriverVersions.Linq3AndHigher, LinqVersion.V3, targetFramework)
        {
        }
    }

    public class InvalidLinqAttribute : DiagnosticRuleTestCaseAttribute
    {
        public InvalidLinqAttribute(
            string message,
            string version = null,
            LinqVersion linqProvider = LinqVersion.V2,
            DriverTargetFramework targetFramework = DriverTargetFramework.All) :
            base(DiagnosticRulesConstants.NotSupportedLinqExpression, message, version, linqProvider, targetFramework)
        {
        }
    }

    public sealed class InvalidLinq3Attribute : InvalidLinqAttribute
    {
        public InvalidLinq3Attribute(
            string message,
            DriverTargetFramework targetFramework = DriverTargetFramework.All) :
            base(message, DriverVersions.Linq3AndHigher, LinqVersion.V3, targetFramework)
        {
        }
    }

    public sealed class NotSupportedLinq2Attribute : DiagnosticRuleTestCaseAttribute
    {
        public NotSupportedLinq2Attribute(
            string message,
            DriverTargetFramework targetFramework = DriverTargetFramework.All) :
            base(DiagnosticRulesConstants.NotSupportedLinq2Expression, message, DriverVersions.Linq3AndHigher, LinqVersion.V2, targetFramework)
        {
        }
    }

    public sealed class BuildersMQLAttribute : DiagnosticRuleTestCaseAttribute
    {
        public BuildersMQLAttribute(string message, params int[] codeLines) :
            base(DiagnosticRulesConstants.Builders2MQL,
                message,
                locations: codeLines.Any() ? codeLines.Select(l => new Location(l, -1)).ToArray() : null)
        {
        }
    }

    public sealed class NotSupportedBuildersAttribute : DiagnosticRuleTestCaseAttribute
    {
        public NotSupportedBuildersAttribute(string message) :
            base(DiagnosticRulesConstants.NotSupportedBuildersExpression, message)
        {
        }
    }
}
