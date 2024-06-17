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
using MongoDB.Analyzer.Tests.Common.DataModel;
using MongoDB.Driver.Linq;

#nullable enable
#pragma warning disable CS8602 // Dereference of a possibly null reference.

namespace MongoDB.Analyzer.Tests.Common.TestCases.Linq
{
    public sealed class LinqNullables : TestCasesBase
    {
        [MQL("aggregate([{ \"$match\" : { \"Vehicle.VehicleType.Type\" : GetVehicleTypeEnum() } }])")]
        [MQL("aggregate([{ \"$match\" : { \"ShortNullable\" : GetNullableShort() } }])")]
        public void Method_with_nullable_return_type()
        {
            _ = GetMongoQueryable<Person>()
                .Where(p => p.Vehicle.VehicleType.Type == GetVehicleTypeEnum());

            _ = GetMongoQueryable<NullableHolder>()
                .Where(n => n.ShortNullable == GetNullableShort());
        }

        [MQL("aggregate([{ \"$match\" : { \"Vehicle.VehicleType.Type\" : GetVehicleTypeEnum() } }])")]
        [MQL("aggregate([{ \"$match\" : { \"ShortNullable\" : GetNullableShort() } }])")]
        public void Method_with_nullable_return_type_with_query_syntax()
        {
            _ = from person in GetMongoQueryable<Person?>()
                where person.Vehicle.VehicleType.Type == GetVehicleTypeEnum()
                select person;

            _ = from nullableHolder in GetMongoQueryable<NullableHolder?>()
                where nullableHolder.ShortNullable == GetNullableShort()
                select nullableHolder;
        }

        [MQL("aggregate([{ \"$match\" : { \"$or\" : [{ \"EnumInt8\" : nullableEnumInt8 }, { \"EnumUInt8\" : nullableEnumUInt8 }, { \"EnumInt16\" : nullableEnumInt16 }, { \"EnumUInt16\" : nullableEnumUInt16 }] } }, { \"$match\" : { \"$or\" : [{ \"EnumInt32\" : nullableEnumInt32 }, { \"EnumUInt32\" : nullableEnumUInt32 }, { \"EnumInt64\" : NumberLong(nullableEnumInt64) }, { \"EnumUInt64\" : NumberLong(nullableEnumUInt64) }] } }, { \"$project\" : { \"EnumInt16\" : \"$EnumInt16\", \"_id\" : nullableEnumInt8 } }])")]
        [MQL("aggregate([{ \"$match\" : { \"$or\" : [{ \"EnumInt8\" : null }, { \"EnumUInt8\" : null }, { \"EnumInt16\" : null }, { \"EnumUInt16\" : null }] } }, { \"$match\" : { \"$or\" : [{ \"EnumInt32\" : null }, { \"EnumUInt32\" : null }, { \"EnumInt64\" : null }, { \"EnumUInt64\" : null }] } }, { \"$project\" : { \"EnumInt16\" : \"$EnumInt16\", \"_id\" : 0 } }])")]
        [MQL("aggregate([{ \"$match\" : { \"$or\" : [{ \"EnumInt8\" : 0 }, { \"EnumUInt8\" : 0 }, { \"EnumInt16\" : 0 }, { \"EnumUInt16\" : 0 }] } }, { \"$match\" : { \"$or\" : [{ \"EnumInt32\" : 0 }, { \"EnumUInt32\" : 0 }, { \"EnumInt64\" : NumberLong(0) }, { \"EnumUInt64\" : NumberLong(0) }] } }, { \"$project\" : { \"EnumInt16\" : \"$EnumInt16\", \"_id\" : 0 } }])")]
        [MQL("aggregate([{ \"$match\" : { \"$or\" : [{ \"EnumInt8\" : 0 }, { \"EnumUInt8\" : 0 }, { \"EnumInt16\" : 0 }, { \"EnumUInt16\" : 0 }] } }, { \"$match\" : { \"$or\" : [{ \"EnumInt32\" : 0 }, { \"EnumUInt32\" : 0 }, { \"EnumInt64\" : NumberLong(0) }, { \"EnumUInt64\" : NumberLong(0) }] } }, { \"$project\" : { \"EnumInt16\" : \"$EnumInt16\", \"_id\" : 0 } }])")]
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

