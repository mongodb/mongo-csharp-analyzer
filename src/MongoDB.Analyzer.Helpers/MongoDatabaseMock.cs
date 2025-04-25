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
    public sealed class MongoDatabaseMock : IMongoDatabase
    {
        public IMongoClient Client => new MongoClientMock();
        public DatabaseNamespace DatabaseNamespace => throw new NotImplementedException();
        public MongoDatabaseSettings Settings => throw new NotImplementedException();

        public IAsyncCursor<TResult> Aggregate<TResult>(PipelineDefinition<NoPipelineInput, TResult> pipeline, AggregateOptions options = null, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public IAsyncCursor<TResult> Aggregate<TResult>(IClientSessionHandle session, PipelineDefinition<NoPipelineInput, TResult> pipeline, AggregateOptions options = null, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task<IAsyncCursor<TResult>> AggregateAsync<TResult>(PipelineDefinition<NoPipelineInput, TResult> pipeline, AggregateOptions options = null, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task<IAsyncCursor<TResult>> AggregateAsync<TResult>(IClientSessionHandle session, PipelineDefinition<NoPipelineInput, TResult> pipeline, AggregateOptions options = null, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public void AggregateToCollection<TResult>(PipelineDefinition<NoPipelineInput, TResult> pipeline, AggregateOptions options = null, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public void AggregateToCollection<TResult>(IClientSessionHandle session, PipelineDefinition<NoPipelineInput, TResult> pipeline, AggregateOptions options = null, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task AggregateToCollectionAsync<TResult>(PipelineDefinition<NoPipelineInput, TResult> pipeline, AggregateOptions options = null, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task AggregateToCollectionAsync<TResult>(IClientSessionHandle session, PipelineDefinition<NoPipelineInput, TResult> pipeline, AggregateOptions options = null, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public void CreateCollection(string name, CreateCollectionOptions options = null, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public void CreateCollection(IClientSessionHandle session, string name, CreateCollectionOptions options = null, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task CreateCollectionAsync(string name, CreateCollectionOptions options = null, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task CreateCollectionAsync(IClientSessionHandle session, string name, CreateCollectionOptions options = null, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public void CreateView<TDocument, TResult>(string viewName, string viewOn, PipelineDefinition<TDocument, TResult> pipeline, CreateViewOptions<TDocument> options = null, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public void CreateView<TDocument, TResult>(IClientSessionHandle session, string viewName, string viewOn, PipelineDefinition<TDocument, TResult> pipeline, CreateViewOptions<TDocument> options = null, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task CreateViewAsync<TDocument, TResult>(string viewName, string viewOn, PipelineDefinition<TDocument, TResult> pipeline, CreateViewOptions<TDocument> options = null, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task CreateViewAsync<TDocument, TResult>(IClientSessionHandle session, string viewName, string viewOn, PipelineDefinition<TDocument, TResult> pipeline, CreateViewOptions<TDocument> options = null, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public void DropCollection(string name, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public void DropCollection(string name, DropCollectionOptions options, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public void DropCollection(IClientSessionHandle session, string name, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public void DropCollection(IClientSessionHandle session, string name, DropCollectionOptions options, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task DropCollectionAsync(string name, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task DropCollectionAsync(string name, DropCollectionOptions options, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task DropCollectionAsync(IClientSessionHandle session, string name, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task DropCollectionAsync(IClientSessionHandle session, string name, DropCollectionOptions options, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public IMongoCollection<TDocument> GetCollection<TDocument>(string name, MongoCollectionSettings settings = null) => new MongoCollectionMock<TDocument>();
        public IAsyncCursor<string> ListCollectionNames(ListCollectionNamesOptions options = null, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public IAsyncCursor<string> ListCollectionNames(IClientSessionHandle session, ListCollectionNamesOptions options = null, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task<IAsyncCursor<string>> ListCollectionNamesAsync(ListCollectionNamesOptions options = null, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task<IAsyncCursor<string>> ListCollectionNamesAsync(IClientSessionHandle session, ListCollectionNamesOptions options = null, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public IAsyncCursor<BsonDocument> ListCollections(ListCollectionsOptions options = null, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public IAsyncCursor<BsonDocument> ListCollections(IClientSessionHandle session, ListCollectionsOptions options = null, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task<IAsyncCursor<BsonDocument>> ListCollectionsAsync(ListCollectionsOptions options = null, CancellationToken cancellationToken = default) => default;
        public Task<IAsyncCursor<BsonDocument>> ListCollectionsAsync(IClientSessionHandle session, ListCollectionsOptions options = null, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public void RenameCollection(string oldName, string newName, RenameCollectionOptions options = null, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public void RenameCollection(IClientSessionHandle session, string oldName, string newName, RenameCollectionOptions options = null, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task RenameCollectionAsync(string oldName, string newName, RenameCollectionOptions options = null, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task RenameCollectionAsync(IClientSessionHandle session, string oldName, string newName, RenameCollectionOptions options = null, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public TResult RunCommand<TResult>(Command<TResult> command, ReadPreference readPreference = null, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public TResult RunCommand<TResult>(IClientSessionHandle session, Command<TResult> command, ReadPreference readPreference = null, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task<TResult> RunCommandAsync<TResult>(Command<TResult> command, ReadPreference readPreference = null, CancellationToken cancellationToken = default) => default;
        public Task<TResult> RunCommandAsync<TResult>(IClientSessionHandle session, Command<TResult> command, ReadPreference readPreference = null, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public IChangeStreamCursor<TResult> Watch<TResult>(PipelineDefinition<ChangeStreamDocument<BsonDocument>, TResult> pipeline, ChangeStreamOptions options = null, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public IChangeStreamCursor<TResult> Watch<TResult>(IClientSessionHandle session, PipelineDefinition<ChangeStreamDocument<BsonDocument>, TResult> pipeline, ChangeStreamOptions options = null, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task<IChangeStreamCursor<TResult>> WatchAsync<TResult>(PipelineDefinition<ChangeStreamDocument<BsonDocument>, TResult> pipeline, ChangeStreamOptions options = null, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task<IChangeStreamCursor<TResult>> WatchAsync<TResult>(IClientSessionHandle session, PipelineDefinition<ChangeStreamDocument<BsonDocument>, TResult> pipeline, ChangeStreamOptions options = null, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public IMongoDatabase WithReadConcern(ReadConcern readConcern) => throw new NotImplementedException();
        public IMongoDatabase WithReadPreference(ReadPreference readPreference) => throw new NotImplementedException();
        public IMongoDatabase WithWriteConcern(WriteConcern writeConcern) => throw new NotImplementedException();
    }
}
