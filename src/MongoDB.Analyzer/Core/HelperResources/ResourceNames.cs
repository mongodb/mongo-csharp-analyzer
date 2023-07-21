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

namespace MongoDB.Analyzer.Core;

internal static class ResourceNames
{
    public const string HelperResourcesBasePath = "MongoDB.Analyzer.Core.HelperResources";

    internal static class Linq
    {
        public const string IQueryableProvider = $"Linq.{nameof(IQueryableProvider)}";
        public const string IQueryableProviderV2 = $"Linq.{nameof(IQueryableProviderV2)}";
        public const string IQueryableProviderV3 = $"Linq.{nameof(IQueryableProviderV3)}";
        public const string MqlGenerator = $"Linq.{nameof(MqlGenerator)}";
    }

    internal static class Builders
    {
        public const string MqlGenerator = $"Builders.{nameof(MqlGenerator)}";
        public const string Renderer = $"Builders.{nameof(Renderer)}";
        public const string Renderer_2_19_and_higher = $"Builders.{nameof(Renderer_2_19_and_higher)}";
    }

    internal static class Json
    {
        public const string JsonGenerator = $"Json.{nameof(JsonGenerator)}";
    }

    public const string EmptyCursor = nameof(EmptyCursor);
    public const string MongoClientMock = nameof(MongoClientMock);
    public const string MongoCollectionMock = nameof(MongoCollectionMock);
    public const string MongoDatabaseMock = nameof(MongoDatabaseMock);
}