            _ = GetMongoQueryable<NullableEnumHolder>()
                .Where(n => n.EnumInt8 == nullableEnumInt8 || n.EnumUInt8 == nullableEnumUInt8 || n.EnumInt16 == nullableEnumInt16 || n.EnumUInt16 == nullableEnumUInt16)
                .Where(n => n.EnumInt32 == nullableEnumInt32 || n.EnumUInt32 == nullableEnumUInt32 || n.EnumInt64 == nullableEnumInt64 || n.EnumUInt64 == nullableEnumUInt64)
                .Select(n => n.EnumInt16);

            _ = GetMongoQueryable<NullableEnumHolder>()
                .Where(n => n.EnumInt8 == null || n.EnumUInt8 == null || n.EnumInt16 == null || n.EnumUInt16 == null)
                .Where(n => n.EnumInt32 == null || n.EnumUInt32 == null || n.EnumInt64 == null || n.EnumUInt64 == null)
                .Select(n => n.EnumInt16);

            _ = GetMongoQueryable<NullableEnumHolder>()
                .Where(n => n.EnumInt8 == EnumInt8.Value0 || n.EnumUInt8 == EnumUInt8.Value0 || n.EnumInt16 == EnumInt16.Value0 || n.EnumUInt16 == EnumUInt16.Value0)
                .Where(n => n.EnumInt32 == EnumInt32.Value0 || n.EnumUInt32 == EnumUInt32.Value0 || n.EnumInt64 == EnumInt64.Value0 || n.EnumUInt64 == EnumUInt64.Value0)
                .Select(n => n.EnumInt16);

            _ = GetMongoQueryable<EnumHolder>()
                .Where(e => e.EnumInt8 == EnumInt8.Value0 || e.EnumUInt8 == EnumUInt8.Value0 || e.EnumInt16 == EnumInt16.Value0 || e.EnumUInt16 == EnumUInt16.Value0)
                .Where(e => e.EnumInt32 == EnumInt32.Value0 || e.EnumUInt32 == EnumUInt32.Value0 || e.EnumInt64 == EnumInt64.Value0 || e.EnumUInt64 == EnumUInt64.Value0)
                .Select(e => e.EnumInt16);
        }

