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

using System.Linq;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Options;
using MongoDB.Driver;

namespace MongoDB.Analyzer.Helpers.Builders
{
    public static class MqlGenerator
    {
#pragma warning disable CS0169 // These fields are never used, they are needed to ensure that the relevant usings are not accidently removed
#pragma warning disable IDE0051
        private static readonly BsonType s_dummyRef1;
        private static readonly TimeSpanUnits s_dummyRef2;
#pragma warning restore IDE0051 // These fields are never used, they are needed to ensure that the relevant usings are not accidently removed
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
