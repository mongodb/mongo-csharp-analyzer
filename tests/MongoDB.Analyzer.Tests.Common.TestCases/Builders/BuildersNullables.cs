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

#nullable enable
#pragma warning disable CS8602 // Dereference of a possibly null reference.

namespace MongoDB.Analyzer.Tests.Common.TestCases.Builders
{
    public sealed class BuildersNullables : TestCasesBase
    {
        [BuildersMQL("{ \"Vehicle.VehicleType.Type\" : GetVehicleTypeEnum() }")]
        [BuildersMQL("{ \"ShortNullable\" : GetNullableShort() }")]
        public void Method_with_nullable_return_type()
        {
            _ = Builders<Person?>.Filter.Eq(p => p.Vehicle.VehicleType.Type, GetVehicleTypeEnum());
            _ = Builders<NullableHolder?>.Filter.Eq(n => n.ShortNullable, GetNullableShort());
        }

        [BuildersMQL("{ \"$or\" : [{ \"EnumInt8\" : nullableEnumInt8 }, { \"EnumUInt8\" : nullableEnumUInt8 }, { \"EnumInt16\" : nullableEnumInt16 }, { \"EnumUInt16\" : nullableEnumUInt16 }, { \"EnumInt32\" : nullableEnumInt32 }, { \"EnumUInt32\" : nullableEnumUInt32 }, { \"EnumInt64\" : NumberLong(nullableEnumInt64) }, { \"EnumUInt64\" : NumberLong(nullableEnumUInt64) }] }")]
        [BuildersMQL("{ \"$or\" : [{ \"EnumInt8\" : null }, { \"EnumUInt8\" : null }, { \"EnumInt16\" : null }, { \"EnumUInt16\" : null }, { \"EnumInt32\" : null }, { \"EnumUInt32\" : null }, { \"EnumInt64\" : null }, { \"EnumUInt64\" : null }] }")]
        [BuildersMQL("{ \"$or\" : [{ \"EnumInt8\" : 0 }, { \"EnumUInt8\" : 0 }, { \"EnumInt16\" : 0 }, { \"EnumUInt16\" : 0 }, { \"EnumInt32\" : 0 }, { \"EnumUInt32\" : 0 }, { \"EnumInt64\" : NumberLong(0) }, { \"EnumUInt64\" : NumberLong(0) }] }")]
        [BuildersMQL("{ \"$or\" : [{ \"EnumInt8\" : nullableEnumInt8 }, { \"EnumUInt8\" : nullableEnumUInt8 }, { \"EnumInt16\" : nullableEnumInt16 }, { \"EnumUInt16\" : nullableEnumUInt16 }, { \"EnumInt32\" : nullableEnumInt32 }, { \"EnumUInt32\" : nullableEnumUInt32 }, { \"EnumInt64\" : NumberLong(nullableEnumInt64) }, { \"EnumUInt64\" : NumberLong(nullableEnumUInt64) }] }")]
        public void Nullables_enum()
        {
            EnumInt8? nullableEnumInt8 = null;
            EnumUInt8? nullableEnumUInt8 = null;
            EnumInt16? nullableEnumInt16 = null;
            EnumUInt16? nullableEnumUInt16 = null;
            EnumInt32? nullableEnumInt32 = null;
            EnumUInt32? nullableEnumUInt32 = null;
            EnumInt64? nullableEnumInt64 = null;
            EnumUInt64? nullableEnumUInt64 = null;

            _ = Builders<NullableEnumHolder>.Filter.Eq(n => n.EnumInt8, nullableEnumInt8) |
                Builders<NullableEnumHolder>.Filter.Eq(n => n.EnumUInt8, nullableEnumUInt8) |
                Builders<NullableEnumHolder>.Filter.Eq(n => n.EnumInt16, nullableEnumInt16) |
                Builders<NullableEnumHolder>.Filter.Eq(n => n.EnumUInt16, nullableEnumUInt16) |
                Builders<NullableEnumHolder>.Filter.Eq(n => n.EnumInt32, nullableEnumInt32) |
                Builders<NullableEnumHolder>.Filter.Eq(n => n.EnumUInt32, nullableEnumUInt32) |
                Builders<NullableEnumHolder>.Filter.Eq(n => n.EnumInt64, nullableEnumInt64) |
                Builders<NullableEnumHolder>.Filter.Eq(n => n.EnumUInt64, nullableEnumUInt64);

            _ = Builders<NullableEnumHolder>.Filter.Eq(n => n.EnumInt8, null) |
                Builders<NullableEnumHolder>.Filter.Eq(n => n.EnumUInt8, null) |
                Builders<NullableEnumHolder>.Filter.Eq(n => n.EnumInt16, null) |
                Builders<NullableEnumHolder>.Filter.Eq(n => n.EnumUInt16, null) |
                Builders<NullableEnumHolder>.Filter.Eq(n => n.EnumInt32, null) |
                Builders<NullableEnumHolder>.Filter.Eq(n => n.EnumUInt32, null) |
                Builders<NullableEnumHolder>.Filter.Eq(n => n.EnumInt64, null) |
                Builders<NullableEnumHolder>.Filter.Eq(n => n.EnumUInt64, null);

            _ = Builders<NullableEnumHolder>.Filter.Eq(n => n.EnumInt8, EnumInt8.Value0) |
                Builders<NullableEnumHolder>.Filter.Eq(n => n.EnumUInt8, EnumUInt8.Value0) |
                Builders<NullableEnumHolder>.Filter.Eq(n => n.EnumInt16, EnumInt16.Value0) |
                Builders<NullableEnumHolder>.Filter.Eq(n => n.EnumUInt16, EnumUInt16.Value0) |
                Builders<NullableEnumHolder>.Filter.Eq(n => n.EnumInt32, EnumInt32.Value0) |
                Builders<NullableEnumHolder>.Filter.Eq(n => n.EnumUInt32, EnumUInt32.Value0) |
                Builders<NullableEnumHolder>.Filter.Eq(n => n.EnumInt64, EnumInt64.Value0) |
                Builders<NullableEnumHolder>.Filter.Eq(n => n.EnumUInt64, EnumUInt64.Value0);

            _ = Builders<EnumHolder>.Filter.Eq(e => e.EnumInt8, nullableEnumInt8) |
                Builders<EnumHolder>.Filter.Eq(e => e.EnumUInt8, nullableEnumUInt8) |
                Builders<EnumHolder>.Filter.Eq(e => e.EnumInt16, nullableEnumInt16) |
                Builders<EnumHolder>.Filter.Eq(e => e.EnumUInt16, nullableEnumUInt16) |
                Builders<EnumHolder>.Filter.Eq(e => e.EnumInt32, nullableEnumInt32) |
                Builders<EnumHolder>.Filter.Eq(e => e.EnumUInt32, nullableEnumUInt32) |
                Builders<EnumHolder>.Filter.Eq(e => e.EnumInt64, nullableEnumInt64) |
                Builders<EnumHolder>.Filter.Eq(e => e.EnumUInt64, nullableEnumUInt64);
        }

