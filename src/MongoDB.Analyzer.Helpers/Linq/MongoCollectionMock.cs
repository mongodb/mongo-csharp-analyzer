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

namespace MongoDB.Analyzer.Helpers.Linq
{
    public sealed class MongoCollectionMock<TDocument> : MongoCollectionBase<TDocument>
    {
        public override CollectionNamespace CollectionNamespace => new CollectionNamespace("db", "coll");

        public override IMongoDatabase Database => throw new NotImplementedException();

        public override IBsonSerializer<TDocument> DocumentSerializer => BsonSerializer.LookupSerializer<TDocument>();

        public override IMongoIndexManager<TDocument> Indexes => throw new NotImplementedException();

        public override MongoCollectionSettings Settings => new MongoCollectionSettings();

        /// <inheritdoc />
        public override IAsyncCursor<TResult> Aggregate<TResult>(PipelineDefinition<TDocument, TResult> pipeline, AggregateOptions options = null, CancellationToken cancellationToken = default)
        {
            return new EmptyCursor<TResult>();
        }

        /// <inheritdoc />
        public override IAsyncCursor<TResult> Aggregate<TResult>(IClientSessionHandle session, PipelineDefinition<TDocument, TResult> pipeline, AggregateOptions options = null, CancellationToken cancellationToken = default)
        {
            return new EmptyCursor<TResult>();
        }

        /// <inheritdoc />
        public override async Task<IAsyncCursor<TResult>> AggregateAsync<TResult>(PipelineDefinition<TDocument, TResult> pipeline, AggregateOptions options = null, CancellationToken cancellationToken = default)
        {
            await Task.FromResult(1);
            return new EmptyCursor<TResult>();
        }

        /// <inheritdoc />
        public override async Task<IAsyncCursor<TResult>> AggregateAsync<TResult>(IClientSessionHandle session, PipelineDefinition<TDocument, TResult> pipeline, AggregateOptions options = null, CancellationToken cancellationToken = default)
        {
            await Task.FromResult(1);
            return new EmptyCursor<TResult>();
        }

        /// <inheritdoc />
        public override void AggregateToCollection<TResult>(PipelineDefinition<TDocument, TResult> pipeline, AggregateOptions options = null, CancellationToken cancellationToken = default)
        {
        }

        /// <inheritdoc />
        public override void AggregateToCollection<TResult>(IClientSessionHandle session, PipelineDefinition<TDocument, TResult> pipeline, AggregateOptions options = null, CancellationToken cancellationToken = default)
        {
        }

        /// <inheritdoc />
        public override async Task AggregateToCollectionAsync<TResult>(PipelineDefinition<TDocument, TResult> pipeline, AggregateOptions options = null, CancellationToken cancellationToken = default)
        {
            await Task.FromResult(1);
        }

        /// <inheritdoc />
        public override async Task AggregateToCollectionAsync<TResult>(IClientSessionHandle session, PipelineDefinition<TDocument, TResult> pipeline, AggregateOptions options = null, CancellationToken cancellationToken = default)
        {
            await Task.FromResult(1);
        }

        /// <inheritdoc />
        public override BulkWriteResult<TDocument> BulkWrite(IEnumerable<WriteModel<TDocument>> requests, BulkWriteOptions options = null, CancellationToken cancellationToken = default)
        {
            return null;
        }

        /// <inheritdoc />
        public override BulkWriteResult<TDocument> BulkWrite(IClientSessionHandle session, IEnumerable<WriteModel<TDocument>> requests, BulkWriteOptions options = null, CancellationToken cancellationToken = default)
        {
            return null;
        }

        /// <inheritdoc />
        public override Task<BulkWriteResult<TDocument>> BulkWriteAsync(IEnumerable<WriteModel<TDocument>> requests, BulkWriteOptions options = null, CancellationToken cancellationToken = default)
        {
            return null;
        }

        /// <inheritdoc />
        public override Task<BulkWriteResult<TDocument>> BulkWriteAsync(IClientSessionHandle session, IEnumerable<WriteModel<TDocument>> requests, BulkWriteOptions options = null, CancellationToken cancellationToken = default)
        {
            return null;
        }

