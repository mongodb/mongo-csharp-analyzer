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

using MongoDB.Analyzer.Tests.Common.DataModel;
using MongoDB.Driver;

namespace MongoDB.Analyzer.Tests.Common.TestCases.Builders
{
    public sealed class BuildersNullForgivingOperators : TestCasesBase
    {
        [BuildersMQL("{ \"StringField\" : \"value\" }")]
        [BuildersMQL("{ \"Age\" : 1 }")]
        public void Identifier()
        {
            _ = Builders<ClassWithObjectId>.Filter.Eq(c => c!.StringField, "value");
            _ = Builders<User>.IndexKeys.Ascending(x => x!.Age);
        }

        [BuildersMQL("{ \"Name\" : \"name\" }")]
        [BuildersMQL("{ \"Name\" : null }")]
        public void Literals()
        {
            _ = Builders<Person>.Filter.Eq(p => p.Name, "name"!);
            _ = Builders<Person>.Filter.Eq(p => p.Name, null!);
        }

        [BuildersMQL("{ \"Name\" : GetNullableString() }")]
        [BuildersMQL("{ \"Name\" : name }")]
        public void Methods()
        {
            _ = Builders<Person>.Filter.Eq(p => p.Name, GetNullableString()!);

            string? name = GetNullableString();
            _ = Builders<Person>.Filter.Eq(p => p.Name, name!);
        }

        [BuildersMQL("{ \"Address.City\" : \"Boston\" }")]
        [BuildersMQL("{ \"Address.City\" : \"Boston\" }")]
        [BuildersMQL("{ \"Address.City\" : \"Boston\" }")]
        [BuildersMQL("{ \"Address.City\" : \"Boston\" }")]
        [BuildersMQL("{ \"Address.City\" : \"Boston\" }")]
        [BuildersMQL("{ \"Address.City\" : \"Boston\" }")]
        [BuildersMQL("{ \"Address.City\" : \"Boston\" }")]
        public void Nested()
        {
            _ = Builders<Person>.Filter.Eq(p => p!.Address!.City!, "Boston");
            _ = Builders<Person>.Filter.Eq(p => p!.Address.City!, "Boston");
            _ = Builders<Person>.Filter.Eq(p => p!.Address!.City, "Boston");
            _ = Builders<Person>.Filter.Eq(p => p.Address!.City!, "Boston");
            _ = Builders<Person>.Filter.Eq(p => p.Address!.City, "Boston");
            _ = Builders<Person>.Filter.Eq(p => p.Address.City!, "Boston");
            _ = Builders<Person>.Filter.Eq(p => p!.Address.City, "Boston");
        }

        [BuildersMQL("{ \"Age\" : 1 }")]
        public void Property()
        {
            _ = Builders<User>.IndexKeys.Ascending(x => x.Age!);
        }

        [BuildersMQL("{ \"Name\" : name }")]
        [BuildersMQL("{ \"LastName\" : lastName }")]
        public void Variables()
        {
            string? name = "name";
            _ = Builders<Person>.Filter.Eq(p => p.Name, name!);

            string? lastName = null;
            _ = Builders<Person>.Filter.Eq(p => p.LastName, lastName!);
        }

        private string? GetNullableString() => "string";
    }
}
