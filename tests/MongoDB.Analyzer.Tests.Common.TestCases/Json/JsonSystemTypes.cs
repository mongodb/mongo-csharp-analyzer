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

namespace MongoDB.Analyzer.Tests.Common.TestCases.Json
{
    public sealed class JsonSystemTypes : TestCasesBase
    {
        [Json("{ \"DateTimeKindField\" : 0, \"DateTimeOffsetField\" : [NumberLong(0), 0], \"TimeSpanField\" : \"00:00:00\", \"TypeField\" : null }")]
        public void SystemTypeContainer()
        {
        }

        public class TestClasses
        {
            public class SystemTypeContainer
            {
                public DateTimeKind DateTimeKindField { get; set; }
                public DateTimeOffset DateTimeOffsetField { get; set; }
                public TimeSpan TimeSpanField { get; set; }
                public Type TypeField { get; set; }
            }
        }
    }
}

