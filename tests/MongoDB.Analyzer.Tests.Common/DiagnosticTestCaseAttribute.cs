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
        public int PocoLimit { get; }
        public PocoAnalysisVerbosity PocoAnalysisVerbosity { get; }

        public DiagnosticRuleTestCaseAttribute(
            string ruleId,
            string message,
            string version = null,
            LinqVersion linqProvider = LinqVersion.V2,
            DriverTargetFramework targetFramework = DriverTargetFramework.All,
            int pocoLimit = 500,
            PocoAnalysisVerbosity pocoAnalysisVerbosity = PocoAnalysisVerbosity.All,
            int[] codeLines = null)
        {
            RuleId = ruleId;
            Message = message;
            Version = version;
            LinqProvider = linqProvider;
            TargetFramework = targetFramework;
            Locations = codeLines?.Any() == true ? codeLines.Select(l => new Location(l, -1)).ToArray() : new[] { Location.Empty };
            PocoLimit = pocoLimit;
            PocoAnalysisVerbosity = pocoAnalysisVerbosity;
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class NoDiagnosticsAttribute : DiagnosticRuleTestCaseAttribute
    {
        public NoDiagnosticsAttribute(string version = null, int pocoLimit = 500, PocoAnalysisVerbosity pocoAnalysisVerbosity = PocoAnalysisVerbosity.All) : base(DiagnosticRulesConstants.NoRule, null, version, pocoLimit: pocoLimit, pocoAnalysisVerbosity: pocoAnalysisVerbosity) { }
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
                codeLines: codeLines)
        {
        }
    }

    public sealed class MQLLinq3Attribute : MQLAttribute
    {
        public MQLLinq3Attribute(
            string message,
            DriverTargetFramework targetFramework = DriverTargetFramework.All) :
            base(message, DriverVersions.Linq3OrGreater, LinqVersion.V3, targetFramework)
        {
        }
    }

    public sealed class MQLEFAttribute : DiagnosticRuleTestCaseAttribute
    {
        public MQLEFAttribute(
            string message,
            params int[] codeLines) :
            this(message, null, LinqVersion.V3, DriverTargetFramework.All, codeLines)
        {
        }

        public MQLEFAttribute(
            string message,
            string version = null,
            LinqVersion linqProvider = LinqVersion.V3,
            DriverTargetFramework targetFramework = DriverTargetFramework.All,
            params int[] codeLines) :
            base(DiagnosticRulesConstants.EF2MQL,
                message,
                version,
                linqProvider,
                targetFramework,
                codeLines: codeLines)
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
          string version = DriverVersions.Linq3OrGreater,
          DriverTargetFramework targetFramework = DriverTargetFramework.All) :
          base(message, version, LinqVersion.V3, targetFramework)
        {
        }
    }

    public sealed class NotSupportedLinq2Attribute : DiagnosticRuleTestCaseAttribute
    {
        public NotSupportedLinq2Attribute(
            string message,
            DriverTargetFramework targetFramework = DriverTargetFramework.All,
            string version = DriverVersions.Linq3OrGreater,
            LinqVersion linqVersion = LinqVersion.V2) :
            base(DiagnosticRulesConstants.NotSupportedLinq2Expression, message, version, linqVersion, targetFramework)
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

        public BuildersMQLAttribute(string message, int pocoLimit, PocoAnalysisVerbosity pocoAnalysisVerbosity) :
            base(DiagnosticRulesConstants.Builders2MQL,
                message,
                pocoLimit: pocoLimit,
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
            string message, int pocoLimit = 500, PocoAnalysisVerbosity pocoAnalysisVerbosity = PocoAnalysisVerbosity.All) :
            base(DiagnosticRulesConstants.Poco2Json, message, null, targetFramework: DriverTargetFramework.All, pocoLimit: pocoLimit, pocoAnalysisVerbosity: pocoAnalysisVerbosity, codeLines: null)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public sealed class NotSupportedPocoAttribute : DiagnosticRuleTestCaseAttribute
    {
        public NotSupportedPocoAttribute(string message, string version = null, int pocoLimit = 500, PocoAnalysisVerbosity pocoAnalysisVerbosity = PocoAnalysisVerbosity.All) :
            base(DiagnosticRulesConstants.NotSupportedPoco, message, version, pocoLimit: pocoLimit, pocoAnalysisVerbosity: pocoAnalysisVerbosity)
        {
        }
    }
}
