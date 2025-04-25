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
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Clusters;

namespace MongoDB.Analyzer.Helpers
{
    public sealed class MongoClientMock : IMongoClient
    {
        public ICluster Cluster => throw new NotImplementedException();

        public MongoClientSettings Settings => new MongoClientSettings();

        public ClientBulkWriteResult BulkWrite(IReadOnlyList<BulkWriteModel> models, ClientBulkWriteOptions options = null, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public ClientBulkWriteResult BulkWrite(IClientSessionHandle session, IReadOnlyList<BulkWriteModel> models, ClientBulkWriteOptions options = null, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task<ClientBulkWriteResult> BulkWriteAsync(IReadOnlyList<BulkWriteModel> models, ClientBulkWriteOptions options = null, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task<ClientBulkWriteResult> BulkWriteAsync(IClientSessionHandle session, IReadOnlyList<BulkWriteModel> models, ClientBulkWriteOptions options = null, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public void Dispose() => throw new NotImplementedException();
        public void DropDatabase(string name, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public void DropDatabase(IClientSessionHandle session, string name, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task DropDatabaseAsync(string name, CancellationToken cancellationToken = default) => default;
        public Task DropDatabaseAsync(IClientSessionHandle session, string name, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public IMongoDatabase GetDatabase(string name, MongoDatabaseSettings settings = null) => default;
        public IAsyncCursor<string> ListDatabaseNames(CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public IAsyncCursor<string> ListDatabaseNames(ListDatabaseNamesOptions options, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public IAsyncCursor<string> ListDatabaseNames(IClientSessionHandle session, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public IAsyncCursor<string> ListDatabaseNames(IClientSessionHandle session, ListDatabaseNamesOptions options, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task<IAsyncCursor<string>> ListDatabaseNamesAsync(CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task<IAsyncCursor<string>> ListDatabaseNamesAsync(ListDatabaseNamesOptions options, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task<IAsyncCursor<string>> ListDatabaseNamesAsync(IClientSessionHandle session, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task<IAsyncCursor<string>> ListDatabaseNamesAsync(IClientSessionHandle session, ListDatabaseNamesOptions options, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public IAsyncCursor<BsonDocument> ListDatabases(CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public IAsyncCursor<BsonDocument> ListDatabases(ListDatabasesOptions options, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public IAsyncCursor<BsonDocument> ListDatabases(IClientSessionHandle session, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public IAsyncCursor<BsonDocument> ListDatabases(IClientSessionHandle session, ListDatabasesOptions options, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task<IAsyncCursor<BsonDocument>> ListDatabasesAsync(CancellationToken cancellationToken = default) => default;
        public Task<IAsyncCursor<BsonDocument>> ListDatabasesAsync(ListDatabasesOptions options, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task<IAsyncCursor<BsonDocument>> ListDatabasesAsync(IClientSessionHandle session, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task<IAsyncCursor<BsonDocument>> ListDatabasesAsync(IClientSessionHandle session, ListDatabasesOptions options, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public IClientSessionHandle StartSession(ClientSessionOptions options = null, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task<IClientSessionHandle> StartSessionAsync(ClientSessionOptions options = null, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public IChangeStreamCursor<TResult> Watch<TResult>(PipelineDefinition<ChangeStreamDocument<BsonDocument>, TResult> pipeline, ChangeStreamOptions options = null, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public IChangeStreamCursor<TResult> Watch<TResult>(IClientSessionHandle session, PipelineDefinition<ChangeStreamDocument<BsonDocument>, TResult> pipeline, ChangeStreamOptions options = null, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task<IChangeStreamCursor<TResult>> WatchAsync<TResult>(PipelineDefinition<ChangeStreamDocument<BsonDocument>, TResult> pipeline, ChangeStreamOptions options = null, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task<IChangeStreamCursor<TResult>> WatchAsync<TResult>(IClientSessionHandle session, PipelineDefinition<ChangeStreamDocument<BsonDocument>, TResult> pipeline, ChangeStreamOptions options = null, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public IMongoClient WithReadConcern(ReadConcern readConcern) => throw new NotImplementedException();
        public IMongoClient WithReadPreference(ReadPreference readPreference) => throw new NotImplementedException();
        public IMongoClient WithWriteConcern(WriteConcern writeConcern) => throw new NotImplementedException();
    }
}

