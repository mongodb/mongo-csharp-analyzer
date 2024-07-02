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
using MongoDB.Bson;
using MongoDB.Driver;

#pragma warning disable IDE0005
using common = MongoDB.Analyzer.Tests.Common;
using dataModel = MongoDB.Analyzer.Tests.Common.DataModel;
using driver = MongoDB.Driver;
using mongo = MongoDB;
using user = MongoDB.Analyzer.Tests.Common.DataModel.User;
#pragma warning restore IDE0005

namespace MongoDB.Analyzer.Tests.Common.TestCases.Builders
{
    public sealed class BuildersQualifiedNames : TestCasesBase
    {
        [BuildersMQL("{ \"Age\" : 22 }")]
        [BuildersMQL("{ \"Age\" : 22 }")]
        [BuildersMQL("{ \"Age\" : 22 }")]
        [BuildersMQL("{ \"Age\" : 22 }")]
        [BuildersMQL("{ \"Age\" : 22 }")]
        [BuildersMQL("{ \"ByteNullable\" : common::DataModel.StaticHolder.ReadonlyByteNullable }")]
        [BuildersMQL("{ \"ByteNullable\" : dataModel::StaticHolder.ReadonlyByteNullable }")]
        public void Qualified_alias()
        {
            _ = Builders<user>.Filter.Eq(user => user.Age, 22);
            _ = Builders<dataModel::User>.Filter.Eq(user => user.Age, 22);
            _ = Builders<common::DataModel.User>.Filter.Eq(user => user.Age, 22);

            _ = driver.Builders<User>.Filter.Eq(user => user.Age, 22);
            _ = mongo.Driver.Builders<User>.Filter.Eq(user => user.Age, 22);

            _ = Builders<NullableHolder>.Filter.Eq(n => n.ByteNullable, common::DataModel.StaticHolder.ReadonlyByteNullable);
            _ = Builders<NullableHolder>.Filter.Eq(n => n.ByteNullable, dataModel::StaticHolder.ReadonlyByteNullable);
        }

        [BuildersMQL("find({ \"$or\" : [{ \"Age\" : { \"$lt\" : 10 } }, { \"Age\" : { \"$gt\" : 20 } }, { \"Name\" : { \"$ne\" : \"Bob\" }, \"LastName\" : { \"$ne\" : null } }] })")]
        public void Qualified_fluent_api()
        {
            var mongoCollection = GetMongoCollection();
            mongoCollection.Find(
                u => u.Age < 10 || u.Age > 20 || (u.Name != "Bob" && u.LastName != null),
                new MongoDB.Driver.FindOptions() { MaxTime = System.TimeSpan.FromSeconds(3), Max = new MongoDB.Bson.BsonDocument("min", 2) });
        }

        [BuildersMQL("{ \"field\" : { \"$elemMatch\" : { \"field\" : fieldValue } } }")]
        [BuildersMQL("{ \"ByteNullable\" : MongoDB.Analyzer.Tests.Common.DataModel.StaticHolder.ReadonlyByteNullable }")]
        [BuildersMQL("{ \"$or\" : [{ \"SiblingsCount\" : MongoDB.Analyzer.Tests.Common.DataModel.StaticHolder.ReadonlyByte }, { \"SiblingsCount\" : Analyzer.Tests.Common.DataModel.StaticHolder.ReadonlyShort }, { \"SiblingsCount\" : Tests.Common.DataModel.StaticHolder.ReadonlyInt }, { \"TicksSinceBirth\" : NumberLong(Common.DataModel.StaticHolder.ReadonlyLong) }, { \"Name\" : DataModel.StaticHolder.ReadonlyString }, { \"Name\" : StaticHolder.ReadonlyString }] }")]
        [BuildersMQL("{ \"Age\" : 22 }")]
        [BuildersMQL("{ \"Age\" : 22 }")]
        [BuildersMQL("{ \"Age\" : 22 }")]
        [BuildersMQL("{ \"Age\" : 22 }")]
        [BuildersMQL("{ \"Age\" : 22 }")]
        [BuildersMQL("{ \"Age\" : 22 }")]
        [BuildersMQL("{ \"Age\" : 22 }")]
        [BuildersMQL("{ \"Age\" : 22 }")]
        public void Qualified_type_names()
        {
            var fieldValue = "fieldValue";
            _ = Builders<BsonDocument>.Filter.ElemMatch("field", MongoDB.Driver.Builders<MongoDB.Bson.BsonValue>.Filter.Eq("field", fieldValue));

            _ = Builders<NullableHolder>.Filter.Eq(n => n.ByteNullable, MongoDB.Analyzer.Tests.Common.DataModel.StaticHolder.ReadonlyByteNullable);

            _ = Builders<Person>.Filter.Eq(p => p.SiblingsCount, MongoDB.Analyzer.Tests.Common.DataModel.StaticHolder.ReadonlyByte) |
                Builders<Person>.Filter.Eq(p => p.SiblingsCount, Analyzer.Tests.Common.DataModel.StaticHolder.ReadonlyShort) |
                Builders<Person>.Filter.Eq(p => p.SiblingsCount, Tests.Common.DataModel.StaticHolder.ReadonlyInt) |
                Builders<Person>.Filter.Eq(p => p.TicksSinceBirth, Common.DataModel.StaticHolder.ReadonlyLong) |
                Builders<Person>.Filter.Eq(p => p.Name, DataModel.StaticHolder.ReadonlyString) |
                Builders<Person>.Filter.Eq(p => p.Name, StaticHolder.ReadonlyString);

            _ = Builders<MongoDB.Analyzer.Tests.Common.DataModel.User>.Filter.Eq(user => user.Age, 22);
            _ = Builders<Analyzer.Tests.Common.DataModel.User>.Filter.Eq(user => user.Age, 22);
            _ = Builders<Tests.Common.DataModel.User>.Filter.Eq(user => user.Age, 22);
            _ = Builders<Common.DataModel.User>.Filter.Eq(user => user.Age, 22);
            _ = Builders<DataModel.User>.Filter.Eq(user => user.Age, 22);

            _ = MongoDB.Driver.Builders<User>.Filter.Eq(user => user.Age, 22);
            _ = Driver.Builders<User>.Filter.Eq(user => user.Age, 22);
            _ = Builders<User>.Filter.Eq(user => user.Age, 22);
        }
    }
}
