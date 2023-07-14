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
using MongoDB.Analyzer.Tests.Common.DataModel;
using MongoDB.Driver;

namespace MongoDB.Analyzer.Tests.Common.TestCases.Builders
{
    public sealed class BuildersBsonAttributes : TestCasesBase
    {
        [BuildersMQL("{ \"Cost\" : { \"$exists\" : true } }")]
        [BuildersMQL("{ \"_id\" : 0 }")]
        [BuildersMQL("{ \"Weight\" : 1 }")]
        [BuildersMQL("{ \"Color\" : \"Red\" }")]
        [BuildersMQL("{ \"$set\" : { \"Volume\" : 22.0 } }")]
        [BuildersMQL("{ \"$set\" : { \"Width\" : 5 } }")]
        [BuildersMQL("{ \"DictionaryField\" : { \"$exists\" : true } }")]
        [BuildersMQL("{ \"Name\" : 1 }")]
        [BuildersMQL("{ \"ExpiryDate\" : { \"$exists\" : true } }")]
        [BuildersMQL("{ \"TimeSpanField\" : { \"$exists\" : false } }")]
        [BuildersMQL("{ \"AppleType\" : { \"$exists\" : false } }")]
        [BuildersMQL("{ \"GreenAppleCost\" : { \"$exists\" : false } }")]
        [BuildersMQL("{ \"$or\" : [{ \"TimeSpanField\" : { \"$exists\" : true } }, { \"DateTimeOffset\" : { \"$exists\" : true } }, { \"Quantity\" : DateTimeOffset.Now.Day }] }")]
        public void Basic_bson_attributes()
        {
            _ = Builders<Fruit>.Filter.Exists(f => f.TotalCost, true);
            _ = Builders<Fruit>.Projection.Exclude(f => f.Name);

            _ = Builders<Fruit>.Sort.Ascending(f => f.Weight);
            _ = Builders<Fruit>.Filter.Eq(f => f.Color, "Red");

            _ = Builders<Fruit>.Update.Set(f => f.Volume, 22);
            _ = Builders<Fruit>.Update.Set(f => f.Width, 5);

            _ = Builders<Fruit>.Filter.Exists(f => f.DictionaryField, true);
            _ = Builders<Flower>.Projection.Include("Name");

            _ = Builders<Fruit>.Filter.Exists(f => f.ExpiryDate, true);
            _ = Builders<Fruit>.Filter.Exists(f => f.TimeSpanField, false);

            _ = Builders<Apple>.Filter.Exists(a => a.AppleType, false);
            _ = Builders<GreenApple>.Filter.Exists(g => g.GreenAppleCost, false);

            _ = Builders<Pear>.Filter.Exists(p => p.TimeSpanField, true) |
                Builders<Pear>.Filter.Exists(p => p.DateTimeOffset, true) |
                Builders<Pear>.Filter.Eq(p => p.Quantity, DateTimeOffset.Now.Day);
        }

        [BuildersMQL("{ \"RedAppleCost\" : { \"$exists\" : false } }")]
        public void Custom_bson_serializer()
        {
            _ = Builders<RedApple>.Filter.Exists(g => g.RedAppleCost, false);
        }

        [BuildersMQL("{ \"GrannyAppleCost\" : { \"$exists\" : false } }")]
        public void Unsupported_bson_attributes_should_be_ignored_and_MQL_should_still_be_rendered()
        {
            _ = Builders<GrannyApple>.Filter.Exists(g => g.GrannyAppleCost, false);
        }
    }
}