        [MQL("aggregate([{ \"$match\" : { \"$or\" : [{ \"EnumInt8\" : nullableEnumInt8 }, { \"EnumUInt8\" : nullableEnumUInt8 }, { \"EnumInt16\" : nullableEnumInt16 }, { \"EnumUInt16\" : nullableEnumUInt16 }] } }, { \"$match\" : { \"$or\" : [{ \"EnumInt32\" : nullableEnumInt32 }, { \"EnumUInt32\" : nullableEnumUInt32 }, { \"EnumInt64\" : NumberLong(nullableEnumInt64) }, { \"EnumUInt64\" : NumberLong(nullableEnumUInt64) }] } }, { \"$project\" : { \"EnumInt16\" : \"$EnumInt16\", \"_id\" : nullableEnumInt8 } }])")]
        [MQL("aggregate([{ \"$match\" : { \"$or\" : [{ \"EnumInt8\" : null }, { \"EnumUInt8\" : null }, { \"EnumInt16\" : null }, { \"EnumUInt16\" : null }] } }, { \"$match\" : { \"$or\" : [{ \"EnumInt32\" : null }, { \"EnumUInt32\" : null }, { \"EnumInt64\" : null }, { \"EnumUInt64\" : null }] } }, { \"$project\" : { \"EnumInt16\" : \"$EnumInt16\", \"_id\" : 0 } }])")]
        [MQL("aggregate([{ \"$match\" : { \"$or\" : [{ \"EnumInt8\" : 0 }, { \"EnumUInt8\" : 0 }, { \"EnumInt16\" : 0 }, { \"EnumUInt16\" : 0 }] } }, { \"$match\" : { \"$or\" : [{ \"EnumInt32\" : 0 }, { \"EnumUInt32\" : 0 }, { \"EnumInt64\" : NumberLong(0) }, { \"EnumUInt64\" : NumberLong(0) }] } }, { \"$project\" : { \"EnumInt16\" : \"$EnumInt16\", \"_id\" : 0 } }])")]
        [MQL("aggregate([{ \"$match\" : { \"$or\" : [{ \"EnumInt8\" : 0 }, { \"EnumUInt8\" : 0 }, { \"EnumInt16\" : 0 }, { \"EnumUInt16\" : 0 }] } }, { \"$match\" : { \"$or\" : [{ \"EnumInt32\" : 0 }, { \"EnumUInt32\" : 0 }, { \"EnumInt64\" : NumberLong(0) }, { \"EnumUInt64\" : NumberLong(0) }] } }, { \"$project\" : { \"EnumInt16\" : \"$EnumInt16\", \"_id\" : 0 } }])")]
        public void Nullables_enum_with_query_syntax()
        {
            EnumInt8? nullableEnumInt8 = null;
            EnumUInt8? nullableEnumUInt8 = null;
            EnumInt16? nullableEnumInt16 = null;
            EnumUInt16? nullableEnumUInt16 = null;
            EnumInt32? nullableEnumInt32 = null;
            EnumUInt32? nullableEnumUInt32 = null;
            EnumInt64? nullableEnumInt64 = null;
            EnumUInt64? nullableEnumUInt64 = null;

            _ = from nullableEnumHolder in GetMongoQueryable<NullableEnumHolder>()
                where nullableEnumHolder.EnumInt8 == nullableEnumInt8 || nullableEnumHolder.EnumUInt8 == nullableEnumUInt8 || nullableEnumHolder.EnumInt16 == nullableEnumInt16 || nullableEnumHolder.EnumUInt16 == nullableEnumUInt16
                where nullableEnumHolder.EnumInt32 == nullableEnumInt32 || nullableEnumHolder.EnumUInt32 == nullableEnumUInt32 || nullableEnumHolder.EnumInt64 == nullableEnumInt64 || nullableEnumHolder.EnumUInt64 == nullableEnumUInt64
                select nullableEnumHolder.EnumInt16;

            _ = from nullableEnumHolder in GetMongoQueryable<NullableEnumHolder>()
                where nullableEnumHolder.EnumInt8 == null || nullableEnumHolder.EnumUInt8 == null || nullableEnumHolder.EnumInt16 == null || nullableEnumHolder.EnumUInt16 == null
                where nullableEnumHolder.EnumInt32 == null || nullableEnumHolder.EnumUInt32 == null || nullableEnumHolder.EnumInt64 == null || nullableEnumHolder.EnumUInt64 == null
                select nullableEnumHolder.EnumInt16;

            _ = from nullableEnumHolder in GetMongoQueryable<NullableEnumHolder>()
                where nullableEnumHolder.EnumInt8 == EnumInt8.Value0 || nullableEnumHolder.EnumUInt8 == EnumUInt8.Value0 || nullableEnumHolder.EnumInt16 == EnumInt16.Value0 || nullableEnumHolder.EnumUInt16 == EnumUInt16.Value0
                where nullableEnumHolder.EnumInt32 == EnumInt32.Value0 || nullableEnumHolder.EnumUInt32 == EnumUInt32.Value0 || nullableEnumHolder.EnumInt64 == EnumInt64.Value0 || nullableEnumHolder.EnumUInt64 == EnumUInt64.Value0
                select nullableEnumHolder.EnumInt16;

            _ = from enumHolder in GetMongoQueryable<EnumHolder>()
                where enumHolder.EnumInt8 == EnumInt8.Value0 || enumHolder.EnumUInt8 == EnumUInt8.Value0 || enumHolder.EnumInt16 == EnumInt16.Value0 || enumHolder.EnumUInt16 == EnumUInt16.Value0
                where enumHolder.EnumInt32 == EnumInt32.Value0 || enumHolder.EnumUInt32 == EnumUInt32.Value0 || enumHolder.EnumInt64 == EnumInt64.Value0 || enumHolder.EnumUInt64 == EnumUInt64.Value0
                select enumHolder.EnumInt16;
        }

        [MQL("aggregate([{ \"$project\" : { \"Name\" : \"$Name\", \"City\" : \"$Address.City\", \"_id\" : 0 } }])")]
        public void Nullable_mongo_queryable()
        {
            _ = GetMongoQueryable<Person?>()
                .Select(p => new { p.Name, p.Address.City });
        }