        /// <inheritdoc />
        [Obsolete("Use CountDocuments or EstimatedDocumentCount instead.")]
        public override long Count(FilterDefinition<TDocument> filter, CountOptions options = null, CancellationToken cancellationToken = default)
        {
            return 1;
        }

        /// <inheritdoc />
        [Obsolete("Use CountDocuments or EstimatedDocumentCount instead.")]
        public override long Count(IClientSessionHandle session, FilterDefinition<TDocument> filter, CountOptions options = null, CancellationToken cancellationToken = default)
        {
            return 1;
        }

        /// <inheritdoc />
        [Obsolete("Use CountDocumentsAsync or EstimatedDocumentCountAsync instead.")]
        public override async Task<long> CountAsync(FilterDefinition<TDocument> filter, CountOptions options = null, CancellationToken cancellationToken = default)
        {
            await Task.FromResult(1);
            return 1;
        }

        /// <inheritdoc />
        [Obsolete("Use CountDocumentsAsync or EstimatedDocumentCountAsync instead.")]
        public override async Task<long> CountAsync(IClientSessionHandle session, FilterDefinition<TDocument> filter, CountOptions options = null, CancellationToken cancellationToken = default)
        {
            await Task.FromResult(1);
            return 1;
        }

        /// <inheritdoc />
        public override long CountDocuments(FilterDefinition<TDocument> filter, CountOptions options = null, CancellationToken cancellationToken = default)
        {
            return 1;
        }

        /// <inheritdoc />
        public override long CountDocuments(IClientSessionHandle session, FilterDefinition<TDocument> filter, CountOptions options = null, CancellationToken cancellationToken = default)
        {
            return 1;
        }

        /// <inheritdoc />
        public override async Task<long> CountDocumentsAsync(FilterDefinition<TDocument> filter, CountOptions options = null, CancellationToken cancellationToken = default)
        {
            await Task.FromResult(1);
            return 1;
        }

        /// <inheritdoc />
        public override async Task<long> CountDocumentsAsync(IClientSessionHandle session, FilterDefinition<TDocument> filter, CountOptions options = null, CancellationToken cancellationToken = default)
        {
            await Task.FromResult(1);
            return 1;
        }

        /// <inheritdoc />
        public override IAsyncCursor<TField> Distinct<TField>(FieldDefinition<TDocument, TField> field, FilterDefinition<TDocument> filter, DistinctOptions options = null, CancellationToken cancellationToken = default)
        {
            return new EmptyCursor<TField>();
        }

        /// <inheritdoc />
        public override IAsyncCursor<TField> Distinct<TField>(IClientSessionHandle session, FieldDefinition<TDocument, TField> field, FilterDefinition<TDocument> filter, DistinctOptions options = null, CancellationToken cancellationToken = default)
        {
            return new EmptyCursor<TField>();
        }

        /// <inheritdoc />
        public override async Task<IAsyncCursor<TField>> DistinctAsync<TField>(FieldDefinition<TDocument, TField> field, FilterDefinition<TDocument> filter, DistinctOptions options = null, CancellationToken cancellationToken = default)
        {
            await Task.FromResult(1);
            return new EmptyCursor<TField>();
        }

        /// <inheritdoc />
        public override async Task<IAsyncCursor<TField>> DistinctAsync<TField>(IClientSessionHandle session, FieldDefinition<TDocument, TField> field, FilterDefinition<TDocument> filter, DistinctOptions options = null, CancellationToken cancellationToken = default)
        {
            await Task.FromResult(1);
            return new EmptyCursor<TField>();
        }

        /// <inheritdoc />
        public override long EstimatedDocumentCount(EstimatedDocumentCountOptions options = null, CancellationToken cancellationToken = default)
        {
            return 1;
        }

        /// <inheritdoc />
        public override async Task<long> EstimatedDocumentCountAsync(EstimatedDocumentCountOptions options = null, CancellationToken cancellationToken = default)
        {
            await Task.FromResult(1);
            return 1;
        }

        /// <inheritdoc />
        public override IAsyncCursor<TProjection> FindSync<TProjection>(FilterDefinition<TDocument> filter, FindOptions<TDocument, TProjection> options = null, CancellationToken cancellationToken = default)
        {
            return new EmptyCursor<TProjection>();
        }

