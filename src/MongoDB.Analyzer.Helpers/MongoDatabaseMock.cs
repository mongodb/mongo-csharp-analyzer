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
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MongoDB.Analyzer.Helpers
{
    public sealed class MongoDatabaseMock : MongoDatabaseBase
    {
        public override IMongoClient Client => new MongoClientMock();
        public override DatabaseNamespace DatabaseNamespace => throw new NotImplementedException();
        public override MongoDatabaseSettings Settings => throw new NotImplementedException();

        public override Task CreateCollectionAsync(string name, CreateCollectionOptions options = null, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public override Task DropCollectionAsync(string name, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public override IMongoCollection<TDocument> GetCollection<TDocument>(string name, MongoCollectionSettings settings = null) => new MongoCollectionMock<TDocument>();
        public override Task<IAsyncCursor<BsonDocument>> ListCollectionsAsync(ListCollectionsOptions options = null, CancellationToken cancellationToken = default) => default;
        public override Task RenameCollectionAsync(string oldName, string newName, RenameCollectionOptions options = null, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public override Task<TResult> RunCommandAsync<TResult>(Command<TResult> command, ReadPreference readPreference = null, CancellationToken cancellationToken = default) => default;
    }
}
