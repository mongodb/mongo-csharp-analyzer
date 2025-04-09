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
using MongoDB.Analyzer.Tests.Common.DataModel;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace MongoDB.Analyzer.Tests.Common.TestCases.Linq
{
    public sealed class LinqBsonAttributes : TestCasesBase
    {
        [MQL("Aggregate([{ \"$match\" : { \"Cost\" : 22.5 } }])")]
        [MQL("Aggregate([{ \"$project\" : { \"Name\" : \"$_id\", \"_id\" : 0 } }])")]
        [MQL("Aggregate([{ \"$sort\" : { \"Weight\" : 1 } }])")]
        [MQL("Aggregate([{ \"$match\" : { \"Color\" : \"Red\" } }])")]
        [MQL("Aggregate([{ \"$group\" : { \"_id\" : \"$ExpiryDate\", \"_elements\" : { \"$push\" : \"$$ROOT\" } } }])")]
        [MQL("Aggregate([{ \"$match\" : { \"Price\" : { \"$lt\" : 20.0 } } }, { \"$skip\" : 2 }])")]
        [MQL("Aggregate([{ \"$match\" : { \"GreenAppleCost\" : { \"$lt\" : 35.0 } } }, { \"$project\" : { \"Name\" : \"$_id\", \"TotalCost\" : \"$Cost\", \"_id\" : 0 } }, { \"$skip\" : 5 }])")]
        [MQL("Aggregate([{ \"$match\" : { \"AppleType\" : \"Green Apple\" } }, { \"$project\" : { \"Name\" : \"$_id\", \"TotalCost\" : \"$Cost\", \"_id\" : 0 } }, { \"$limit\" : 5 }])")]
        [MQL("Aggregate([{ \"$match\" : { \"Quantity\" : DateTimeOffset.Now.Day } }, { \"$project\" : { \"TimeSpanField\" : \"$TimeSpanField\", \"DateTimeOffset\" : \"$DateTimeOffset\", \"_id\" : 0 } }])")]
        public void Basic_bson_attributes()
        {
            _ = GetMongoCollection<Fruit>().AsQueryable().Where(f => f.TotalCost == 22.5);
            _ = GetMongoCollection<Fruit>().AsQueryable().Select(f => new { f.Name });

            _ = GetMongoCollection<Fruit>().AsQueryable().OrderBy(f => f.Weight);
            _ = GetMongoCollection<Fruit>().AsQueryable().Where(f => f.Color == "Red");

            _ = GetMongoCollection<Fruit>().AsQueryable().GroupBy(f => f.ExpiryDate);
            _ = GetMongoCollection<Flower>().AsQueryable().Where(f => f.Price < 20).Skip(2);

            _ = GetMongoCollection<GreenApple>().AsQueryable()
                .Where(g => g.GreenAppleCost < 35)
                .Select(g => new { g.Name, g.TotalCost })
                .Skip(5);

            _ = GetMongoCollection<Apple>().AsQueryable()
                .Where(a => a.AppleType == "Green Apple")
                .Select(a => new { a.Name, a.TotalCost })
                .Take(5);

            _ = GetMongoCollection<Pear>().AsQueryable()
                .Where(p => p.Quantity == DateTimeOffset.Now.Day)
                .Select(p => new { p.TimeSpanField, p.DateTimeOffset });
        }

        [MQL("Aggregate([{ \"$match\" : { \"Cost\" : 22.5 } }])")]
        public void Custom_bson_serializer()
        {
            _ = GetMongoCollection<RedApple>().AsQueryable().Where(r => r.TotalCost == 22.5);
        }

        [MQL("Aggregate([{ \"$match\" : { \"GrannyAppleCost\" : 22.0 } }])")]
        public void Unsupported_bson_attributes_should_be_ignored_and_MQL_should_still_be_rendered()
        {
            _ = GetMongoCollection<GrannyApple>().AsQueryable()
                .Where(g => g.GrannyAppleCost == 22)
                .Select(g => g);
        }
    }
}
