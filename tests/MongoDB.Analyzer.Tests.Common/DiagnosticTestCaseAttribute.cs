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
        public PocoAnalysisVerbosity PocoAnalysisVerbosity { get; }

        public DiagnosticRuleTestCaseAttribute(
            string ruleId,
            string message,
            string version = null,
            DriverTargetFramework targetFramework = DriverTargetFramework.All,
            PocoAnalysisVerbosity pocoAnalysisVerbosity = PocoAnalysisVerbosity.All,
            int[] codeLines = null)
        {
            RuleId = ruleId;
            Message = message;
            Version = version;
            TargetFramework = targetFramework;
            Locations = codeLines?.Any() == true ? codeLines.Select(l => new Location(l, -1)).ToArray() : new[] { Location.Empty };
            PocoAnalysisVerbosity = pocoAnalysisVerbosity;
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class NoDiagnosticsAttribute : DiagnosticRuleTestCaseAttribute
    {
        public NoDiagnosticsAttribute(string version = null, PocoAnalysisVerbosity pocoAnalysisVerbosity = PocoAnalysisVerbosity.All) : base(DiagnosticRulesConstants.NoRule, null, version, pocoAnalysisVerbosity: pocoAnalysisVerbosity) { }
    }

    public class MQLAttribute : DiagnosticRuleTestCaseAttribute
    {
        public MQLAttribute(
            string message,
            params int[] codeLines) :
            this(message, null, DriverTargetFramework.All, codeLines)
        {
        }

        public MQLAttribute(
            string message,
            string version = null,
            DriverTargetFramework targetFramework = DriverTargetFramework.All,
            params int[] codeLines) :
            base(DiagnosticRulesConstants.MongoLinq2MQL,
                message,
                version,
                targetFramework,
                codeLines: codeLines)
        {
        }
    }

    public sealed class MQLEFAttribute : DiagnosticRuleTestCaseAttribute
    {
        public MQLEFAttribute(
            string message,
            params int[] codeLines) :
            this(message, null, DriverTargetFramework.All, codeLines)
        {
        }

        public MQLEFAttribute(
            string message,
            string version = null,
            DriverTargetFramework targetFramework = DriverTargetFramework.All,
            params int[] codeLines) :
            base(DiagnosticRulesConstants.EF2MQL,
                message,
                version,
                targetFramework,
                codeLines: codeLines)
        {
        }
    }

    public sealed class NotSupportedEFAttribute : DiagnosticRuleTestCaseAttribute
    {
        public NotSupportedEFAttribute(string message, string version = null, DriverTargetFramework targetFramework = DriverTargetFramework.All, params int[] codeLines) :
            base(DiagnosticRulesConstants.NotSupportedEFExpression, message, version, targetFramework, codeLines: codeLines)
        {
        }
    }

    public class InvalidLinqAttribute : DiagnosticRuleTestCaseAttribute
    {
        public InvalidLinqAttribute(
            string message,
            string version = null,
            DriverTargetFramework targetFramework = DriverTargetFramework.All) :
            base(DiagnosticRulesConstants.NotSupportedLinqExpression, message, version, targetFramework)
        {
        }
    }

    public sealed class BuildersMQLAttribute : DiagnosticRuleTestCaseAttribute
    {
        public BuildersMQLAttribute(string message, params int[] codeLines) :
            base(DiagnosticRulesConstants.Builders2MQL,
                message,
                codeLines: codeLines)
        {
        }

        public BuildersMQLAttribute(string message, string version, params int[] codeLines) :
            base(DiagnosticRulesConstants.Builders2MQL,
                message,
                version,
                codeLines: codeLines)
        {
        }

        public BuildersMQLAttribute(string message, PocoAnalysisVerbosity pocoAnalysisVerbosity) :
            base(DiagnosticRulesConstants.Builders2MQL,
                message,
                pocoAnalysisVerbosity: pocoAnalysisVerbosity)
        {
        }
    }

    public sealed class NotSupportedBuildersAttribute : DiagnosticRuleTestCaseAttribute
    {
        public NotSupportedBuildersAttribute(string message, string version = null) :
            base(DiagnosticRulesConstants.NotSupportedBuildersExpression, message, version)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public sealed class PocoJsonAttribute : DiagnosticRuleTestCaseAttribute
    {
        public PocoJsonAttribute(
            string message, PocoAnalysisVerbosity pocoAnalysisVerbosity = PocoAnalysisVerbosity.All) :
            base(DiagnosticRulesConstants.Poco2Json, message, null, targetFramework: DriverTargetFramework.All, pocoAnalysisVerbosity: pocoAnalysisVerbosity, codeLines: null)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public sealed class NotSupportedPocoAttribute : DiagnosticRuleTestCaseAttribute
    {
        public NotSupportedPocoAttribute(string message, string version = null, PocoAnalysisVerbosity pocoAnalysisVerbosity = PocoAnalysisVerbosity.All) :
            base(DiagnosticRulesConstants.NotSupportedPoco, message, version, pocoAnalysisVerbosity: pocoAnalysisVerbosity)
        {
        }
    }
}
