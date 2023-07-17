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
using System.Collections.Generic;
using MongoDB.Analyzer.Tests.Common.DataModel;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MongoDB.Analyzer.Tests.Common.TestCases.Json
{
    public sealed class NotSupportedJsonClassDeclarations : TestCasesBase
    {
        [NotSupportedJson("A serializer of type 'BooleanSerializer' is not configurable using an attribute of type 'BsonTimeSpanOptionsAttribute'.")]
        public void ClassTimeSpan()
        {
        }

        [NotSupportedJson("A serializer of type 'DecimalSerializer' is not configurable using an attribute of type 'BsonDateTimeOptionsAttribute'.")]
        public void ClassDateTime()
        {
        }

        public class TestClasses
        {
            public class ClassTimeSpan
            {
                public string Name { get; set; }

                [BsonTimeSpanOptions(representation: BsonType.Array, Units = Bson.Serialization.Options.TimeSpanUnits.Hours)]
                public bool InStock { get; set; }

                public decimal Price { get; set; }
                public Pair Pair { get; set; }
                public DateTime ExpiryDate;
                public int Length { get; set; }

                public int Width { get; set; }
                public TimeSpan SaleTime { get; set; }
                public Dictionary<string, string> DictionaryField;
            }

            public class ClassDateTime
            {
                public string Name { get; set; }
                public bool InStock { get; set; }

                [BsonDateTimeOptions(DateOnly = true, Kind = DateTimeKind.Local)]
                public decimal Price { get; set; }

                public Pair Pair { get; set; }
                public DateTime ExpiryDate;
                public int Length { get; set; }

                public int Width { get; set; }
                public TimeSpan SaleTime { get; set; }
                public Dictionary<string, string> DictionaryField;
            }
        }
    }
}