        /// <inheritdoc />
        public override IAsyncCursor<TProjection> FindSync<TProjection>(IClientSessionHandle session, FilterDefinition<TDocument> filter, FindOptions<TDocument, TProjection> options = null, CancellationToken cancellationToken = default)
        {
            return new EmptyCursor<TProjection>();
        }

        /// <inheritdoc />
        public override async Task<IAsyncCursor<TProjection>> FindAsync<TProjection>(FilterDefinition<TDocument> filter, FindOptions<TDocument, TProjection> options = null, CancellationToken cancellationToken = default)
        {
            await Task.FromResult(1);
            return new EmptyCursor<TProjection>();
        }

        /// <inheritdoc />
        public override async Task<IAsyncCursor<TProjection>> FindAsync<TProjection>(IClientSessionHandle session, FilterDefinition<TDocument> filter, FindOptions<TDocument, TProjection> options = null, CancellationToken cancellationToken = default)
        {
            await Task.FromResult(1);
            return new EmptyCursor<TProjection>();
        }

        /// <inheritdoc />
        public override TProjection FindOneAndDelete<TProjection>(FilterDefinition<TDocument> filter, FindOneAndDeleteOptions<TDocument, TProjection> options = null, CancellationToken cancellationToken = default)
        {
            return default;
        }

        /// <inheritdoc />
        public override TProjection FindOneAndDelete<TProjection>(IClientSessionHandle session, FilterDefinition<TDocument> filter, FindOneAndDeleteOptions<TDocument, TProjection> options = null, CancellationToken cancellationToken = default)
        {
            return default;
        }

        /// <inheritdoc />
        public override async Task<TProjection> FindOneAndDeleteAsync<TProjection>(FilterDefinition<TDocument> filter, FindOneAndDeleteOptions<TDocument, TProjection> options = null, CancellationToken cancellationToken = default)
        {
            await Task.FromResult(1);
            return default;
        }

        /// <inheritdoc />
        public override async Task<TProjection> FindOneAndDeleteAsync<TProjection>(IClientSessionHandle session, FilterDefinition<TDocument> filter, FindOneAndDeleteOptions<TDocument, TProjection> options = null, CancellationToken cancellationToken = default)
        {
            await Task.FromResult(1);
            return default;
        }

        /// <inheritdoc />
        public override TProjection FindOneAndReplace<TProjection>(FilterDefinition<TDocument> filter, TDocument replacement, FindOneAndReplaceOptions<TDocument, TProjection> options = null, CancellationToken cancellationToken = default)
        {
            return default;
        }

        /// <inheritdoc />
        public override TProjection FindOneAndReplace<TProjection>(IClientSessionHandle session, FilterDefinition<TDocument> filter, TDocument replacement, FindOneAndReplaceOptions<TDocument, TProjection> options = null, CancellationToken cancellationToken = default)
        {
            return default;
        }

        /// <inheritdoc />
        public override Task<TProjection> FindOneAndReplaceAsync<TProjection>(FilterDefinition<TDocument> filter, TDocument replacement, FindOneAndReplaceOptions<TDocument, TProjection> options = null, CancellationToken cancellationToken = default)
        {
            return default;
        }

        /// <inheritdoc />
        public override async Task<TProjection> FindOneAndReplaceAsync<TProjection>(IClientSessionHandle session, FilterDefinition<TDocument> filter, TDocument replacement, FindOneAndReplaceOptions<TDocument, TProjection> options = null, CancellationToken cancellationToken = default)
        {
            await Task.FromResult(1);
            return default;
        }

        /// <inheritdoc />
        public override TProjection FindOneAndUpdate<TProjection>(FilterDefinition<TDocument> filter, UpdateDefinition<TDocument> update, FindOneAndUpdateOptions<TDocument, TProjection> options = null, CancellationToken cancellationToken = default)
        {
            return default;
        }

        /// <inheritdoc />
        public override TProjection FindOneAndUpdate<TProjection>(IClientSessionHandle session, FilterDefinition<TDocument> filter, UpdateDefinition<TDocument> update, FindOneAndUpdateOptions<TDocument, TProjection> options = null, CancellationToken cancellationToken = default)
        {
            return default;
        }

