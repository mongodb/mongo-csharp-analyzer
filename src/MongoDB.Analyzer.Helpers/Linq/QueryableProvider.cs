﻿// Copyright 2021-present MongoDB Inc.
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

using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace MongoDB.Analyzer.Helpers.Linq
{
    public static class QueryableProvider
    {

#if DRIVER_2_14_OR_GREATER
        public static IMongoQueryable<TDocument> GetQueryable<TDocument>(bool isV3)
        {
            var provider = isV3 ? LinqProviderAdapter.V3 : LinqProviderAdapter.V2;
            return provider.AsQueryable(new MongoCollectionMock<TDocument>(), null, new AggregateOptions());
        }
#else
        public static IMongoQueryable<TDocument> GetQueryable<TDocument>(bool isV3) =>
            !isV3 ? (new MongoCollectionMock<TDocument>()).AsQueryable() : null;
#endif
    }
}
