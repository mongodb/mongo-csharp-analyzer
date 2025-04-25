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
using MongoDB.Driver;

namespace MongoDB.Analyzer.Tests.Common
{
    public abstract class TestCasesBase
    {
        protected IMongoDatabase GetDatabase() => MongoDBProvider.MongoDatabase;

        protected FilterDefinition<User> GetFilterUser() => Builders<User>.Filter.Lt(u => u.Age, 10);

        protected IMongoCollection<User> GetMongoCollection() =>
            GetMongoCollection<User>();

        protected IMongoCollection<T> GetMongoCollection<T>() =>
            MongoDBProvider.MongoDatabase.GetCollection<T>("TestCollection");

        protected IQueryable<User> GetMongoQueryable() => GetMongoCollection().AsQueryable();
        protected IQueryable<T> GetMongoQueryable<T>() => GetMongoCollection<T>().AsQueryable();

        protected T ReturnArgument<T>(T arg) => arg;
    }
}
