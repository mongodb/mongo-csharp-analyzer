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
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDB.Driver.Search;

namespace MongoDB.Analyzer.Helpers
{
    public sealed class MongoCollectionMock<TDocument> : IMongoCollection<TDocument>
    {
        public CollectionNamespace CollectionNamespace => new CollectionNamespace("db", "coll");

        public IMongoDatabase Database => new MongoDatabaseMock();

        public IBsonSerializer<TDocument> DocumentSerializer => BsonSerializer.LookupSerializer<TDocument>();

        public IMongoIndexManager<TDocument> Indexes => throw new NotImplementedException();

        public MongoCollectionSettings Settings => new MongoCollectionSettings();

        public IMongoSearchIndexManager SearchIndexes => throw new NotImplementedException();

        /// <inheritdoc />
        public IAsyncCursor<TResult> Aggregate<TResult>(PipelineDefinition<TDocument, TResult> pipeline, AggregateOptions options = null, CancellationToken cancellationToken = default)
        {
            return new EmptyCursor<TResult>();
        }

        /// <inheritdoc />
        public IAsyncCursor<TResult> Aggregate<TResult>(IClientSessionHandle session, PipelineDefinition<TDocument, TResult> pipeline, AggregateOptions options = null, CancellationToken cancellationToken = default)
        {
            return new EmptyCursor<TResult>();
        }

        /// <inheritdoc />
        public async Task<IAsyncCursor<TResult>> AggregateAsync<TResult>(PipelineDefinition<TDocument, TResult> pipeline, AggregateOptions options = null, CancellationToken cancellationToken = default)
        {
            await Task.FromResult(1);
            return new EmptyCursor<TResult>();
        }

        /// <inheritdoc />
        public async Task<IAsyncCursor<TResult>> AggregateAsync<TResult>(IClientSessionHandle session, PipelineDefinition<TDocument, TResult> pipeline, AggregateOptions options = null, CancellationToken cancellationToken = default)
        {
            await Task.FromResult(1);
            return new EmptyCursor<TResult>();
        }

        /// <inheritdoc />
        public void AggregateToCollection<TResult>(PipelineDefinition<TDocument, TResult> pipeline, AggregateOptions options = null, CancellationToken cancellationToken = default)
        {
        }

        /// <inheritdoc />
        public void AggregateToCollection<TResult>(IClientSessionHandle session, PipelineDefinition<TDocument, TResult> pipeline, AggregateOptions options = null, CancellationToken cancellationToken = default)
        {
        }

        /// <inheritdoc />
        public async Task AggregateToCollectionAsync<TResult>(PipelineDefinition<TDocument, TResult> pipeline, AggregateOptions options = null, CancellationToken cancellationToken = default)
        {
            await Task.FromResult(1);
        }

        /// <inheritdoc />
        public async Task AggregateToCollectionAsync<TResult>(IClientSessionHandle session, PipelineDefinition<TDocument, TResult> pipeline, AggregateOptions options = null, CancellationToken cancellationToken = default)
        {
            await Task.FromResult(1);
        }

        /// <inheritdoc />
        public BulkWriteResult<TDocument> BulkWrite(IEnumerable<WriteModel<TDocument>> requests, BulkWriteOptions options = null, CancellationToken cancellationToken = default)
        {
            return null;
        }

        /// <inheritdoc />
        public BulkWriteResult<TDocument> BulkWrite(IClientSessionHandle session, IEnumerable<WriteModel<TDocument>> requests, BulkWriteOptions options = null, CancellationToken cancellationToken = default)
        {
            return null;
        }

        /// <inheritdoc />
        public Task<BulkWriteResult<TDocument>> BulkWriteAsync(IEnumerable<WriteModel<TDocument>> requests, BulkWriteOptions options = null, CancellationToken cancellationToken = default)
        {
            return null;
        }

        /// <inheritdoc />
        public Task<BulkWriteResult<TDocument>> BulkWriteAsync(IClientSessionHandle session, IEnumerable<WriteModel<TDocument>> requests, BulkWriteOptions options = null, CancellationToken cancellationToken = default)
        {
            return null;
        }

        /// <inheritdoc />
        [Obsolete("Use CountDocuments or EstimatedDocumentCount instead.")]
        public long Count(FilterDefinition<TDocument> filter, CountOptions options = null, CancellationToken cancellationToken = default)
        {
            return 1;
        }

