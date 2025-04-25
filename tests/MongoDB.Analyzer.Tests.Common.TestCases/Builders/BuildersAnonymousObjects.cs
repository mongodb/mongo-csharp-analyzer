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
using MongoDB.Analyzer.Tests.Common.DataModel;
using MongoDB.Driver;

namespace MongoDB.Analyzer.Tests.Common.TestCases.Builders
{
    public sealed class BuildersAnonymousObjects : TestCasesBase
    {
        [BuildersMQL("{ \"Age\" : new { Age = value << 2, IntField = new { Value = 10 }.Value, Address = \"White House\" }.Age }")]
        [BuildersMQL("{ \"Age\" : new { Age = value << 2, IntField = Math.Round(2.35, new { DecimalPoints = 2 }.DecimalPoints), Address = \"White House\".ToLower() }.Age }")]
        [BuildersMQL("{ \"Age\" : new { Age = new { ShiftedValue = value << 2 }.ShiftedValue, IntField = Math.Round(2.35, new { DecimalPoints = 2 }.DecimalPoints), Address = address.City.ToUpper() }.Age }")]
        public void Complex_anonymous_object()
        {
            int value = 10;
            _ = Builders<User>.Filter.Eq(u => u.Age, new { Age = value << 2, IntField = new { Value = 10 }.Value, Address = "White House" }.Age);
            _ = Builders<User>.Filter.Eq(u => u.Age, new { Age = value << 2, IntField = Math.Round(2.35, new { DecimalPoints = 2 }.DecimalPoints), Address = "White House".ToLower() }.Age);

            var address = new Address();
            _ = Builders<User>.Filter.Eq(u => u.Age, new { Age = new { ShiftedValue = value << 2 }.ShiftedValue, IntField = Math.Round(2.35, new { DecimalPoints = 2 }.DecimalPoints), Address = address.City.ToUpper() }.Age);
        }

        [BuildersMQL("{ \"Age\" : new { Age = value << 2, IntField = 22 }.Age }")]
        [BuildersMQL("{ \"Scores\" : { \"$gt\" : new { Item = value, IntField = 22 }.Item } }")]
        [BuildersMQL("{ \"PesonsList\" : { \"$elemMatch\" : { \"SiblingsCount\" : new { SiblingsCount = 2 }.SiblingsCount } } }")]
        [BuildersMQL("{ \"SiblingsCount\" : { \"$lte\" : anonymousObject.Item } }")]
        [BuildersMQL("{ \"Root.Data\" : { \"$lt\" : new { RootValue = 22 }.RootValue } }")]
        [BuildersMQL("{ new { Field = \"IntList\" }.Field : { \"$gt\" : new { Value = 22 }.Value } }")]
        [BuildersMQL("{ new { Field = \"Age\" }.Field : { \"$mod\" : [new { Value = 21 }.Value, new { Value = 23 }.Value] } }")]
        [BuildersMQL("{ new { Field = \"Age\" }.Field : { \"$ne\" : new { Age = (value << 2) * (value / 2) }.Age } }")]
        [BuildersMQL("{ new { Field = \"IntArray\" }.Field : { \"$in\" : [11, 22, 33] } }")]
        [BuildersMQL("{ \"$or\" : [{ new { Field = \"IntArray\" }.Field : { \"$gt\" : new { Value = 123 }.Value } }, { new { Field = \"ObjectArray\" }.Field : { \"$ne\" : null } }] }")]
        public void Filter()
        {
            int value = 10;
            _ = Builders<User>.Filter.Eq(u => u.Age, new { Age = value << 2, IntField = 22 }.Age);
            _ = Builders<User>.Filter.AnyGt("Scores", new { Item = value, IntField = 22 }.Item);
            _ = Builders<ListsHolder>.Filter.ElemMatch(u => u.PesonsList, p => p.SiblingsCount == new { SiblingsCount = 2 }.SiblingsCount);

            var anonymousObject = new { Item = 10, BooleanValue = false, StringValue = "String" };
            _ = Builders<Person>.Filter.Lte(u => u.SiblingsCount, anonymousObject.Item);
            _ = Builders<Tree>.Filter.Lt(t => t.Root.Data, new { RootValue = 22 }.RootValue);
            _ = Builders<ListsHolder>.Filter.AnyGt(new { Field = "IntList" }.Field, new { Value = 22 }.Value);

            _ = Builders<User>.Filter.Mod(new { Field = "Age" }.Field, new { Value = 21 }.Value, new { Value = 23 }.Value);
            _ = Builders<User>.Filter.Ne(new { Field = "Age" }.Field, new { Age = (value << 2) * (value / 2) }.Age);
            _ = Builders<SimpleTypesArraysHolder>.Filter.In(new { Field = "IntArray" }.Field, new[] { 11, 22, 33 });

            _ = Builders<SimpleTypesArraysHolder>.Filter.AnyGt(new { Field = "IntArray" }.Field, new { Value = 123 }.Value) |
                Builders<SimpleTypesArraysHolder>.Filter.AnyNe<object>(new { Field = "ObjectArray" }.Field, null);
        }

