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

using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace MongoDB.Analyzer.Helpers.Builders
{
    public static class Renderer
    {
        public static string Render<T>(FilterDefinition<T> filterDefinition)
        {
            var renderedBuildersDefinition = filterDefinition.Render(BsonSerializer.LookupSerializer<T>(), BsonSerializer.SerializerRegistry);
            return renderedBuildersDefinition.ToString();
        }

        public static string Render<T>(UpdateDefinition<T> updateDefinition)
        {
            var renderedBuildersDefinition = updateDefinition.Render(BsonSerializer.LookupSerializer<T>(), BsonSerializer.SerializerRegistry);
            return renderedBuildersDefinition.ToString();
        }

        public static string Render<T>(SortDefinition<T> sortDefinition)
        {
            var renderedBuildersDefinition = sortDefinition.Render(BsonSerializer.LookupSerializer<T>(), BsonSerializer.SerializerRegistry);
            return renderedBuildersDefinition.ToString();
        }

        public static string Render<T>(IndexKeysDefinition<T> indexDefinition)
        {
            var renderedBuildersDefinition = indexDefinition.Render(BsonSerializer.LookupSerializer<T>(), BsonSerializer.SerializerRegistry);
            return renderedBuildersDefinition.ToString();
        }

        public static string Render<T>(ProjectionDefinition<T> projectionDefinition)
        {
            var renderedBuildersDefinition = projectionDefinition.Render(BsonSerializer.LookupSerializer<T>(), BsonSerializer.SerializerRegistry);
            return renderedBuildersDefinition.ToString();
        }

        public static string Render<T, E>(ProjectionDefinition<T, E> projectionDefinition)
        {
            var renderedBuildersDefinition = projectionDefinition.Render(BsonSerializer.LookupSerializer<T>(), BsonSerializer.SerializerRegistry);
            return renderedBuildersDefinition.Document.ToString();
        }

        public static string Render<T, E>(IFindFluent<T, E> fluentDefinition)
        {
            return fluentDefinition.ToString();
        }
    }
}