        /// <inheritdoc />
        public override async Task<TProjection> FindOneAndUpdateAsync<TProjection>(FilterDefinition<TDocument> filter, UpdateDefinition<TDocument> update, FindOneAndUpdateOptions<TDocument, TProjection> options = null, CancellationToken cancellationToken = default)
        {
            await Task.FromResult(1);
            return default;
        }

        /// <inheritdoc />
        public override async Task<TProjection> FindOneAndUpdateAsync<TProjection>(IClientSessionHandle session, FilterDefinition<TDocument> filter, UpdateDefinition<TDocument> update, FindOneAndUpdateOptions<TDocument, TProjection> options = null, CancellationToken cancellationToken = default)
        {
            await Task.FromResult(1);
            return default;
        }

        /// <inheritdoc />
        public override IAsyncCursor<TResult> MapReduce<TResult>(BsonJavaScript map, BsonJavaScript reduce, MapReduceOptions<TDocument, TResult> options = null, CancellationToken cancellationToken = default)
        {
            return new EmptyCursor<TResult>();
        }

        /// <inheritdoc />
        public override IAsyncCursor<TResult> MapReduce<TResult>(IClientSessionHandle session, BsonJavaScript map, BsonJavaScript reduce, MapReduceOptions<TDocument, TResult> options = null, CancellationToken cancellationToken = default)
        {
            return new EmptyCursor<TResult>();
        }

        /// <inheritdoc />
        public override async Task<IAsyncCursor<TResult>> MapReduceAsync<TResult>(BsonJavaScript map, BsonJavaScript reduce, MapReduceOptions<TDocument, TResult> options = null, CancellationToken cancellationToken = default)
        {
            await Task.FromResult(1);
            return new EmptyCursor<TResult>();
        }

        /// <inheritdoc />
        public override async Task<IAsyncCursor<TResult>> MapReduceAsync<TResult>(IClientSessionHandle session, BsonJavaScript map, BsonJavaScript reduce, MapReduceOptions<TDocument, TResult> options = null, CancellationToken cancellationToken = default)
        {
            await Task.FromResult(1);
            return new EmptyCursor<TResult>();
        }

        /// <inheritdoc />
        public override IFilteredMongoCollection<TDerivedDocument> OfType<TDerivedDocument>()
        {
            return default;
        }

        /// <inheritdoc />
        public override IChangeStreamCursor<TResult> Watch<TResult>(
            PipelineDefinition<ChangeStreamDocument<TDocument>, TResult> pipeline,
            ChangeStreamOptions options = null,
            CancellationToken cancellationToken = default)
        {
            return default;
        }

        /// <inheritdoc />
        public override IChangeStreamCursor<TResult> Watch<TResult>(
            IClientSessionHandle session,
            PipelineDefinition<ChangeStreamDocument<TDocument>, TResult> pipeline,
            ChangeStreamOptions options = null,
            CancellationToken cancellationToken = default)
        {
            return default;
        }

        /// <inheritdoc />
        public override Task<IChangeStreamCursor<TResult>> WatchAsync<TResult>(
            PipelineDefinition<ChangeStreamDocument<TDocument>, TResult> pipeline,
            ChangeStreamOptions options = null,
            CancellationToken cancellationToken = default)
        {
            return default;
        }

        /// <inheritdoc />
        public override Task<IChangeStreamCursor<TResult>> WatchAsync<TResult>(
            IClientSessionHandle session,
            PipelineDefinition<ChangeStreamDocument<TDocument>, TResult> pipeline,
            ChangeStreamOptions options = null,
            CancellationToken cancellationToken = default)
        {
            return default;
        }

        /// <inheritdoc />
        public override IMongoCollection<TDocument> WithReadConcern(ReadConcern readConcern)
        {
            return default;
        }

        /// <inheritdoc />
        public override IMongoCollection<TDocument> WithReadPreference(ReadPreference readPreference)
        {
            return this;
        }

        /// <inheritdoc />
        public override IMongoCollection<TDocument> WithWriteConcern(WriteConcern writeConcern)
        {
            return this;
        }
    }
}
