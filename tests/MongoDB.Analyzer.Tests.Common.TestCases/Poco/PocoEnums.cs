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

using MongoDB.Analyzer.Tests.Common.DataModel;

namespace MongoDB.Analyzer.Tests.Common.TestCases.Poco
{
    public sealed class PocoEnums : TestCasesBase
    {
        [Json("{ \"EnumInt8\" : 0, \"EnumUInt8\" : 0, \"EnumInt16\" : 0, \"EnumUInt16\" : 0, \"EnumInt32\" : 0, \"EnumUInt32\" : 0, \"EnumInt64\" : NumberLong(0), \"EnumUInt64\" : NumberLong(0) }")]
        public void EnumHolder()
        {
        }

        public class TestClasses
        {
            public class EnumHolder
            {
                public EnumInt8 EnumInt8 { get; set; }
                public EnumUInt8 EnumUInt8 { get; set; }
                public EnumInt16 EnumInt16 { get; set; }
                public EnumUInt16 EnumUInt16 { get; set; }
                public EnumInt32 EnumInt32 { get; set; }
                public EnumUInt32 EnumUInt32 { get; set; }
                public EnumInt64 EnumInt64 { get; set; }
                public EnumUInt64 EnumUInt64 { get; set; }
            }
        }
    }
}

