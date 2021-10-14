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

using System.Linq;
using MongoDB.Analyzer.Tests.Common.DataModel;
using MongoDB.Driver.Linq;

namespace MongoDB.Analyzer.Tests.Common.TestCases.Linq
{
    public sealed class NotSupportedLinq2 : TestCasesBase
    {
        [NotSupportedLinq2("Supported in LINQ3 only: db.coll.Aggregate([{ \"$match\" : { \"Name\" : /^\\s*(?!\\s)123(?<!\\s)\\s*$/s } }])")]
        [MQLLinq3("db.coll.Aggregate([{ \"$match\" : { \"Name\" : /^\\s*(?!\\s)123(?<!\\s)\\s*$/s } }])")]
        public void String_methods_Trim()
        {
            _ = GetMongoQueryable()
                .Where(u => u.Name.Trim() == "123");
        }

        [NotSupportedLinq2("Supported in LINQ3 only: db.coll.Aggregate([{ \"$match\" : { \"$expr\" : { \"$eq\" : [{ \"$substrCP\" : [\"$Name\", 1, 2] }, \"abc\"] } } }])")]
        [MQLLinq3("db.coll.Aggregate([{ \"$match\" : { \"$expr\" : { \"$eq\" : [{ \"$substrCP\" : [\"$Name\", 1, 2] }, \"abc\"] } } }])")]
        public void String_methods_Substring()
        {
            _ = GetMongoQueryable()
                .Where(u => u.Name.Substring(1, 2) == "abc");
        }

        [NotSupportedLinq2("Supported in LINQ3 only: db.coll.Aggregate([{ \"$match\" : { \"$expr\" : { \"$eq\" : [\"$Name\", \"$LastName\"] } } }])")]
        [MQLLinq3("db.coll.Aggregate([{ \"$match\" : { \"$expr\" : { \"$eq\" : [\"$Name\", \"$LastName\"] } } }])")]
        public void Unsupported_cross_reference_1()
        {
            _ = GetMongoQueryable<Person>()
                .Where(u => u.Name == u.LastName);
        }

        [NotSupportedLinq2("Supported in LINQ3 only: db.coll.Aggregate([{ \"$match\" : { \"$expr\" : { \"$eq\" : [{ \"$arrayElemAt\" : [\"$IntArray\", 0] }, { \"$arrayElemAt\" : [\"$IntArray\", 1] }] } } }])")]
        [MQLLinq3("db.coll.Aggregate([{ \"$match\" : { \"$expr\" : { \"$eq\" : [{ \"$arrayElemAt\" : [\"$IntArray\", 0] }, { \"$arrayElemAt\" : [\"$IntArray\", 1] }] } } }])")]
        public void Unsupported_cross_reference_2()
        {
            _ = GetMongoQueryable<SimpleTypesArraysHolder>()
                .Where(u => u.IntArray[0] == u.IntArray[1]);
        }

        [NotSupportedLinq2("Supported in LINQ3 only: db.coll.Aggregate([{ \"$match\" : { \"$expr\" : { \"$eq\" : [{ \"$add\" : [\"$Age\", 1] }, 123] } } }, { \"$match\" : { \"$expr\" : { \"$eq\" : [{ \"$concat\" : [\"Dr \", \"$Name\"] }, \"Dr Bob\"] } } }])")]
        [MQLLinq3("db.coll.Aggregate([{ \"$match\" : { \"$expr\" : { \"$eq\" : [{ \"$add\" : [\"$Age\", 1] }, 123] } } }, { \"$match\" : { \"$expr\" : { \"$eq\" : [{ \"$concat\" : [\"Dr \", \"$Name\"] }, \"Dr Bob\"] } } }])")]
        public void Unsupported_property_transformation()
        {
            _ = GetMongoQueryable()
                .Where(u => u.Age + 1 == 123)
                .Where(u => "Dr " + u.Name == "Dr Bob");
        }
    }
}