        [BuildersMQL("{ \"Address.City\" : \"New York City\" }")]
        public void Nullable_mongo_queryable()
        {
            _ = Builders<Person?>.Filter.Eq(p => p.Address.City, "New York City");
        }

        [BuildersMQL("{ \"$or\" : [{ \"ByteNullable\" : StaticHolder.ReadonlyByteNullable }, { \"DoubleNullable\" : StaticHolder.ReadonlyDoubleNullable }, { \"IntNullable\" : StaticHolder.ReadonlyIntNullable }, { \"LongNullable\" : NumberLong(StaticHolder.ReadonlyLongNullable) }, { \"ShortNullable\" : StaticHolder.ReadonlyShortNullable }, { \"StringNullable\" : StaticHolder.ReadonlyStringNullable }] }")]
        [BuildersMQL("{ \"$or\" : [{ \"ByteNullable\" : null }, { \"DoubleNullable\" : null }, { \"IntNullable\" : null }, { \"LongNullable\" : null }, { \"ShortNullable\" : null }, { \"StringNullable\" : null }] }")]
        [BuildersMQL("{ \"$or\" : [{ \"ByteNullable\" : 1 }, { \"DoubleNullable\" : 4.5 }, { \"IntNullable\" : 21 }, { \"LongNullable\" : NumberLong(22) }, { \"ShortNullable\" : 2 }, { \"StringNullable\" : \"String\" }] }")]
        [BuildersMQL("{ \"$or\" : [{ \"ByteNullable\" : 1 }, { \"DoubleNullable\" : 4.5 }, { \"IntNullable\" : 21 }, { \"LongNullable\" : NumberLong(22) }, { \"ShortNullable\" : 2 }, { \"StringNullable\" : \"String\" }] }")]
        [BuildersMQL("{ \"$or\" : [{ \"ByteNullable\" : 1 }, { \"DoubleNullable\" : 4.5 }, { \"IntNullable\" : 21 }, { \"LongNullable\" : NumberLong(22) }, { \"ShortNullable\" : 2 }, { \"StringNullable\" : \"String\" }] }")]
        [BuildersMQL("{ \"$or\" : [{ \"SiblingsCount\" : StaticHolder.ReadonlyIntNullable }, { \"TicksSinceBirth\" : NumberLong(StaticHolder.ReadonlyLongNullable) }, { \"Name\" : StaticHolder.ReadonlyStringNullable }, { \"Address.City\" : StaticHolder.ReadonlyStringNullable }] }")]
        [BuildersMQL("{ \"$or\" : [{ \"SiblingsCount\" : 1 }, { \"TicksSinceBirth\" : NumberLong(2) }, { \"Name\" : \"Name\" }, { \"Address.City\" : \"City\" }] }")]
        public void Nullable_primitive_types()
        {
            _ = Builders<NullableHolder>.Filter.Eq(n => n.ByteNullable, StaticHolder.ReadonlyByteNullable) |
                Builders<NullableHolder>.Filter.Eq(n => n.DoubleNullable, StaticHolder.ReadonlyDoubleNullable) |
                Builders<NullableHolder>.Filter.Eq(n => n.IntNullable, StaticHolder.ReadonlyIntNullable) |
                Builders<NullableHolder>.Filter.Eq(n => n.LongNullable, StaticHolder.ReadonlyLongNullable) |
                Builders<NullableHolder>.Filter.Eq(n => n.ShortNullable, StaticHolder.ReadonlyShortNullable) |
                Builders<NullableHolder>.Filter.Eq(n => n.StringNullable, StaticHolder.ReadonlyStringNullable);

            _ = Builders<NullableHolder>.Filter.Eq(n => n.ByteNullable, null) |
                Builders<NullableHolder>.Filter.Eq(n => n.DoubleNullable, null) |
                Builders<NullableHolder>.Filter.Eq(n => n.IntNullable, null) |
                Builders<NullableHolder>.Filter.Eq(n => n.LongNullable, null) |
                Builders<NullableHolder>.Filter.Eq(n => n.ShortNullable, null) |
                Builders<NullableHolder>.Filter.Eq(n => n.StringNullable, null);

            _ = Builders<NullableHolder>.Filter.Eq(n => n.ByteNullable, (byte)1) |
                Builders<NullableHolder>.Filter.Eq(n => n.DoubleNullable, (double)4.5) |
                Builders<NullableHolder>.Filter.Eq(n => n.IntNullable, (int)21) |
                Builders<NullableHolder>.Filter.Eq(n => n.LongNullable, (long)22) |
                Builders<NullableHolder>.Filter.Eq(n => n.ShortNullable, (short)2) |
                Builders<NullableHolder>.Filter.Eq(n => n.StringNullable, (string)"String");

            _ = Builders<NullableHolder>.Filter.Eq(n => n.ByteNullable, (byte?)1) |
                Builders<NullableHolder>.Filter.Eq(n => n.DoubleNullable, (double?)4.5) |
                Builders<NullableHolder>.Filter.Eq(n => n.IntNullable, (int?)21) |
                Builders<NullableHolder>.Filter.Eq(n => n.LongNullable, (long?)22) |
                Builders<NullableHolder>.Filter.Eq(n => n.ShortNullable, (short?)2) |
                Builders<NullableHolder>.Filter.Eq(n => n.StringNullable, (string?)"String");

            _ = Builders<NullableHolder>.Filter.Eq(n => n.ByteNullable, (byte?)1) |
                Builders<NullableHolder>.Filter.Eq(n => n.DoubleNullable, 4.5) |
                Builders<NullableHolder>.Filter.Eq(n => n.IntNullable, 21) |
                Builders<NullableHolder>.Filter.Eq(n => n.LongNullable, (long?)22) |
                Builders<NullableHolder>.Filter.Eq(n => n.ShortNullable, (short?)2) |
                Builders<NullableHolder>.Filter.Eq(n => n.StringNullable, "String");

            _ = Builders<Person>.Filter.Eq(p => p.SiblingsCount, StaticHolder.ReadonlyIntNullable) |
                Builders<Person>.Filter.Eq(p => p.TicksSinceBirth, StaticHolder.ReadonlyLongNullable) |
                Builders<Person>.Filter.Eq(p => p.Name, StaticHolder.ReadonlyStringNullable) |
                Builders<Person>.Filter.Eq(p => p.Address.City, StaticHolder.ReadonlyStringNullable);

            _ = Builders<Person>.Filter.Eq(p => p.SiblingsCount, (int?)1) |
                Builders<Person>.Filter.Eq(p => p.TicksSinceBirth, (long?)2) |
                Builders<Person>.Filter.Eq(p => p.Name, (string?)"Name") |
                Builders<Person>.Filter.Eq(p => p.Address.City, (string?)"City");
        }

        private VehicleTypeEnum? GetVehicleTypeEnum() => VehicleTypeEnum.Bus;
        private short? GetNullableShort() => 10;
    }
}
#nullable disable
#pragma warning restore CS8602 // Dereference of a possibly null reference.s