        /// <inheritdoc />
        [Obsolete("Use CountDocuments or EstimatedDocumentCount instead.")]
        public long Count(IClientSessionHandle session, FilterDefinition<TDocument> filter, CountOptions options = null, CancellationToken cancellationToken = default)
        {
            return 1;
        }

        /// <inheritdoc />
        [Obsolete("Use CountDocumentsAsync or EstimatedDocumentCountAsync instead.")]
        public async Task<long> CountAsync(FilterDefinition<TDocument> filter, CountOptions options = null, CancellationToken cancellationToken = default)
        {
            await Task.FromResult(1);
            return 1;
        }

        /// <inheritdoc />
        [Obsolete("Use CountDocumentsAsync or EstimatedDocumentCountAsync instead.")]
        public async Task<long> CountAsync(IClientSessionHandle session, FilterDefinition<TDocument> filter, CountOptions options = null, CancellationToken cancellationToken = default)
        {
            await Task.FromResult(1);
            return 1;
        }

        /// <inheritdoc />
        public long CountDocuments(FilterDefinition<TDocument> filter, CountOptions options = null, CancellationToken cancellationToken = default)
        {
            return 1;
        }

        /// <inheritdoc />
        public long CountDocuments(IClientSessionHandle session, FilterDefinition<TDocument> filter, CountOptions options = null, CancellationToken cancellationToken = default)
        {
            return 1;
        }

        /// <inheritdoc />
        public async Task<long> CountDocumentsAsync(FilterDefinition<TDocument> filter, CountOptions options = null, CancellationToken cancellationToken = default)
        {
            await Task.FromResult(1);
            return 1;
        }

        /// <inheritdoc />
        public async Task<long> CountDocumentsAsync(IClientSessionHandle session, FilterDefinition<TDocument> filter, CountOptions options = null, CancellationToken cancellationToken = default)
        {
            await Task.FromResult(1);
            return 1;
        }

