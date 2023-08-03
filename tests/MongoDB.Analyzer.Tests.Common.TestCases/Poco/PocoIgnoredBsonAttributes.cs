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

namespace MongoDB.Analyzer.Tests.Common.TestCases.Poco
{
    public sealed class PocoIgnoredBsonAttributes : TestCasesBase
    {
        [PocoJson("{ \"ExpiryDate\" : ISODate(\"0001-01-01T00:00:00Z\"), \"DictionaryField\" : { }, \"Name\" : \"Name_val\", \"InStock\" : true, \"Price\" : \"0\", \"Pair\" : { \"StringA\" : null, \"StringB\" : null }, \"Length\" : 6, \"Width\" : 5, \"SaleTime\" : \"00:00:00\" }")]
        public void UnsupportedBsonAttributes()
        {
        }

        public class TestClasses
        {
            public class UnsupportedBsonAttributes
            {
                public string Name { get; set; }
                public bool InStock { get; set; }

                [BsonRepresentation(BsonType.Double)]
                public decimal Price { get; set; }

                public Pair Pair { get; set; }
                public DateTime ExpiryDate;
                public int Length { get; set; }
                public int Width { get; set; }
                public TimeSpan SaleTime { get; set; }

                [BsonDictionaryOptions(Bson.Serialization.Options.DictionaryRepresentation.Document)]
                public Dictionary<string, string> DictionaryField;
            }
        }
    }
}