        [BuildersMQL("find({ \"Age\" : { \"$lt\" : 10 } }, { \"_v\" : { \"$literal\" : { } }, \"_id\" : 0 })")]
        [BuildersMQL("find({ \"Age\" : { \"$lt\" : 10 } }, { \"Address\" : 1, \"_id\" : 0 })")]
        [BuildersMQL("find({ }, { \"A\" : \"$Address\", \"B\" : \"$Scores\", \"C\" : \"$Age\", \"_id\" : 0 })")]
        [BuildersMQL("find({ \"$text\" : { \"$search\" : \"testSearch\" } }, { \"A\" : \"$Address\", \"B\" : { \"$arrayElemAt\" : [\"$Scores\", 0] }, \"C\" : { \"$multiply\" : [\"$Age\", 10] }, \"D\" : { \"$multiply\" : [\"$Height\", \"$Age\", { \"$arrayElemAt\" : [\"$Scores\", 10] }] }, \"_id\" : 0 })")]
        public void FluentApi_project()
        {
            GetMongoCollection()
                .Find(Builders<User>.Filter.Lt(u => u.Age, 10))
                .Project(u => new { });

            GetMongoCollection()
                .Find(Builders<User>.Filter.Lt(u => u.Age, 10))
                .Project(u => new { Address = u.Address });

            GetMongoCollection()
                .Find(Builders<User>.Filter.Empty)
                .Project(u => new { A = u.Address, B = u.Scores, C = u.Age });

            GetMongoCollection()
                .Find(Builders<User>.Filter.Text("testSearch"))
                .Project(u => new { A = u.Address, B = u.Scores[0], C = u.Age * 10, D = u.Height * u.Age * u.Scores[10] });
        }

        [BuildersMQL("{ new { Field = \"Name\" }.Field : 1 }")]
        [BuildersMQL("{ new { Field = \"TicksSinceBirth\" }.Field : \"2d\", new { Field = \"Name\" }.Field : \"2dsphere\" }")]
        [BuildersMQL("{ new { Field = \"Age\" }.Field : \"text\", new { Field = \"LastName\" }.Field : \"text\" }")]
        [BuildersMQL("{ new { Field = \"LicenseNumber\" }.Field : \"geoHaystack\" }")]
        [BuildersMQL("{ new { Field = \"Name\", IntValue = 10, BoolValue = false, DoubleValue = 2.5 }.Field : \"2d\", new { Field = \"LastName\", IntValue = 10, BoolValue = false, DoubleValue = 2.5 }.Field : \"2d\", new { Field = \"Address\", IntValue = 10, BoolValue = false, DoubleValue = 2.5 }.Field : \"text\", new { Field = \"Vehicle\", IntValue = 10, BoolValue = false, DoubleValue = 2.5 }.Field : \"text\" }")]
        public void IndexKey()
        {
            _ = Builders<Person>.IndexKeys.Ascending(new { Field = "Name" }.Field);
            _ = Builders<Person>.IndexKeys.Geo2D(new { Field = "TicksSinceBirth" }.Field).Geo2DSphere(new { Field = "Name" }.Field);
            _ = Builders<User>.IndexKeys.Text(new { Field = "Age" }.Field).Text(new { Field = "LastName" }.Field);
            _ = Builders<Vehicle>.IndexKeys.GeoHaystack(new { Field = "LicenseNumber" }.Field);
            _ = Builders<Person>.IndexKeys.Combine(
                    Builders<Person>.IndexKeys.Geo2D(new { Field = "Name", IntValue = 10, BoolValue = false, DoubleValue = 2.5 }.Field)
                        .Geo2D(new { Field = "LastName", IntValue = 10, BoolValue = false, DoubleValue = 2.5 }.Field),
                    Builders<Person>.IndexKeys.Text(new { Field = "Address", IntValue = 10, BoolValue = false, DoubleValue = 2.5 }.Field)
                        .Text(new { Field = "Vehicle", IntValue = 10, BoolValue = false, DoubleValue = 2.5 }.Field));
        }

