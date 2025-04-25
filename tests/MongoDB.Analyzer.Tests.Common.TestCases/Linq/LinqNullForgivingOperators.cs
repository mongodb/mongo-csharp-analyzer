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
using System.Xml.Linq;
using MongoDB.Analyzer.Tests.Common.DataModel;
using MongoDB.Driver.Linq;

namespace MongoDB.Analyzer.Tests.Common.TestCases.Linq
{
    public sealed class LinqNullForgivingOperators : TestCasesBase
    {
        [MQL("Aggregate([{ \"$match\" : { \"StringField\" : \"value\" } }])")]
        [MQL("Aggregate([{ \"$match\" : { \"Name\" : \"Bob\" } }])")]
        public void Identifier()
        {
            _ = GetMongoQueryable<ClassWithObjectId>().Where(c => c!.StringField == "value");
            _ = GetMongoQueryable<User>().Where(u => u!.Name == "Bob");
        }

        [MQL("Aggregate([{ \"$match\" : { \"FieldString\" : \"Bob\", \"PropertyArray.0\" : 1 } }, { \"$match\" : { \"FieldMixedDataMembers.FieldString\" : \"Alice\" } }])")]
        [MQL("Aggregate([{ \"$match\" : { \"StringField\" : null } }])")]
        public void Literals()
        {
            _ = GetMongoQueryable<MixedDataMembers>()
                .Where(u => u.FieldString == "Bob"! && u.PropertyArray[0] == 1)
                .Where(u => u.FieldMixedDataMembers.FieldString == "Alice"!);

            _ = GetMongoQueryable<ClassWithObjectId>().Where(c => c.StringField == null!);
        }

        [MQL("Aggregate([{ \"$match\" : { \"Name\" : GetNullableString() } }])")]
        [MQL("Aggregate([{ \"$match\" : { \"Name\" : name } }])")]
        public void Methods()
        {
            _ = GetMongoQueryable<User>().Where(u => u.Name == GetNullableString()!);

            string? name = GetNullableString();
            _ = GetMongoQueryable<User>().Where(u => u.Name == name!);
        }

        [MQL("Aggregate([{ \"$match\" : { \"Address.City\" : \"Boston\" } }])")]
        [MQL("Aggregate([{ \"$match\" : { \"Address.City\" : \"Boston\" } }])")]
        [MQL("Aggregate([{ \"$match\" : { \"Address.City\" : \"Boston\" } }])")]
        [MQL("Aggregate([{ \"$match\" : { \"Address.City\" : \"Boston\" } }])")]
        [MQL("Aggregate([{ \"$match\" : { \"Address.City\" : \"Boston\" } }])")]
        [MQL("Aggregate([{ \"$match\" : { \"Address.City\" : \"Boston\" } }])")]
        [MQL("Aggregate([{ \"$match\" : { \"Address.City\" : \"Boston\" } }])")]
        public void Nested()
        {
            _ = GetMongoQueryable<Person>().Where(p => p!.Address!.City! == "Boston");
            _ = GetMongoQueryable<Person>().Where(p => p!.Address!.City == "Boston");
            _ = GetMongoQueryable<Person>().Where(p => p!.Address.City! == "Boston");
            _ = GetMongoQueryable<Person>().Where(p => p.Address!.City! == "Boston");
            _ = GetMongoQueryable<Person>().Where(p => p.Address.City! == "Boston");
            _ = GetMongoQueryable<Person>().Where(p => p.Address!.City == "Boston");
            _ = GetMongoQueryable<Person>().Where(p => p!.Address.City == "Boston");
        }

        [MQL("Aggregate([{ \"$match\" : { \"Name\" : \"Bob\" } }])")]
        public void Property()
        {
            _ = GetMongoQueryable<User>().Where(u => u.Name! == "Bob");
        }

        [MQL("Aggregate([{ \"$match\" : { \"Name\" : name } }])")]
        [MQL("Aggregate([{ \"$match\" : { \"LastName\" : lastName } }])")]
        public void Variables()
        {
            string? name = "name";
            _ = GetMongoQueryable<Person>().Where(p => p.Name == name!);

            string? lastName = null;
            _ = GetMongoQueryable<Person>().Where(p => p.LastName == lastName!);
        }

        private string? GetNullableString() => "string";
    }
}