        public DeleteResult DeleteMany(FilterDefinition<TDocument> filter, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public DeleteResult DeleteMany(FilterDefinition<TDocument> filter, DeleteOptions options, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public DeleteResult DeleteMany(IClientSessionHandle session, FilterDefinition<TDocument> filter, DeleteOptions options = null, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task<DeleteResult> DeleteManyAsync(FilterDefinition<TDocument> filter, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task<DeleteResult> DeleteManyAsync(FilterDefinition<TDocument> filter, DeleteOptions options, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task<DeleteResult> DeleteManyAsync(IClientSessionHandle session, FilterDefinition<TDocument> filter, DeleteOptions options = null, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public DeleteResult DeleteOne(FilterDefinition<TDocument> filter, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public DeleteResult DeleteOne(FilterDefinition<TDocument> filter, DeleteOptions options, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public DeleteResult DeleteOne(IClientSessionHandle session, FilterDefinition<TDocument> filter, DeleteOptions options = null, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task<DeleteResult> DeleteOneAsync(FilterDefinition<TDocument> filter, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task<DeleteResult> DeleteOneAsync(FilterDefinition<TDocument> filter, DeleteOptions options, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task<DeleteResult> DeleteOneAsync(IClientSessionHandle session, FilterDefinition<TDocument> filter, DeleteOptions options = null, CancellationToken cancellationToken = default) => throw new NotImplementedException();

        /// <inheritdoc />
        public IAsyncCursor<TField> Distinct<TField>(FieldDefinition<TDocument, TField> field, FilterDefinition<TDocument> filter, DistinctOptions options = null, CancellationToken cancellationToken = default)
        {
            return new EmptyCursor<TField>();
        }

        /// <inheritdoc />
        public IAsyncCursor<TField> Distinct<TField>(IClientSessionHandle session, FieldDefinition<TDocument, TField> field, FilterDefinition<TDocument> filter, DistinctOptions options = null, CancellationToken cancellationToken = default)
        {
            return new EmptyCursor<TField>();
        }

        /// <inheritdoc />
        public async Task<IAsyncCursor<TField>> DistinctAsync<TField>(FieldDefinition<TDocument, TField> field, FilterDefinition<TDocument> filter, DistinctOptions options = null, CancellationToken cancellationToken = default)
        {
            await Task.FromResult(1);
            return new EmptyCursor<TField>();
        }

        /// <inheritdoc />
        public async Task<IAsyncCursor<TField>> DistinctAsync<TField>(IClientSessionHandle session, FieldDefinition<TDocument, TField> field, FilterDefinition<TDocument> filter, DistinctOptions options = null, CancellationToken cancellationToken = default)
        {
            await Task.FromResult(1);
            return new EmptyCursor<TField>();
        }

        public IAsyncCursor<TItem> DistinctMany<TItem>(FieldDefinition<TDocument, IEnumerable<TItem>> field, FilterDefinition<TDocument> filter, DistinctOptions options = null, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public IAsyncCursor<TItem> DistinctMany<TItem>(IClientSessionHandle session, FieldDefinition<TDocument, IEnumerable<TItem>> field, FilterDefinition<TDocument> filter, DistinctOptions options = null, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task<IAsyncCursor<TItem>> DistinctManyAsync<TItem>(FieldDefinition<TDocument, IEnumerable<TItem>> field, FilterDefinition<TDocument> filter, DistinctOptions options = null, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task<IAsyncCursor<TItem>> DistinctManyAsync<TItem>(IClientSessionHandle session, FieldDefinition<TDocument, IEnumerable<TItem>> field, FilterDefinition<TDocument> filter, DistinctOptions options = null, CancellationToken cancellationToken = default) => throw new NotImplementedException();

        /// <inheritdoc />
        public long EstimatedDocumentCount(EstimatedDocumentCountOptions options = null, CancellationToken cancellationToken = default)
        {
            return 1;
        }

        /// <inheritdoc />
        public async Task<long> EstimatedDocumentCountAsync(EstimatedDocumentCountOptions options = null, CancellationToken cancellationToken = default)
        {
            await Task.FromResult(1);
            return 1;
        }

        /// <inheritdoc />
        public async Task<IAsyncCursor<TProjection>> FindAsync<TProjection>(FilterDefinition<TDocument> filter, FindOptions<TDocument, TProjection> options = null, CancellationToken cancellationToken = default)
        {
            await Task.FromResult(1);
            return new EmptyCursor<TProjection>();
        }

        /// <inheritdoc />
        public async Task<IAsyncCursor<TProjection>> FindAsync<TProjection>(IClientSessionHandle session, FilterDefinition<TDocument> filter, FindOptions<TDocument, TProjection> options = null, CancellationToken cancellationToken = default)
        {
            await Task.FromResult(1);
            return new EmptyCursor<TProjection>();
        }

        /// <inheritdoc />
        public TProjection FindOneAndDelete<TProjection>(FilterDefinition<TDocument> filter, FindOneAndDeleteOptions<TDocument, TProjection> options = null, CancellationToken cancellationToken = default)
        {
            return default;
        }

        /// <inheritdoc />
        public TProjection FindOneAndDelete<TProjection>(IClientSessionHandle session, FilterDefinition<TDocument> filter, FindOneAndDeleteOptions<TDocument, TProjection> options = null, CancellationToken cancellationToken = default)
        {
            return default;
        }

        /// <inheritdoc />
        public async Task<TProjection> FindOneAndDeleteAsync<TProjection>(FilterDefinition<TDocument> filter, FindOneAndDeleteOptions<TDocument, TProjection> options = null, CancellationToken cancellationToken = default)
        {
            await Task.FromResult(1);
            return default;
        }

        /// <inheritdoc />
        public async Task<TProjection> FindOneAndDeleteAsync<TProjection>(IClientSessionHandle session, FilterDefinition<TDocument> filter, FindOneAndDeleteOptions<TDocument, TProjection> options = null, CancellationToken cancellationToken = default)
        {
            await Task.FromResult(1);
            return default;
        }

        /// <inheritdoc />
        public TProjection FindOneAndReplace<TProjection>(FilterDefinition<TDocument> filter, TDocument replacement, FindOneAndReplaceOptions<TDocument, TProjection> options = null, CancellationToken cancellationToken = default)
        {
            return default;
        }

        /// <inheritdoc />
        public TProjection FindOneAndReplace<TProjection>(IClientSessionHandle session, FilterDefinition<TDocument> filter, TDocument replacement, FindOneAndReplaceOptions<TDocument, TProjection> options = null, CancellationToken cancellationToken = default)
        {
            return default;
        }

        /// <inheritdoc />
        public Task<TProjection> FindOneAndReplaceAsync<TProjection>(FilterDefinition<TDocument> filter, TDocument replacement, FindOneAndReplaceOptions<TDocument, TProjection> options = null, CancellationToken cancellationToken = default)
        {
            return default;
        }

        /// <inheritdoc />
        public async Task<TProjection> FindOneAndReplaceAsync<TProjection>(IClientSessionHandle session, FilterDefinition<TDocument> filter, TDocument replacement, FindOneAndReplaceOptions<TDocument, TProjection> options = null, CancellationToken cancellationToken = default)
        {
            await Task.FromResult(1);
            return default;
        }

        /// <inheritdoc />
        public TProjection FindOneAndUpdate<TProjection>(FilterDefinition<TDocument> filter, UpdateDefinition<TDocument> update, FindOneAndUpdateOptions<TDocument, TProjection> options = null, CancellationToken cancellationToken = default)
        {
            return default;
        }

        /// <inheritdoc />
        public TProjection FindOneAndUpdate<TProjection>(IClientSessionHandle session, FilterDefinition<TDocument> filter, UpdateDefinition<TDocument> update, FindOneAndUpdateOptions<TDocument, TProjection> options = null, CancellationToken cancellationToken = default)
        {
            return default;
        }

        /// <inheritdoc />
        public async Task<TProjection> FindOneAndUpdateAsync<TProjection>(FilterDefinition<TDocument> filter, UpdateDefinition<TDocument> update, FindOneAndUpdateOptions<TDocument, TProjection> options = null, CancellationToken cancellationToken = default)
        {
            await Task.FromResult(1);
            return default;
        }

        /// <inheritdoc />
        public async Task<TProjection> FindOneAndUpdateAsync<TProjection>(IClientSessionHandle session, FilterDefinition<TDocument> filter, UpdateDefinition<TDocument> update, FindOneAndUpdateOptions<TDocument, TProjection> options = null, CancellationToken cancellationToken = default)
        {
            await Task.FromResult(1);
            return default;
        }

        /// <inheritdoc />
        public IAsyncCursor<TProjection> FindSync<TProjection>(FilterDefinition<TDocument> filter, FindOptions<TDocument, TProjection> options = null, CancellationToken cancellationToken = default)
        {
            return new EmptyCursor<TProjection>();
        }

        /// <inheritdoc />
        public IAsyncCursor<TProjection> FindSync<TProjection>(IClientSessionHandle session, FilterDefinition<TDocument> filter, FindOptions<TDocument, TProjection> options = null, CancellationToken cancellationToken = default)
        {
            return new EmptyCursor<TProjection>();
        }

        public void InsertMany(IEnumerable<TDocument> documents, InsertManyOptions options = null, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public void InsertMany(IClientSessionHandle session, IEnumerable<TDocument> documents, InsertManyOptions options = null, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task InsertManyAsync(IEnumerable<TDocument> documents, InsertManyOptions options = null, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task InsertManyAsync(IClientSessionHandle session, IEnumerable<TDocument> documents, InsertManyOptions options = null, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public void InsertOne(TDocument document, InsertOneOptions options = null, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public void InsertOne(IClientSessionHandle session, TDocument document, InsertOneOptions options = null, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task InsertOneAsync(TDocument document, CancellationToken _cancellationToken) => throw new NotImplementedException();
        public Task InsertOneAsync(TDocument document, InsertOneOptions options = null, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task InsertOneAsync(IClientSessionHandle session, TDocument document, InsertOneOptions options = null, CancellationToken cancellationToken = default) => throw new NotImplementedException();

        /// <inheritdoc />
        [Obsolete]
        public IAsyncCursor<TResult> MapReduce<TResult>(BsonJavaScript map, BsonJavaScript reduce, MapReduceOptions<TDocument, TResult> options = null, CancellationToken cancellationToken = default)
        {
            return new EmptyCursor<TResult>();
        }

        /// <inheritdoc />
        [Obsolete]
        public IAsyncCursor<TResult> MapReduce<TResult>(IClientSessionHandle session, BsonJavaScript map, BsonJavaScript reduce, MapReduceOptions<TDocument, TResult> options = null, CancellationToken cancellationToken = default)
        {
            return new EmptyCursor<TResult>();
        }

        /// <inheritdoc />
        [Obsolete]
        public async Task<IAsyncCursor<TResult>> MapReduceAsync<TResult>(BsonJavaScript map, BsonJavaScript reduce, MapReduceOptions<TDocument, TResult> options = null, CancellationToken cancellationToken = default)
        {
            await Task.FromResult(1);
            return new EmptyCursor<TResult>();
        }

        /// <inheritdoc />
        [Obsolete]
        public async Task<IAsyncCursor<TResult>> MapReduceAsync<TResult>(IClientSessionHandle session, BsonJavaScript map, BsonJavaScript reduce, MapReduceOptions<TDocument, TResult> options = null, CancellationToken cancellationToken = default)
        {
            await Task.FromResult(1);
            return new EmptyCursor<TResult>();
        }

        public IFilteredMongoCollection<TDerivedDocument> OfType<TDerivedDocument>() where TDerivedDocument : TDocument => throw new NotImplementedException();
        public ReplaceOneResult ReplaceOne(FilterDefinition<TDocument> filter, TDocument replacement, ReplaceOptions options = null, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public ReplaceOneResult ReplaceOne(FilterDefinition<TDocument> filter, TDocument replacement, UpdateOptions options, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public ReplaceOneResult ReplaceOne(IClientSessionHandle session, FilterDefinition<TDocument> filter, TDocument replacement, ReplaceOptions options = null, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public ReplaceOneResult ReplaceOne(IClientSessionHandle session, FilterDefinition<TDocument> filter, TDocument replacement, UpdateOptions options, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task<ReplaceOneResult> ReplaceOneAsync(FilterDefinition<TDocument> filter, TDocument replacement, ReplaceOptions options = null, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task<ReplaceOneResult> ReplaceOneAsync(FilterDefinition<TDocument> filter, TDocument replacement, UpdateOptions options, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task<ReplaceOneResult> ReplaceOneAsync(IClientSessionHandle session, FilterDefinition<TDocument> filter, TDocument replacement, ReplaceOptions options = null, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task<ReplaceOneResult> ReplaceOneAsync(IClientSessionHandle session, FilterDefinition<TDocument> filter, TDocument replacement, UpdateOptions options, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public UpdateResult UpdateMany(FilterDefinition<TDocument> filter, UpdateDefinition<TDocument> update, UpdateOptions options = null, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public UpdateResult UpdateMany(IClientSessionHandle session, FilterDefinition<TDocument> filter, UpdateDefinition<TDocument> update, UpdateOptions options = null, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task<UpdateResult> UpdateManyAsync(FilterDefinition<TDocument> filter, UpdateDefinition<TDocument> update, UpdateOptions options = null, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task<UpdateResult> UpdateManyAsync(IClientSessionHandle session, FilterDefinition<TDocument> filter, UpdateDefinition<TDocument> update, UpdateOptions options = null, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public UpdateResult UpdateOne(FilterDefinition<TDocument> filter, UpdateDefinition<TDocument> update, UpdateOptions options = null, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public UpdateResult UpdateOne(IClientSessionHandle session, FilterDefinition<TDocument> filter, UpdateDefinition<TDocument> update, UpdateOptions options = null, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task<UpdateResult> UpdateOneAsync(FilterDefinition<TDocument> filter, UpdateDefinition<TDocument> update, UpdateOptions options = null, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task<UpdateResult> UpdateOneAsync(IClientSessionHandle session, FilterDefinition<TDocument> filter, UpdateDefinition<TDocument> update, UpdateOptions options = null, CancellationToken cancellationToken = default) => throw new NotImplementedException();

        /// <inheritdoc />
        public IChangeStreamCursor<TResult> Watch<TResult>(
            PipelineDefinition<ChangeStreamDocument<TDocument>, TResult> pipeline,
            ChangeStreamOptions options = null,
            CancellationToken cancellationToken = default)
        {
            return default;
        }

        /// <inheritdoc />
        public IChangeStreamCursor<TResult> Watch<TResult>(
            IClientSessionHandle session,
            PipelineDefinition<ChangeStreamDocument<TDocument>, TResult> pipeline,
            ChangeStreamOptions options = null,
            CancellationToken cancellationToken = default)
        {
            return default;
        }

        /// <inheritdoc />
        public Task<IChangeStreamCursor<TResult>> WatchAsync<TResult>(
            PipelineDefinition<ChangeStreamDocument<TDocument>, TResult> pipeline,
            ChangeStreamOptions options = null,
            CancellationToken cancellationToken = default)
        {
            return default;
        }

        /// <inheritdoc />
        public Task<IChangeStreamCursor<TResult>> WatchAsync<TResult>(
            IClientSessionHandle session,
            PipelineDefinition<ChangeStreamDocument<TDocument>, TResult> pipeline,
            ChangeStreamOptions options = null,
            CancellationToken cancellationToken = default)
        {
            return default;
        }

        /// <inheritdoc />
        public IMongoCollection<TDocument> WithReadConcern(ReadConcern readConcern)
        {
            return default;
        }

        /// <inheritdoc />
        public IMongoCollection<TDocument> WithReadPreference(ReadPreference readPreference)
        {
            return this;
        }

        /// <inheritdoc />
        public IMongoCollection<TDocument> WithWriteConcern(WriteConcern writeConcern)
        {
            return this;
        }
    }
}

