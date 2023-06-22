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

using MongoDB.Driver;
using System.Linq;
using BsonDocumentCustom123 = MongoDB.Bson.BsonDocument;
using BsonValueCustom123 = MongoDB.Bson.BsonValue;
using BsonObjectIdCustom123 = MongoDB.Bson.BsonObjectId;
using BsonTypeCustom123 = MongoDB.Bson.BsonType;
using BsonTimeSpanCustom123 = MongoDB.Bson.Serialization.Options.TimeSpanUnits;

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
        private static readonly BsonTimeSpanCustom123 s_dummyRef5;
#pragma warning restore IDE0051 // The field is never used
#pragma warning restore CS0169

        private sealed class MqlGeneratorTemplateType
        {
            public int Field { get; set; }
        }

        public static string GetDriverVersion() => typeof(Builders<>).Assembly.GetName().Version.ToString(3);
        public static int[] LinqReference => Enumerable.Range(1, 10).ToArray();

        public static string GetMQL()
        {
            var collection = new MongoCollectionMock<MqlGeneratorTemplateType>();
            var buildersDefinition = Builders<MqlGeneratorTemplateType>.Filter.Gt(p => p.Field, 10);
            return Renderer.Render(buildersDefinition);
        }
    }
}