        [MQL("aggregate([{ \"$project\" : { \"Name\" : \"$Name\", \"City\" : \"$Address.City\", \"_id\" : 0 } }])")]
        public void Nullable_mongo_queryable_with_query_syntax()
        {
            _ = from person in GetMongoQueryable<Person?>()
                select new { person.Name, person.Address.City };
        }

        [MQL("aggregate([{ \"$match\" : { \"ByteNullable\" : StaticHolder.ReadonlyByteNullable } }, { \"$match\" : { \"DoubleNullable\" : StaticHolder.ReadonlyDoubleNullable } }, { \"$match\" : { \"IntNullable\" : StaticHolder.ReadonlyIntNullable } }, { \"$match\" : { \"LongNullable\" : NumberLong(StaticHolder.ReadonlyLongNullable) } }, { \"$match\" : { \"ShortNullable\" : StaticHolder.ReadonlyShortNullable } }, { \"$match\" : { \"StringNullable\" : StaticHolder.ReadonlyStringNullable } }])")]
        [MQL("aggregate([{ \"$match\" : { \"ByteNullable\" : null } }, { \"$match\" : { \"DoubleNullable\" : null } }, { \"$match\" : { \"IntNullable\" : null } }, { \"$match\" : { \"LongNullable\" : null } }, { \"$match\" : { \"ShortNullable\" : null } }, { \"$match\" : { \"StringNullable\" : null } }])")]
        [MQL("aggregate([{ \"$match\" : { \"ByteNullable\" : 1 } }, { \"$match\" : { \"DoubleNullable\" : 4.5 } }, { \"$match\" : { \"IntNullable\" : 21 } }, { \"$match\" : { \"LongNullable\" : NumberLong(22) } }, { \"$match\" : { \"ShortNullable\" : 2 } }, { \"$match\" : { \"StringNullable\" : \"String\" } }])")]
        [MQL("aggregate([{ \"$match\" : { \"ByteNullable\" : 1 } }, { \"$match\" : { \"DoubleNullable\" : 4.5 } }, { \"$match\" : { \"IntNullable\" : 21 } }, { \"$match\" : { \"LongNullable\" : NumberLong(22) } }, { \"$match\" : { \"ShortNullable\" : 2 } }, { \"$match\" : { \"StringNullable\" : \"String\" } }])")]
        [MQL("aggregate([{ \"$match\" : { \"ByteNullable\" : 1 } }, { \"$match\" : { \"DoubleNullable\" : 4.5 } }, { \"$match\" : { \"IntNullable\" : 21 } }, { \"$match\" : { \"LongNullable\" : NumberLong(22) } }, { \"$match\" : { \"ShortNullable\" : 2 } }, { \"$match\" : { \"StringNullable\" : \"String\" } }])")]
        [MQL("aggregate([{ \"$match\" : { \"SiblingsCount\" : StaticHolder.ReadonlyIntNullable } }, { \"$match\" : { \"TicksSinceBirth\" : NumberLong(StaticHolder.ReadonlyLongNullable) } }, { \"$match\" : { \"Name\" : StaticHolder.ReadonlyStringNullable } }, { \"$match\" : { \"Address.City\" : StaticHolder.ReadonlyStringNullable } }])")]
        [MQL("aggregate([{ \"$match\" : { \"SiblingsCount\" : 1 } }, { \"$match\" : { \"TicksSinceBirth\" : NumberLong(2) } }, { \"$match\" : { \"Name\" : \"Name\" } }, { \"$match\" : { \"Address.City\" : \"City\" } }])")]
        public void Nullable_primitive_types()
        {
            _ = GetMongoQueryable<NullableHolder?>()
                .Where(n => n.ByteNullable == StaticHolder.ReadonlyByteNullable)
                .Where(n => n.DoubleNullable == StaticHolder.ReadonlyDoubleNullable)
                .Where(n => n.IntNullable == StaticHolder.ReadonlyIntNullable)
                .Where(n => n.LongNullable == StaticHolder.ReadonlyLongNullable)
                .Where(n => n.ShortNullable == StaticHolder.ReadonlyShortNullable)
                .Where(n => n.StringNullable == StaticHolder.ReadonlyStringNullable);

            _ = GetMongoQueryable<NullableHolder?>()
                .Where(n => n.ByteNullable == null)
                .Where(n => n.DoubleNullable == null)
                .Where(n => n.IntNullable == null)
                .Where(n => n.LongNullable == null)
                .Where(n => n.ShortNullable == null)
                .Where(n => n.StringNullable == null);

            _ = GetMongoQueryable<NullableHolder?>()
                .Where(n => n.ByteNullable == (byte)1)
                .Where(n => n.DoubleNullable == (double)4.5)
                .Where(n => n.IntNullable == (int)21)
                .Where(n => n.LongNullable == (long)22)
                .Where(n => n.ShortNullable == (short)2)
                .Where(n => n.StringNullable == (string)"String");

            _ = GetMongoQueryable<NullableHolder?>()
                .Where(n => n.ByteNullable == (byte?)1)
                .Where(n => n.DoubleNullable == (double?)4.5)
                .Where(n => n.IntNullable == (int?)21)
                .Where(n => n.LongNullable == (long?)22)
                .Where(n => n.ShortNullable == (short?)2)
                .Where(n => n.StringNullable == (string?)"String");

            _ = GetMongoQueryable<NullableHolder?>()
                .Where(n => n.ByteNullable == (byte?)1)
                .Where(n => n.DoubleNullable == 4.5)
                .Where(n => n.IntNullable == 21)
                .Where(n => n.LongNullable == (long?)22)
                .Where(n => n.ShortNullable == (short?)2)
                .Where(n => n.StringNullable == "String");

            _ = GetMongoQueryable<Person?>()
                .Where(p => p.SiblingsCount == StaticHolder.ReadonlyIntNullable)
                .Where(p => p.TicksSinceBirth == StaticHolder.ReadonlyLongNullable)
                .Where(p => p.Name == StaticHolder.ReadonlyStringNullable)
                .Where(p => p.Address.City == StaticHolder.ReadonlyStringNullable);

            _ = GetMongoQueryable<Person?>()
                .Where(p => p.SiblingsCount == (int?)1)
                .Where(p => p.TicksSinceBirth == (long?)2)
                .Where(p => p.Name == (string?)"Name")
                .Where(p => p.Address.City == (string?)"City");
        }

