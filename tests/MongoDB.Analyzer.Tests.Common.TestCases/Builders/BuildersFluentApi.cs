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
using System.Threading.Tasks;
using MongoDB.Analyzer.Tests.Common.DataModel;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MongoDB.Analyzer.Tests.Common.TestCases.Builders
{
    public sealed class BuildersFluentApi : TestCasesBase
    {
        [BuildersMQL("find({ \"Age\" : { \"$gt\" : 1 } })")]
        [BuildersMQL("find({ \"Age\" : { \"$gt\" : 1 } })")]
        public async Task Async_suffix_methods()
        {
            GetMongoCollection().Find(u => u.Age > 1).ToListAsync().GetAwaiter().GetResult();
            await GetMongoCollection().Find(u => u.Age > 1).CountDocumentsAsync();
        }

        [BuildersMQL("find({ \"Age\" : 1 })")]
        [BuildersMQL("find({ \"Age\" : 1 })")]
        [BuildersMQL("find({ \"Age\" : 1 })")]
        [BuildersMQL("find({ \"Age\" : 1 })")]
        [BuildersMQL("find({ \"Age\" : 1 })")]
        public void Filter_as_count_first_single()
        {
            GetMongoCollection()
                .Find(u => u.Age == 1)
                .As<Person>();

            GetMongoCollection()
                .Find(u => u.Age == 1)
                .CountDocuments();

            GetMongoCollection()
                .Find(u => u.Age == 1)
                .First();

            GetMongoCollection()
                .Find(u => u.Age == 1)
                .Single();

            GetMongoCollection()
                .Find(u => u.Age == 1)
                .SingleOrDefault();
        }

        [BuildersMQL("find({ \"Age\" : 1 }).skip(10).limit(12)")]
        [BuildersMQL("find({ \"Age\" : 1 }).skip(10).limit(12)")]
        [BuildersMQL("find({ \"Age\" : 1 }).sort({ \"Name\" : 1, \"LastName\" : 1 }).skip(10).limit(10)")]
        public void Filter_skip_limit()
        {
            GetMongoCollection()
                .Find(u => u.Age == 1)
                .Skip(10)
                .Limit(12);

            GetMongoCollection()
                .Find(u => u.Age == 1)
                .Skip(10)
                .Limit(12)
                .As<Person>();

            GetMongoCollection()
                .Find(u => u.Age == 1)
                .Skip(10)
                .Limit(10)
                .SortBy(u => u.Name)
                .ThenBy(u => u.LastName);
        }

        [BuildersMQL("find({ \"Age\" : { \"$lt\" : 10 } }).sort({ \"Age\" : -1 })")]
        [BuildersMQL("find({ \"$or\" : [{ \"Age\" : { \"$lt\" : 10 } }, { \"Age\" : { \"$gt\" : 20 } }, { \"Name\" : { \"$ne\" : \"Bob\" }, \"LastName\" : { \"$exists\" : true } }] }).sort({ \"Age\" : -1, \"Name\" : 1, \"Address\" : 1 })")]
        public void Filter_sort()
        {
            GetMongoCollection()
                .Find(Builders<User>.Filter.Lt(u => u.Age, 10))
                .Sort(Builders<User>.Sort.Descending(u => u.Age));

            GetMongoCollection().Find(
                Builders<User>.Filter.Lt(u => u.Age, 10) |
                Builders<User>.Filter.Gt(u => u.Age, 20) |
                (Builders<User>.Filter.Ne(u => u.Name, "Bob") &
                 Builders<User>.Filter.Exists(u => u.LastName)))
                .Sort(Builders<User>.Sort.Combine(
                    Builders<User>.Sort.Descending(u => u.Age),
                    Builders<User>.Sort.Ascending(u => u.Name),
                    Builders<User>.Sort.Ascending(u => u.Address)));
        }

        [BuildersMQL("find({ \"Age\" : { \"$lt\" : 10 } }, { \"Age\" : 1 })")]
        [BuildersMQL("find({ \"Age\" : { \"$lt\" : 10 } }, { \"Age\" : 1 }).sort({ \"Age\" : -1 })")]
        [BuildersMQL("find({ \"Age\" : { \"$lt\" : 10 } }, { \"Age\" : 1, \"Height\" : 1, \"Address\" : 0 }).sort({ \"Age\" : -1 })")]
        public void Filter_sort_project()
        {
            GetMongoCollection()
                .Find(Builders<User>.Filter.Lt(u => u.Age, 10))
                .Project(Builders<User>.Projection.Include(u => u.Age));

            GetMongoCollection()
                .Find(Builders<User>.Filter.Lt(u => u.Age, 10))
                .Sort(Builders<User>.Sort.Descending(u => u.Age))
                .Project(Builders<User>.Projection.Include(u => u.Age));

            GetMongoCollection()
                .Find(Builders<User>.Filter.Lt(u => u.Age, 10))
                .Sort(Builders<User>.Sort.Descending(u => u.Age))
                .Project(Builders<User>.Projection
                    .Include(u => u.Age)
                    .Include(u => u.Height)
                    .Exclude(u => u.Address));
        }

        [NoDiagnostics]
        public void Filter_sort_project_to_new_object_should_be_ignored()
        {
            GetMongoCollection()
                .Find(Builders<User>.Filter.Lt(u => u.Age, 10))
                .Project(u => new User() { Address = u.Address });

            GetMongoCollection()
                .Find(Builders<User>.Filter.Lt(u => u.Age, 10))
                .Project(u => new User() { });

            GetMongoCollection()
                .Find(Builders<User>.Filter.Lt(u => u.Age, 10))
                .Project(u => new { Address = u.Address });

            GetMongoCollection()
                .Find(Builders<User>.Filter.Lt(u => u.Age, 10))
                .Project(u => new { });
        }

        [BuildersMQL("find({ \"Age\" : 1 }).sort({ \"Address\" : 1, \"Name\" : 1 })")]
        [BuildersMQL("find({ \"Age\" : 1 }).sort({ \"Address\" : -1, \"Name\" : -1, \"LastName\" : 1 })")]
        public void Filter_sort_thenby()
        {
            GetMongoCollection()
                .Find(u => u.Age == 1)
                .SortBy(u => u.Address)
                .ThenBy(u => u.Name);

            GetMongoCollection()
                .Find(u => u.Age == 1)
                .SortByDescending(u => u.Address)
                .ThenByDescending(u => u.Name)
                .ThenBy(u => u.LastName);
        }

        [BuildersMQL("find({ \"$or\" : [{ \"Age\" : { \"$lt\" : 10 } }, { \"Age\" : { \"$gt\" : 20 } }, { \"Name\" : { \"$ne\" : \"Bob\" }, \"LastName\" : { \"$ne\" : null } }] })")]
        [BuildersMQL("find({ \"$or\" : [{ \"Age\" : { \"$lt\" : 10 } }, { \"Age\" : { \"$gt\" : 20 } }, { \"Name\" : { \"$ne\" : \"Bob\" }, \"LastName\" : { \"$ne\" : null } }] }, { \"Age\" : 1, \"Address\" : 0, \"Height\" : 1 }).sort({ \"Age\" : -1, \"Name\" : 1, \"Address\" : 1 })")]
        public void Find_expression_based()
        {
            var mongoCollection = GetMongoCollection();
            mongoCollection.Find(u => u.Age < 10 || u.Age > 20 || (u.Name != "Bob" && u.LastName != null));

            GetMongoCollection().Find(u => u.Age < 10 || u.Age > 20 || (u.Name != "Bob" && u.LastName != null))
                .Sort(Builders<User>.Sort.Combine(
                    Builders<User>.Sort.Descending(u => u.Age),
                    Builders<User>.Sort.Ascending(u => u.Name),
                    Builders<User>.Sort.Ascending(u => u.Address)))
                .Project(Builders<User>.Projection.Include(u => u.Age)
                .Exclude(u => u.Address).Include(u => u.Height));
        }

        [BuildersMQL("find({ \"$or\" : [{ \"Age\" : { \"$lt\" : 10 } }, { \"Age\" : { \"$gt\" : 20 } }, { \"Name\" : { \"$ne\" : \"Bob\" }, \"LastName\" : { \"$ne\" : null } }] })")]
        [BuildersMQL("find({ \"$or\" : [{ \"Age\" : { \"$lt\" : 10 } }, { \"Age\" : { \"$gt\" : 20 } }, { \"Name\" : { \"$ne\" : \"Bob\" }, \"LastName\" : { \"$ne\" : null } }] })")]
        [BuildersMQL("find({ \"$or\" : [{ \"Age\" : { \"$lt\" : 10 } }, { \"Age\" : { \"$gt\" : 20 } }, { \"Name\" : { \"$ne\" : \"Bob\" }, \"LastName\" : { \"$ne\" : null } }] })")]
        [BuildersMQL("find({ \"$or\" : [{ \"Age\" : { \"$lt\" : 10 } }, { \"Age\" : { \"$gt\" : 20 } }, { \"Name\" : { \"$ne\" : \"Bob\" }, \"LastName\" : { \"$ne\" : null } }] })")]
        [BuildersMQL("find({ \"$or\" : [{ \"Age\" : { \"$lt\" : 10 } }, { \"Age\" : { \"$gt\" : 20 } }, { \"Name\" : { \"$ne\" : \"Bob\" }, \"LastName\" : { \"$ne\" : null } }] })")]
        [BuildersMQL("find({ \"$or\" : [{ \"Age\" : { \"$lt\" : 10 } }, { \"Age\" : { \"$gt\" : 20 } }, { \"Name\" : { \"$ne\" : \"Bob\" }, \"LastName\" : { \"$ne\" : null } }] })")]
        [BuildersMQL("find({ \"$or\" : [{ \"Age\" : { \"$lt\" : 10 } }, { \"Age\" : { \"$gt\" : 20 } }, { \"Name\" : { \"$ne\" : \"Bob\" }, \"LastName\" : { \"$ne\" : null } }] })")]
        [BuildersMQL("find({ \"$or\" : [{ \"Age\" : { \"$lt\" : 10 } }, { \"Age\" : { \"$gt\" : 20 } }, { \"Name\" : { \"$ne\" : \"Bob\" }, \"LastName\" : { \"$ne\" : null } }] })")]
        public void Find_with_additional_params_options()
        {
            GetMongoCollection().
                Find(u => u.Age < 10 || u.Age > 20 || (u.Name != "Bob" && u.LastName != null), new FindOptions());

            var findOptions = new FindOptions();
            GetMongoCollection().
                Find(u => u.Age < 10 || u.Age > 20 || (u.Name != "Bob" && u.LastName != null), findOptions);

            GetMongoCollection().
                Find(u => u.Age < 10 || u.Age > 20 || (u.Name != "Bob" && u.LastName != null), GetFindOptions());

            GetMongoCollection().
                Find(u => u.Age < 10 || u.Age > 20 || (u.Name != "Bob" && u.LastName != null), null);

            GetMongoCollection().
                Find(u => u.Age < 10 || u.Age > 20 || (u.Name != "Bob" && u.LastName != null), options: null);

            GetMongoCollection().
                Find(u => u.Age < 10 || u.Age > 20 || (u.Name != "Bob" && u.LastName != null), options: _findOptions);

            GetMongoCollection().
                Find(u => u.Age < 10 || u.Age > 20 || (u.Name != "Bob" && u.LastName != null), _findOptions);

            var mongoCollection = GetMongoCollection();
            mongoCollection.Find(
                u => u.Age < 10 || u.Age > 20 || (u.Name != "Bob" && u.LastName != null),
                new FindOptions() { MaxTime = TimeSpan.FromSeconds(3), Max = new BsonDocument("min", 2) });
        }

        [NoDiagnostics]
        public void Find_with_session_should_be_ignored()
        {
            IClientSessionHandle clientSessionHandle = null;

            GetMongoCollection().
                Find(clientSessionHandle, u => u.Age < 10, new FindOptions());

            GetMongoCollection().
                Find(GetClientSessionHandle(), u => u.Age < 10);

            GetMongoCollection().
                Find(GetClientSessionHandle(), u => u.Age < 10, new FindOptions());
        }

        [BuildersMQL("find({ \"Age\" : { \"$lt\" : 10 } }).sort({ \"Age\" : -1 })")]
        [BuildersMQL("find({ \"Age\" : { \"$lt\" : 10 } }).sort({ \"Age\" : -1 })")]
        [BuildersMQL("find({ \"Age\" : { \"$lt\" : 10 } }).sort({ \"Age\" : -1 })")]
        [BuildersMQL("find({ \"Age\" : { \"$lt\" : 10 } }).sort({ \"Age\" : -1 })")]
        [BuildersMQL("find({ \"Age\" : { \"$lt\" : 10 } }).sort({ \"Age\" : -1 })")]
        [BuildersMQL("find({ \"Age\" : { \"$lt\" : 10 } }).sort({ \"Age\" : -1 })")]
        public void MongoCollection_various_sources()
        {
            GetMongoCollection()
                .Find(Builders<User>.Filter.Lt(u => u.Age, 10))
                .Sort(Builders<User>.Sort.Descending(u => u.Age));

            MongoDBProvider.MongoDatabase.GetCollection<User>("TestCollection")
                .Find(Builders<User>.Filter.Lt(u => u.Age, 10))
                .Sort(Builders<User>.Sort.Descending(u => u.Age));

            GetDatabase().GetCollection<User>("TestCollection")
                .Find(Builders<User>.Filter.Lt(u => u.Age, 10))
                .Sort(Builders<User>.Sort.Descending(u => u.Age));

            new MongoClient(@"mongodb://localhost:27017").GetDatabase("TestDatabase").GetCollection<User>("TestCollection")
                .Find(Builders<User>.Filter.Lt(u => u.Age, 10))
                .Sort(Builders<User>.Sort.Descending(u => u.Age));

            var mongoCollection = GetMongoCollection();
            mongoCollection
                .Find(Builders<User>.Filter.Lt(u => u.Age, 10))
                .Sort(Builders<User>.Sort.Descending(u => u.Age));

            _mongoCollection
                .Find(Builders<User>.Filter.Lt(u => u.Age, 10))
                .Sort(Builders<User>.Sort.Descending(u => u.Age));
        }

        [BuildersMQL("{ \"Age\" : { \"$gt\" : 10 } }")]
        [BuildersMQL("{ \"Name\" : \"Bob\" }")]
        public void Variable_tracking_should_be_ignored()
        {
            var filter1 = Builders<User>.Filter.Gt(u => u.Age, 10);
            GetMongoCollection().Find(filter1);

            var filter2 = Builders<User>.Filter.Eq(u => u.Name, "Bob");
            GetMongoCollection().Find(filter1 | filter2);
        }

        private FindOptions GetFindOptions() => null;
        private FindOptions _findOptions = null;
        private IMongoCollection<User> _mongoCollection = null;
        private IClientSessionHandle GetClientSessionHandle() => null;
    }
}
