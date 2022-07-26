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
using System.Linq;
using BsonDocumentCustom123 = MongoDB.Bson.BsonDocument;
using BsonValueCustom123 = MongoDB.Bson.BsonValue;
using BsonObjectIdCustom123 = MongoDB.Bson.BsonObjectId;
using BsonTypeCustom123 = MongoDB.Bson.BsonType;
using System;
using System.Reflection;

namespace MongoDB.Analyzer.Helpers.Builders
{
    public static class MqlGenerator
    {
#pragma warning disable CS0169 // The field is never used
#pragma warning disable IDE0051
        private static readonly BsonDocumentCustom123 s_dummyRef1;
        private static readonly BsonValueCustom123 s_dummyRef2;
        private static readonly BsonObjectIdCustom123 s_dummyRef3;
        private static readonly BsonTypeCustom123 s_dummyRef4;
#pragma warning restore IDE0051 // The field is never used
#pragma warning restore CS0169

        private sealed class MqlGeneratorTemplateType
        {
            public int Field { get; set; }
        }

        public static string GetDriverVersion() => typeof(Builders<>).Assembly.GetName().Version.ToString(3);
        public static int[] LinqReference => Enumerable.Range(1, 10).ToArray();

        private static string Render<T, E>(object buildersDefinition)
        {
            var buildersDefinitionType = buildersDefinition.GetType();
            if (buildersDefinitionType.IsSubclassOf(typeof(ProjectionDefinition<T, E>)) ||
                buildersDefinitionType == typeof(ProjectionDefinition<T, E>))
            {
                var renderedBuildersFilter = ((ProjectionDefinition<T, E>)buildersDefinition).Render(BsonSerializer.LookupSerializer<T>(), BsonSerializer.SerializerRegistry);
                return renderedBuildersFilter.Document.ToString();
            }
            else if (buildersDefinitionType.IsSubclassOf(typeof(ProjectionDefinition<T>)) ||
                buildersDefinitionType == typeof(ProjectionDefinition<T>))
            {
                var renderedBuildersFilter = ((ProjectionDefinition<T>)buildersDefinition).Render(BsonSerializer.LookupSerializer<T>(), BsonSerializer.SerializerRegistry);
                return renderedBuildersFilter.ToString();
            }
            else if (buildersDefinitionType.IsSubclassOf(typeof(IndexKeysDefinition<T>)) ||
                     buildersDefinitionType == typeof(IndexKeysDefinition<T>))
            {
                var renderedBuildersFilter = ((IndexKeysDefinition<T>)buildersDefinition).Render(BsonSerializer.LookupSerializer<T>(), BsonSerializer.SerializerRegistry);
                return renderedBuildersFilter.ToString();
            }
            else if (buildersDefinitionType.IsSubclassOf(typeof(SortDefinition<T>)) ||
                     buildersDefinitionType == typeof(SortDefinition<T>))
            {
                var renderedBuildersFilter = ((SortDefinition<T>)buildersDefinition).Render(BsonSerializer.LookupSerializer<T>(), BsonSerializer.SerializerRegistry);
                return renderedBuildersFilter.ToString();
            }
            else if (buildersDefinitionType.IsSubclassOf(typeof(UpdateDefinition<T>)) ||
                     buildersDefinitionType == typeof(UpdateDefinition<T>))
            {
                var renderedBuildersFilter = ((UpdateDefinition<T>)buildersDefinition).Render(BsonSerializer.LookupSerializer<T>(), BsonSerializer.SerializerRegistry);
                return renderedBuildersFilter.ToString();
            }
            else if (buildersDefinitionType.IsSubclassOf(typeof(FilterDefinition<T>)) ||
                     buildersDefinitionType == typeof(FilterDefinition<T>))
            {
                var renderedBuildersFilter = ((FilterDefinition<T>)buildersDefinition).Render(BsonSerializer.LookupSerializer<T>(), BsonSerializer.SerializerRegistry);
                return renderedBuildersFilter.ToString();
            }
            return null;
        }

        private static string RenderBuildersDefinition(object buildersDefinition)
        {
            var genericTypes = buildersDefinition.GetType().GetGenericArguments();
            var bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
            var methodInfo = typeof(MqlGenerator).GetMethod(nameof(MqlGenerator.Render), bindingFlags);

            Type[] genericTypeParameters = new Type[2];

            if (genericTypes.Count() == 1)
            {
                genericTypeParameters[0] = genericTypeParameters[1] = genericTypes[0];
            }
            else if (genericTypes.Count() == 2)
            {
                genericTypeParameters[0] = genericTypes[0];
                genericTypeParameters[1] = genericTypes[1];
            }
            
            MethodInfo generic = methodInfo.MakeGenericMethod(genericTypeParameters);
            object[] parameters = new object[] { buildersDefinition };
            var returnValue = generic.Invoke(null, parameters);
            if (returnValue is string str)
            {
                return str;
            }
            return null;
        }

        public static string GetMQL()
        {
            var buildersDefinition = Builders<MqlGeneratorTemplateType>.Filter.Gt(p => p.Field, 10);
            var filterRendered = buildersDefinition.Render(BsonSerializer.LookupSerializer<MqlGeneratorTemplateType>(), BsonSerializer.SerializerRegistry);
            return RenderBuildersDefinition(buildersDefinition);
        }
    }
}