        [BuildersMQL("{ \"Age\" : \"$Age\", \"Address\" : \"$Address\", \"_id\" : 0 }")]
        [BuildersMQL("{ \"Average\" : { \"$divide\" : [{ \"$add\" : [\"$SiblingsCount\", \"$TicksSinceBirth\"] }, 2] }, \"Total\" : { \"$add\" : [\"$SiblingsCount\", \"$TicksSinceBirth\"] }, \"_id\" : 0 }")]
        [BuildersMQL("{ \"Avg\" : { \"$divide\" : [{ \"$add\" : [{ \"$multiply\" : [\"$Age\", 2] }, \"$Height\"] }, 2] }, \"_id\" : 0 }")]
        [BuildersMQL("{ new { FieldToExclude = \"Age\" }.FieldToExclude : 0 }")]
        [BuildersMQL("{ new { FieldToInclude = \"Scores\" }.FieldToInclude : 1 }")]
        [BuildersMQL("{ new { Field = \"IntArray\" }.Field : { \"$slice\" : [\"$s__1\", 10, 5] } }")]
        [BuildersMQL("{ new { Field = \"Age\" }.Field : 1, new { Field = \"LastName\" }.Field : 1 }")]
        public void Projection()
        {
            _ = Builders<User>.Projection.Expression(u => new { Age = u.Age, Address = u.Address });
            _ = Builders<Person>.Projection.Expression(u => new { Average = (u.SiblingsCount + u.TicksSinceBirth) / 2, Total = u.SiblingsCount + u.TicksSinceBirth });
            _ = Builders<User>.Projection.Expression(u => new { Avg = (u.Age * 2 + u.Height) / 2 });
            _ = Builders<User>.Projection.Exclude(new { FieldToExclude = "Age" }.FieldToExclude);
            _ = Builders<User>.Projection.Include(new { FieldToInclude = "Scores" }.FieldToInclude);
            _ = Builders<SimpleTypesArraysHolder>.Projection.Slice(new { Field = "IntArray" }.Field, 10, 5);
            _ = Builders<User>.Projection.Combine(
                    Builders<User>.Projection.Include(new { Field = "Age" }.Field),
                    Builders<User>.Projection.Include(new { Field = "LastName" }.Field));
        }

        [BuildersMQL("{ new { Address = \"Address\" }.Address : 1, new { Name = \"Name\" }.Name : -1 }")]
        [BuildersMQL("{ new { Address = \"Address\" }.Address : 1, new { Name = \"Name\" }.Name : -1 }")]
        public void Sort()
        {
            _ = Builders<User>.Sort
                .Ascending(new { Address = "Address" }.Address)
                .Descending(new { Name = "Name" }.Name);

            _ = Builders<User>.Sort.Combine(
                    Builders<User>.Sort.Ascending(new { Address = "Address" }.Address),
                    Builders<User>.Sort.Descending(new { Name = "Name" }.Name));
        }

        [BuildersMQL("{ \"$set\" : { new { Field = \"Age\", IntField = 22 }.Field : new { Value = 22 }.Value } }")]
        [BuildersMQL("{ \"$setOnInsert\" : { new { Field = \"Address\", IntField = 22 }.Field : new { Value = \"White House\" }.Value } }")]
        [BuildersMQL("{ \"$rename\" : { new { FieldToRename = \"Name\", IntField = 22 }.FieldToRename : new { newFieldName = \"FirstName\" }.newFieldName } }")]
        [BuildersMQL("{ \"$addToSet\" : { new { FieldUnset = \"IntList\", IntField = 22 }.FieldUnset : new { ValueToAdd = 11 }.ValueToAdd } }")]
        [BuildersMQL("{ \"$bit\" : { new { Field = \"Age\", IntField = 22 }.Field : { \"and\" : new { AgeValue = 22 }.AgeValue } } }")]
        [BuildersMQL("{ \"$inc\" : { new { Field = \"Height\", IntField = 22 }.Field : new { Value = 11 }.Value } }")]
        [BuildersMQL("{ \"$max\" : { new { Field = \"SiblingsCount\", IntField = 22 }.Field : new { Value = 22 }.Value } }")]
        [BuildersMQL("{ \"$mul\" : { new { Field = \"Age\", IntField = 22 }.Field : new { Value = 23 }.Value } }")]
        [BuildersMQL("{ \"$push\" : { new { Field = \"IntList\", IntField = 22 }.Field : new { Value = 22 }.Value } }")]
        [BuildersMQL("{ \"$pull\" : { new { Field = \"IntList\", IntField = 22 }.Field : new { Value = 22 }.Value } }")]
        public void Updates()
        {
            _ = Builders<User>.Update.Set(new { Field = "Age", IntField = 22 }.Field, new { Value = 22 }.Value);
            _ = Builders<User>.Update.SetOnInsert(new { Field = "Address", IntField = 22 }.Field, new { Value = "White House" }.Value);
            _ = Builders<User>.Update.Rename(new { FieldToRename = "Name", IntField = 22 }.FieldToRename, new { newFieldName = "FirstName" }.newFieldName);
            _ = Builders<ListsHolder>.Update.AddToSet(new { FieldUnset = "IntList", IntField = 22 }.FieldUnset, new { ValueToAdd = 11 }.ValueToAdd);
            _ = Builders<User>.Update.BitwiseAnd(new { Field = "Age", IntField = 22 }.Field, new { AgeValue = 22 }.AgeValue);
            _ = Builders<User>.Update.Inc(new { Field = "Height", IntField = 22 }.Field, new { Value = 11 }.Value);
            _ = Builders<Person>.Update.Max(new { Field = "SiblingsCount", IntField = 22 }.Field, new { Value = 22 }.Value);
            _ = Builders<User>.Update.Mul(new { Field = "Age", IntField = 22 }.Field, new { Value = 23 }.Value);
            _ = Builders<ListsHolder>.Update.Push(new { Field = "IntList", IntField = 22 }.Field, new { Value = 22 }.Value);
            _ = Builders<ListsHolder>.Update.Pull(new { Field = "IntList", IntField = 22 }.Field, new { Value = 22 }.Value);
        }
    }
}