        [MQL("aggregate([{ \"$match\" : { \"ByteNullable\" : StaticHolder.ReadonlyByteNullable } }, { \"$match\" : { \"DoubleNullable\" : StaticHolder.ReadonlyDoubleNullable } }, { \"$match\" : { \"IntNullable\" : StaticHolder.ReadonlyIntNullable } }, { \"$match\" : { \"LongNullable\" : NumberLong(StaticHolder.ReadonlyLongNullable) } }, { \"$match\" : { \"ShortNullable\" : StaticHolder.ReadonlyShortNullable } }, { \"$match\" : { \"StringNullable\" : StaticHolder.ReadonlyStringNullable } }])")]
        [MQL("aggregate([{ \"$match\" : { \"ByteNullable\" : null } }, { \"$match\" : { \"DoubleNullable\" : null } }, { \"$match\" : { \"IntNullable\" : null } }, { \"$match\" : { \"LongNullable\" : null } }, { \"$match\" : { \"ShortNullable\" : null } }, { \"$match\" : { \"StringNullable\" : null } }])")]
        [MQL("aggregate([{ \"$match\" : { \"ByteNullable\" : 1 } }, { \"$match\" : { \"DoubleNullable\" : 4.5 } }, { \"$match\" : { \"IntNullable\" : 21 } }, { \"$match\" : { \"LongNullable\" : NumberLong(22) } }, { \"$match\" : { \"ShortNullable\" : 2 } }, { \"$match\" : { \"StringNullable\" : \"String\" } }])")]
        [MQL("aggregate([{ \"$match\" : { \"ByteNullable\" : 1 } }, { \"$match\" : { \"DoubleNullable\" : 4.5 } }, { \"$match\" : { \"IntNullable\" : 21 } }, { \"$match\" : { \"LongNullable\" : NumberLong(22) } }, { \"$match\" : { \"ShortNullable\" : 2 } }, { \"$match\" : { \"StringNullable\" : \"String\" } }])")]
        [MQL("aggregate([{ \"$match\" : { \"ByteNullable\" : 1 } }, { \"$match\" : { \"DoubleNullable\" : 4.5 } }, { \"$match\" : { \"IntNullable\" : 21 } }, { \"$match\" : { \"LongNullable\" : NumberLong(22) } }, { \"$match\" : { \"ShortNullable\" : 2 } }, { \"$match\" : { \"StringNullable\" : \"String\" } }])")]
        [MQL("aggregate([{ \"$match\" : { \"SiblingsCount\" : StaticHolder.ReadonlyIntNullable } }, { \"$match\" : { \"TicksSinceBirth\" : NumberLong(StaticHolder.ReadonlyLongNullable) } }, { \"$match\" : { \"Name\" : StaticHolder.ReadonlyStringNullable } }, { \"$match\" : { \"Address.City\" : StaticHolder.ReadonlyStringNullable } }])")]
        [MQL("aggregate([{ \"$match\" : { \"SiblingsCount\" : 1 } }, { \"$match\" : { \"TicksSinceBirth\" : NumberLong(2) } }, { \"$match\" : { \"Name\" : \"Name\" } }, { \"$match\" : { \"Address.City\" : \"City\" } }])")]
        public void Nullable_primitive_types_with_query_syntax()
        {
            _ = from nullableHolder in GetMongoQueryable<NullableHolder?>()
                where nullableHolder.ByteNullable == StaticHolder.ReadonlyByteNullable
                where nullableHolder.DoubleNullable == StaticHolder.ReadonlyDoubleNullable
                where nullableHolder.IntNullable == StaticHolder.ReadonlyIntNullable
                where nullableHolder.LongNullable == StaticHolder.ReadonlyLongNullable
                where nullableHolder.ShortNullable == StaticHolder.ReadonlyShortNullable
                where nullableHolder.StringNullable == StaticHolder.ReadonlyStringNullable
                select nullableHolder;

            _ = from nullableHolder in GetMongoQueryable<NullableHolder?>()
                where nullableHolder.ByteNullable == null
                where nullableHolder.DoubleNullable == null
                where nullableHolder.IntNullable == null
                where nullableHolder.LongNullable == null
                where nullableHolder.ShortNullable == null
                where nullableHolder.StringNullable == null
                select nullableHolder;

            _ = from nullableHolder in GetMongoQueryable<NullableHolder?>()
                where nullableHolder.ByteNullable == (byte)1
                where nullableHolder.DoubleNullable == (double)4.5
                where nullableHolder.IntNullable == (int)21
                where nullableHolder.LongNullable == (long)22
                where nullableHolder.ShortNullable == (short)2
                where nullableHolder.StringNullable == (string)"String"
                select nullableHolder;

            _ = from nullableHolder in GetMongoQueryable<NullableHolder?>()
                where nullableHolder.ByteNullable == (byte?)1
                where nullableHolder.DoubleNullable == (double?)4.5
                where nullableHolder.IntNullable == (int?)21
                where nullableHolder.LongNullable == (long?)22
                where nullableHolder.ShortNullable == (short?)2
                where nullableHolder.StringNullable == (string?)"String"
                select nullableHolder;

            _ = from nullableHolder in GetMongoQueryable<NullableHolder?>()
                where nullableHolder.ByteNullable == (byte?)1
                where nullableHolder.DoubleNullable == 4.5
                where nullableHolder.IntNullable == 21
                where nullableHolder.LongNullable == (long?)22
                where nullableHolder.ShortNullable == (short?)2
                where nullableHolder.StringNullable == "String"
                select nullableHolder;

            _ = from person in GetMongoQueryable<Person?>()
                where person.SiblingsCount == StaticHolder.ReadonlyIntNullable
                where person.TicksSinceBirth == StaticHolder.ReadonlyLongNullable
                where person.Name == StaticHolder.ReadonlyStringNullable
                where person.Address.City == StaticHolder.ReadonlyStringNullable
                select person;

            _ = from person in GetMongoQueryable<Person?>()
                where person.SiblingsCount == (int?)1
                where person.TicksSinceBirth == (long?)2
                where person.Name == (string?)"Name"
                where person.Address.City == (string?)"City"
                select person;
        }

        private VehicleTypeEnum? GetVehicleTypeEnum() => VehicleTypeEnum.Bus;
        private short? GetNullableShort() => 10;
    }
}

#nullable disable
#pragma warning restore CS8602 // Dereference of a possibly null reference.
