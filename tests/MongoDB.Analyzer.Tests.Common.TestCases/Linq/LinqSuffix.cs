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
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace MongoDB.Analyzer.Tests.Common.TestCases.Linq
{
    public sealed class LinqSuffix : TestCasesBase
    {
        [MQL("aggregate([{ \"$match\" : { \"Name\" : \"Bob\", \"Age\" : { \"$gt\" : 16, \"$lte\" : 21 } } }, { \"$match\" : { \"LastName\" : /^.{0,9}$/s } }, { \"$project\" : { \"Name\" : \"$Name\", \"_id\" : 0 } }])")]
        public void SingleMethod_suffix()
        {
            var collection = GetMongoCollection();

            _ = collection.AsQueryable()
                .Where(u => u.Name == "Bob" && u.Age > 16 && u.Age <= 21)
                .Where(u => u.LastName.Length < 10)
                .Select(u => u.Name)
                .ToCursor();
        }

        [MQL("aggregate([{ \"$match\" : { \"Name\" : \"Bob\", \"Age\" : { \"$gt\" : 16, \"$lte\" : 21 } } }, { \"$match\" : { \"LastName\" : /^.{0,9}$/s } }, { \"$project\" : { \"Name\" : \"$Name\", \"_id\" : 0 } }])")]
        public void TwoMethods_suffix()
        {
            var collection = GetMongoCollection();

            _ = collection.AsQueryable()
                .Where(u => u.Name == "Bob" && u.Age > 16 && u.Age <= 21)
                .Where(u => u.LastName.Length < 10)
                .Select(u => u.Name)
                .ToCursor()
                .ToList();
        }

        [MQL("aggregate([{ \"$match\" : { \"Name\" : \"Bob\", \"Age\" : { \"$gt\" : 16, \"$lte\" : 21 } } }, { \"$match\" : { \"LastName\" : /^.{0,9}$/s } }, { \"$project\" : { \"Name\" : \"$Name\", \"_id\" : 0 } }])")]
        public void MultipleMethods_suffix()
        {
            var collection = GetMongoCollection();

            _ = collection.AsQueryable()
                .Where(u => u.Name == "Bob" && u.Age > 16 && u.Age <= 21)
                .Where(u => u.LastName.Length < 10)
                .Select(u => u.Name)
                .ToCursor()
                .ToList()
                .ToArray()
                .ToList();
        }

        [MQL("aggregate([{ \"$match\" : { \"Name\" : \"Bob\", \"Age\" : { \"$gt\" : 16, \"$lte\" : 21 } } }, { \"$match\" : { \"LastName\" : /^.{0,9}$/s } }])")]
        public void IMongoQueryable_extensions_suffix()
        {
            _ = GetMongoQueryable()
                .Where(u => u.Name == "Bob" && u.Age > 16 && u.Age <= 21)
                .Where(u => u.LastName.Length < 10)
                .ApplyPaging(1, 0);
        }

        [MQL("aggregate([{ \"$match\" : { \"LastName\" : \"Smith\" } }])")]
        public void IMongoQueryable_extensions_in_middle()
        {
            _ = GetMongoQueryable()
                .Where(u => u.Name == "Bob" && u.Age > 16 && u.Age <= 21)
                .Where(u => u.LastName.Length < 10)
                .ApplyPaging(1, 0)
                .Where(u => u.LastName == "Smith");
        }

        [MQL("aggregate([{ \"$match\" : { \"Name\" : \"Bob\", \"Age\" : { \"$gt\" : 16, \"$lte\" : 21 } } }, { \"$match\" : { \"LastName\" : /^.{0,9}$/s } }])")]
        public void IMongoQueryable_extensions_prefix()
        {
            _ = GetMongoQueryable()
                .ApplyPaging(1, 0)
                .Where(u => u.Name == "Bob" && u.Age > 16 && u.Age <= 21)
                .Where(u => u.LastName.Length < 10);
        }
    }

    public static class LinqExtensions
    {
        public static IMongoQueryable<T> ApplyPaging<T>(this IMongoQueryable<T> query, int page, int pageSize) =>
            query.Skip((page - 1) * pageSize).Take(pageSize);
    }
}
