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

namespace MongoDB.Analyzer.Tests.Common
{
    internal static class DiagnosticRulesConstants
    {
        public const string NotSupportedLinqExpression = "MALinq2001";
        public const string NotSupportedLinq2Expression = "MALinq2002";
        public const string MongoLinq2MQL = "MALinq1001";

        public const string Builders2MQL = "MABuilders1001";
        public const string NotSupportedBuildersExpression = "MABuilders2001";

        public const string NoRule = nameof(NoRule);

        public static string[] AllRules { get; } = new[]
        {
            NotSupportedLinqExpression,
            NotSupportedLinq2Expression,
            MongoLinq2MQL,
            Builders2MQL,
            NotSupportedBuildersExpression,
            NoRule
        };
    }
}
