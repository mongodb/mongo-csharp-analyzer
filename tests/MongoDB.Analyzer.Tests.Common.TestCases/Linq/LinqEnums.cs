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
using System.Linq;
using MongoDB.Analyzer.Tests.Common.DataModel;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace MongoDB.Analyzer.Tests.Common.TestCases.Linq
{
    public sealed class LinqEnums : TestCasesBase
    {
        [MQL("aggregate([{ \"$match\" : { \"$or\" : [{ \"EnumInt8\" : 0 }, { \"EnumInt8\" : 9 }, { \"EnumInt8\" : 127 }] } }])")]
        public void Enum_int8()
        {
            _ = GetMongoQueryable<EnumHolder>()
                .Where(u => u.EnumInt8 == EnumInt8.Value0 || u.EnumInt8 == EnumInt8.Value9 || u.EnumInt8 == EnumInt8.MaxValue);
        }

        [MQL("aggregate([{ \"$match\" : { \"$or\" : [{ \"EnumUInt8\" : 0 }, { \"EnumUInt8\" : 9 }, { \"EnumUInt8\" : 255 }] } }])")]
        public void Enum_uint8()
        {
            _ = GetMongoQueryable<EnumHolder>()
                .Where(u => u.EnumUInt8 == EnumUInt8.Value0 || u.EnumUInt8 == EnumUInt8.Value9 || u.EnumUInt8 == EnumUInt8.MaxValue);
        }

        [MQL("aggregate([{ \"$match\" : { \"$or\" : [{ \"EnumInt16\" : 0 }, { \"EnumInt16\" : 999 }, { \"EnumInt16\" : 32767 }] } }])")]
        public void Enum_int16()
        {
            _ = GetMongoQueryable<EnumHolder>()
                .Where(u => u.EnumInt16 == EnumInt16.Value0 || u.EnumInt16 == EnumInt16.Value999 || u.EnumInt16 == EnumInt16.MaxValue);
        }

        [MQL("aggregate([{ \"$match\" : { \"$or\" : [{ \"EnumUInt16\" : 0 }, { \"EnumUInt16\" : 999 }, { \"EnumUInt16\" : 65535 }] } }])")]
        public void Enum_uint16()
        {
            _ = GetMongoQueryable<EnumHolder>()
                .Where(u => u.EnumUInt16 == EnumUInt16.Value0 || u.EnumUInt16 == EnumUInt16.Value999 || u.EnumUInt16 == EnumUInt16.MaxValue);
        }

        [MQL("aggregate([{ \"$match\" : { \"$or\" : [{ \"EnumInt32\" : 0 }, { \"EnumInt32\" : 999 }, { \"EnumInt32\" : 2147483647 }] } }])")]
        public void Enum_int32()
        {
            _ = GetMongoQueryable<EnumHolder>()
                .Where(u => u.EnumInt32 == EnumInt32.Value0 || u.EnumInt32 == EnumInt32.Value999 || u.EnumInt32 == EnumInt32.MaxValue);
        }

        [MQL("aggregate([{ \"$match\" : { \"$or\" : [{ \"EnumUInt32\" : 0 }, { \"EnumUInt32\" : 999 }, { \"EnumUInt32\" : -1 }] } }])")]
        public void Enum_uint32()
        {
            _ = GetMongoQueryable<EnumHolder>()
                .Where(u => u.EnumUInt32 == EnumUInt32.Value0 || u.EnumUInt32 == EnumUInt32.Value999 || u.EnumUInt32 == EnumUInt32.MaxValue);
        }

        [MQL("aggregate([{ \"$match\" : { \"$or\" : [{ \"EnumUInt64\" : NumberLong(0) }, { \"EnumUInt64\" : NumberLong(999) }, { \"EnumUInt64\" : NumberLong(-1) }] } }])")]
        public void Enum_uint64()
        {
            _ = GetMongoQueryable<EnumHolder>()
                .Where(u => u.EnumUInt64 == EnumUInt64.Value0 || u.EnumUInt64 == EnumUInt64.Value999 || u.EnumUInt64 == EnumUInt64.MaxValue);
        }

        [MQL("aggregate([{ \"$match\" : { \"$or\" : [{ \"EnumInt64\" : NumberLong(0) }, { \"EnumInt64\" : NumberLong(999) }, { \"EnumInt64\" : NumberLong(\"9223372036854775807\") }] } }])")]
        public void Enum_int64()
        {
            _ = GetMongoQueryable<EnumHolder>()
                .Where(u => u.EnumInt64 == EnumInt64.Value0 || u.EnumInt64 == EnumInt64.Value999 || u.EnumInt64 == EnumInt64.MaxValue);
        }

        [MQL("aggregate([{ \"$match\" : { \"$or\" : [{ \"Vehicle.VehicleType.Type\" : 0 }, { \"Vehicle.VehicleType.Type\" : 2 }] } }, { \"$project\" : { \"LicenceNumber\" : \"$Vehicle.LicenceNumber\", \"_id\" : 0 } }])")]
        public void Enum_not_typed_in_nested_class()
        {
            _ = GetMongoQueryable<Person>()
                .Where(u => u.Vehicle.VehicleType.Type == VehicleTypeEnum.Bus || u.Vehicle.VehicleType.Type == VehicleTypeEnum.Motorcylce)
                .Select(u => u.Vehicle.LicenceNumber);
        }

        [MQL("aggregate([{ \"$match\" : { \"MPG\" : { \"$gt\" : 20.0 } } }, { \"$project\" : { \"Item1\" : \"$VehicleMake\", \"Item2\" : \"$Type\", \"_id\" : 0 } }])")]
        public void Enum_in_type_argument_1()
        {
            _ = GetMongoCollection<VehicleType>().AsQueryable()
                .Where(v => v.MPG > 20)
                .Select(v => new Tuple<VehicleMake, VehicleTypeEnum>(v.VehicleMake, v.Type));
        }

        [MQL("aggregate([{ \"$match\" : { \"MPG\" : { \"$gt\" : 20.0 } } }, { \"$project\" : { \"Item1\" : \"$VehicleMake\", \"Item2\" : \"$Type\", \"_id\" : 0 } }])")]
        public void Enum_in_type_argument_query_syntax_1()
        {
            _ = from vehicleType in GetMongoQueryable<VehicleType>()
                where vehicleType.MPG > 20
                select new Tuple<VehicleMake, VehicleTypeEnum>(vehicleType.VehicleMake, vehicleType.Type);
        }

        [MQL("aggregate([{ \"$match\" : { \"MPG\" : { \"$gt\" : 20.0 } } }, { \"$project\" : { \"__fld0\" : [0, 1, 2], \"_id\" : 0 } }])")]
        public void Enum_in_type_argument_2()
        {
            _ = GetMongoCollection<VehicleType>().AsQueryable()
                .Where(v => v.MPG > 20)
                .Select(v => new System.Collections.Generic.List<VehicleTypeEnum> { VehicleTypeEnum.Bus, VehicleTypeEnum.Car, VehicleTypeEnum.Motorcylce });
        }

        [MQL("aggregate([{ \"$match\" : { \"MPG\" : { \"$gt\" : 20.0 } } }, { \"$project\" : { \"__fld0\" : [0, 1, 2], \"_id\" : 0 } }])")]
        public void Enum_in_type_argument_query_syntax_2()
        {
            _ = from vehicleType in GetMongoQueryable<VehicleType>()
                where vehicleType.MPG > 20
                select new System.Collections.Generic.List<VehicleTypeEnum> { VehicleTypeEnum.Bus, VehicleTypeEnum.Car, VehicleTypeEnum.Motorcylce };
        }

        [MQL("aggregate([{ \"$match\" : { \"SiblingsCount\" : { \"$gt\" : 10 } } }, { \"$project\" : { \"__fld0\" : { \"Bus\" : 0, \"Car\" : 1 }, \"_id\" : 0 } }])")]
        public void Enum_in_type_argument_3()
        {
            _ = GetMongoCollection<Person>().AsQueryable()
                .Where(p => p.SiblingsCount > 10)
                .Select(p => new System.Collections.Generic.Dictionary<string, VehicleTypeEnum> { { "Bus", VehicleTypeEnum.Bus }, { "Car", VehicleTypeEnum.Car } });
        }

        [MQL("aggregate([{ \"$match\" : { \"SiblingsCount\" : { \"$gt\" : 10 } } }, { \"$project\" : { \"__fld0\" : { \"Bus\" : 0, \"Car\" : 1 }, \"_id\" : 0 } }])")]
        public void Enum_in_type_argument_query_syntax_3()
        {
            _ = from person in GetMongoQueryable<Person>()
                where person.SiblingsCount > 10
                select new System.Collections.Generic.Dictionary<string, VehicleTypeEnum> { { "Bus", VehicleTypeEnum.Bus }, { "Car", VehicleTypeEnum.Car } };
        }

        [MQL("aggregate([{ \"$match\" : { \"MPG\" : { \"$gt\" : 20.0 } } }, { \"$project\" : { \"Item1\" : \"$VehicleMake\", \"Item2\" : { \"Item1\" : \"$Type\", \"Item2\" : \"$Type\" }, \"_id\" : 0 } }])")]
        public void Enum_in_nested_type_argument()
        {
            _ = GetMongoCollection<VehicleType>().AsQueryable()
                .Where(v => v.MPG > 20)
                .Select(v => new Tuple<VehicleMake, Tuple<VehicleTypeEnum, VehicleTypeEnum>>(v.VehicleMake, new Tuple<VehicleTypeEnum, VehicleTypeEnum>(v.Type, v.Type)));
        }

        [MQL("aggregate([{ \"$match\" : { \"MPG\" : { \"$gt\" : 20.0 } } }, { \"$project\" : { \"Item1\" : \"$VehicleMake\", \"Item2\" : { \"Item1\" : \"$Type\", \"Item2\" : \"$Type\" }, \"_id\" : 0 } }])")]
        public void Enum_in_nested_type_argument_query_syntax()
        {
            _ = from vehicleType in GetMongoQueryable<VehicleType>()
                where vehicleType.MPG > 20
                select new Tuple<VehicleMake, Tuple<VehicleTypeEnum, VehicleTypeEnum>>(vehicleType.VehicleMake, new Tuple<VehicleTypeEnum, VehicleTypeEnum>(vehicleType.Type, vehicleType.Type));
        }

        [MQL("aggregate([{ \"$match\" : { \"$or\" : [{ \"EnumUInt64\" : NumberLong(0) }, { \"EnumUInt64\" : NumberLong(999) }, { \"EnumUInt64\" : NumberLong(-1) }] } }])")]
        [MQL("aggregate([{ \"$match\" : { \"$or\" : [{ \"EnumInt64\" : NumberLong(0) }, { \"EnumInt64\" : NumberLong(999) }, { \"EnumInt64\" : NumberLong(\"9223372036854775807\") }] } }])")]
        [MQL("aggregate([{ \"$match\" : { \"$or\" : [{ \"Vehicle.VehicleType.Type\" : 0 }, { \"Vehicle.VehicleType.Type\" : 2 }] } }, { \"$project\" : { \"LicenceNumber\" : \"$Vehicle.LicenceNumber\", \"_id\" : 0 } }])")]
        public void Query_Syntax()
        {
            _ = from enumHolder in GetMongoQueryable<EnumHolder>()
                where enumHolder.EnumUInt64 == EnumUInt64.Value0 || enumHolder.EnumUInt64 == EnumUInt64.Value999 || enumHolder.EnumUInt64 == EnumUInt64.MaxValue
                select enumHolder;

            _ = from enumHolder in GetMongoQueryable<EnumHolder>()
                where enumHolder.EnumInt64 == EnumInt64.Value0 || enumHolder.EnumInt64 == EnumInt64.Value999 || enumHolder.EnumInt64 == EnumInt64.MaxValue
                select enumHolder;

            _ = from person in GetMongoQueryable<Person>()
                where person.Vehicle.VehicleType.Type == VehicleTypeEnum.Bus || person.Vehicle.VehicleType.Type == VehicleTypeEnum.Motorcylce
                select person.Vehicle.LicenceNumber;
        }
    }
}
