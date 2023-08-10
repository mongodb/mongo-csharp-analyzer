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

        [MQL("aggregate([{ \"$match\" : { \"Vehicle.VehicleType.Type\" : vehicleType } }, { \"$project\" : { \"LicenceNumber\" : \"$Vehicle.LicenceNumber\", \"_id\" : vehicleType } }])")]
        public void Enum_single_variable()
        {
            var vehicleType = VehicleTypeEnum.Bus;

            _ = GetMongoQueryable<Person>()
                .Where(u => u.Vehicle.VehicleType.Type == vehicleType)
                .Select(u => u.Vehicle.LicenceNumber);
        }

        [MQL("aggregate([{ \"$match\" : { \"Vehicle.VehicleType.Type\" : vehicleType } }, { \"$project\" : { \"LicenceNumber\" : \"$Vehicle.LicenceNumber\", \"_id\" : vehicleType } }])")]
        public void Enum_single_variable_query_syntax()
        {
            var vehicleType = VehicleTypeEnum.Bus;

            _ = from person in GetMongoQueryable<Person>()
                where person.Vehicle.VehicleType.Type == vehicleType
                select person.Vehicle.LicenceNumber;
        }

        [MQL("aggregate([{ \"$match\" : { \"$or\" : [{ \"EnumUInt64\" : NumberLong(firstEnumUInt64) }, { \"EnumUInt64\" : NumberLong(secondEnumUInt64) }, { \"EnumUInt64\" : NumberLong(thirdEnumUInt64) }, { \"EnumUInt64\" : NumberLong(fourthEnumUInt64) }] } }, { \"$match\" : { \"$or\" : [{ \"EnumInt64\" : NumberLong(firstEnumInt64) }, { \"EnumInt64\" : NumberLong(secondEnumInt64) }, { \"EnumInt64\" : NumberLong(thirdEnumInt64) }, { \"EnumInt64\" : NumberLong(fourthEnumInt64) }] } }, { \"$match\" : { \"$or\" : [{ \"EnumUInt32\" : firstEnumUInt32 }, { \"EnumUInt32\" : secondEnumUInt32 }, { \"EnumUInt32\" : thirdEnumUInt32 }, { \"EnumUInt32\" : fourthEnumUInt32 }] } }, { \"$match\" : { \"$or\" : [{ \"EnumInt32\" : firstEnumInt32 }, { \"EnumInt32\" : secondEnumInt32 }, { \"EnumInt32\" : thirdEnumInt32 }, { \"EnumInt32\" : fourthEnumInt32 }] } }, { \"$match\" : { \"$or\" : [{ \"EnumUInt16\" : firstEnumUInt16 }, { \"EnumUInt16\" : secondEnumUInt16 }, { \"EnumUInt16\" : thirdEnumUInt16 }, { \"EnumUInt16\" : fourthEnumUInt16 }] } }, { \"$match\" : { \"$or\" : [{ \"EnumInt16\" : firstEnumInt16 }, { \"EnumInt16\" : secondEnumInt16 }, { \"EnumInt16\" : thirdEnumInt16 }, { \"EnumInt16\" : fourthEnumInt16 }] } }, { \"$match\" : { \"$or\" : [{ \"EnumUInt8\" : firstEnumUInt8 }, { \"EnumUInt8\" : secondEnumUInt8 }, { \"EnumUInt8\" : thirdEnumUInt8 }, { \"EnumUInt8\" : fourthEnumUInt8 }] } }, { \"$match\" : { \"$or\" : [{ \"EnumInt8\" : firstEnumInt8 }, { \"EnumInt8\" : secondEnumInt8 }, { \"EnumInt8\" : thirdEnumInt8 }, { \"EnumInt8\" : fourthEnumInt8 }] } }])")]
        [MQL("aggregate([{ \"$match\" : { \"$or\" : [{ \"Vehicle.VehicleType.Type\" : firstVehicleType }, { \"Vehicle.VehicleType.Type\" : secondVehicleType }] } }, { \"$project\" : { \"LicenceNumber\" : \"$Vehicle.LicenceNumber\", \"_id\" : firstVehicleType } }])")]
        public void Enum_multiple_variables()
        {
            var firstEnumInt8 = EnumInt8.Value0;
            var secondEnumInt8 = EnumInt8.Value1;
            var thirdEnumInt8 = EnumInt8.Value9;
            var fourthEnumInt8 = EnumInt8.MaxValue;

            var firstEnumUInt8 = EnumUInt8.Value0;
            var secondEnumUInt8 = EnumUInt8.Value1;
            var thirdEnumUInt8 = EnumUInt8.Value9;
            var fourthEnumUInt8 = EnumUInt8.MaxValue;

            var firstEnumInt16 = EnumInt16.Value0;
            var secondEnumInt16 = EnumInt16.Value1;
            var thirdEnumInt16 = EnumInt16.Value999;
            var fourthEnumInt16 = EnumInt16.MaxValue;

            var firstEnumUInt16 = EnumUInt16.Value0;
            var secondEnumUInt16 = EnumUInt16.Value1;
            var thirdEnumUInt16 = EnumUInt16.Value999;
            var fourthEnumUInt16 = EnumUInt16.MaxValue;

            var firstEnumInt32 = EnumInt32.Value0;
            var secondEnumInt32 = EnumInt32.Value1;
            var thirdEnumInt32 = EnumInt32.Value999;
            var fourthEnumInt32 = EnumInt32.MaxValue;

            var firstEnumUInt32 = EnumUInt32.Value0;
            var secondEnumUInt32 = EnumUInt32.Value1;
            var thirdEnumUInt32 = EnumUInt32.Value999;
            var fourthEnumUInt32 = EnumUInt32.MaxValue;

            var firstEnumInt64 = EnumInt64.Value0;
            var secondEnumInt64 = EnumInt64.Value1;
            var thirdEnumInt64 = EnumInt64.Value999;
            var fourthEnumInt64 = EnumInt64.MaxValue;

            var firstEnumUInt64 = EnumUInt64.Value0;
            var secondEnumUInt64 = EnumUInt64.Value1;
            var thirdEnumUInt64 = EnumUInt64.Value999;
            var fourthEnumUInt64 = EnumUInt64.MaxValue;

            var firstVehicleType = VehicleTypeEnum.Bus;
            var secondVehicleType = VehicleTypeEnum.Motorcylce;

            _ = GetMongoQueryable<EnumHolder>()
                .Where(u => u.EnumUInt64 == firstEnumUInt64 || u.EnumUInt64 == secondEnumUInt64 || u.EnumUInt64 == thirdEnumUInt64 || u.EnumUInt64 == fourthEnumUInt64)
                .Where(u => u.EnumInt64 == firstEnumInt64 || u.EnumInt64 == secondEnumInt64 || u.EnumInt64 == thirdEnumInt64 || u.EnumInt64 == fourthEnumInt64)
                .Where(u => u.EnumUInt32 == firstEnumUInt32 || u.EnumUInt32 == secondEnumUInt32 || u.EnumUInt32 == thirdEnumUInt32 || u.EnumUInt32 == fourthEnumUInt32)
                .Where(u => u.EnumInt32 == firstEnumInt32 || u.EnumInt32 == secondEnumInt32 || u.EnumInt32 == thirdEnumInt32 || u.EnumInt32 == fourthEnumInt32)
                .Where(u => u.EnumUInt16 == firstEnumUInt16 || u.EnumUInt16 == secondEnumUInt16 || u.EnumUInt16 == thirdEnumUInt16 || u.EnumUInt16 == fourthEnumUInt16)
                .Where(u => u.EnumInt16 == firstEnumInt16 || u.EnumInt16 == secondEnumInt16 || u.EnumInt16 == thirdEnumInt16 || u.EnumInt16 == fourthEnumInt16)
                .Where(u => u.EnumUInt8 == firstEnumUInt8 || u.EnumUInt8 == secondEnumUInt8 || u.EnumUInt8 == thirdEnumUInt8 || u.EnumUInt8 == fourthEnumUInt8)
                .Where(u => u.EnumInt8 == firstEnumInt8 || u.EnumInt8 == secondEnumInt8 || u.EnumInt8 == thirdEnumInt8 || u.EnumInt8 == fourthEnumInt8)
                .Select(u => u);

            _ = GetMongoQueryable<Person>()
                .Where(u => u.Vehicle.VehicleType.Type == firstVehicleType || u.Vehicle.VehicleType.Type == secondVehicleType)
                .Select(u => u.Vehicle.LicenceNumber);
        }

        [MQL("aggregate([{ \"$match\" : { \"$or\" : [{ \"EnumUInt64\" : NumberLong(firstEnumUInt64) }, { \"EnumUInt64\" : NumberLong(secondEnumUInt64) }, { \"EnumUInt64\" : NumberLong(thirdEnumUInt64) }, { \"EnumUInt64\" : NumberLong(fourthEnumUInt64) }] } }, { \"$match\" : { \"$or\" : [{ \"EnumInt64\" : NumberLong(firstEnumInt64) }, { \"EnumInt64\" : NumberLong(secondEnumInt64) }, { \"EnumInt64\" : NumberLong(thirdEnumInt64) }, { \"EnumInt64\" : NumberLong(fourthEnumInt64) }] } }, { \"$match\" : { \"$or\" : [{ \"EnumUInt32\" : firstEnumUInt32 }, { \"EnumUInt32\" : secondEnumUInt32 }, { \"EnumUInt32\" : thirdEnumUInt32 }, { \"EnumUInt32\" : fourthEnumUInt32 }] } }, { \"$match\" : { \"$or\" : [{ \"EnumInt32\" : firstEnumInt32 }, { \"EnumInt32\" : secondEnumInt32 }, { \"EnumInt32\" : thirdEnumInt32 }, { \"EnumInt32\" : fourthEnumInt32 }] } }, { \"$match\" : { \"$or\" : [{ \"EnumUInt16\" : firstEnumUInt16 }, { \"EnumUInt16\" : secondEnumUInt16 }, { \"EnumUInt16\" : thirdEnumUInt16 }, { \"EnumUInt16\" : fourthEnumUInt16 }] } }, { \"$match\" : { \"$or\" : [{ \"EnumInt16\" : firstEnumInt16 }, { \"EnumInt16\" : secondEnumInt16 }, { \"EnumInt16\" : thirdEnumInt16 }, { \"EnumInt16\" : fourthEnumInt16 }] } }, { \"$match\" : { \"$or\" : [{ \"EnumUInt8\" : firstEnumUInt8 }, { \"EnumUInt8\" : secondEnumUInt8 }, { \"EnumUInt8\" : thirdEnumUInt8 }, { \"EnumUInt8\" : fourthEnumUInt8 }] } }, { \"$match\" : { \"$or\" : [{ \"EnumInt8\" : firstEnumInt8 }, { \"EnumInt8\" : secondEnumInt8 }, { \"EnumInt8\" : thirdEnumInt8 }, { \"EnumInt8\" : fourthEnumInt8 }] } }])")]
        [MQL("aggregate([{ \"$match\" : { \"$or\" : [{ \"Vehicle.VehicleType.Type\" : firstVehicleType }, { \"Vehicle.VehicleType.Type\" : secondVehicleType }] } }, { \"$project\" : { \"LicenceNumber\" : \"$Vehicle.LicenceNumber\", \"_id\" : firstVehicleType } }])")]
        public void Enum_multiple_variables_query_syntax()
        {
            var firstEnumInt8 = EnumInt8.Value0;
            var secondEnumInt8 = EnumInt8.Value1;
            var thirdEnumInt8 = EnumInt8.Value9;
            var fourthEnumInt8 = EnumInt8.MaxValue;

            var firstEnumUInt8 = EnumUInt8.Value0;
            var secondEnumUInt8 = EnumUInt8.Value1;
            var thirdEnumUInt8 = EnumUInt8.Value9;
            var fourthEnumUInt8 = EnumUInt8.MaxValue;

            var firstEnumInt16 = EnumInt16.Value0;
            var secondEnumInt16 = EnumInt16.Value1;
            var thirdEnumInt16 = EnumInt16.Value999;
            var fourthEnumInt16 = EnumInt16.MaxValue;

            var firstEnumUInt16 = EnumUInt16.Value0;
            var secondEnumUInt16 = EnumUInt16.Value1;
            var thirdEnumUInt16 = EnumUInt16.Value999;
            var fourthEnumUInt16 = EnumUInt16.MaxValue;

            var firstEnumInt32 = EnumInt32.Value0;
            var secondEnumInt32 = EnumInt32.Value1;
            var thirdEnumInt32 = EnumInt32.Value999;
            var fourthEnumInt32 = EnumInt32.MaxValue;

            var firstEnumUInt32 = EnumUInt32.Value0;
            var secondEnumUInt32 = EnumUInt32.Value1;
            var thirdEnumUInt32 = EnumUInt32.Value999;
            var fourthEnumUInt32 = EnumUInt32.MaxValue;

            var firstEnumInt64 = EnumInt64.Value0;
            var secondEnumInt64 = EnumInt64.Value1;
            var thirdEnumInt64 = EnumInt64.Value999;
            var fourthEnumInt64 = EnumInt64.MaxValue;

            var firstEnumUInt64 = EnumUInt64.Value0;
            var secondEnumUInt64 = EnumUInt64.Value1;
            var thirdEnumUInt64 = EnumUInt64.Value999;
            var fourthEnumUInt64 = EnumUInt64.MaxValue;

            var firstVehicleType = VehicleTypeEnum.Bus;
            var secondVehicleType = VehicleTypeEnum.Motorcylce;

            _ = from enumHolder in GetMongoQueryable<EnumHolder>()
                where enumHolder.EnumUInt64 == firstEnumUInt64 || enumHolder.EnumUInt64 == secondEnumUInt64 || enumHolder.EnumUInt64 == thirdEnumUInt64 || enumHolder.EnumUInt64 == fourthEnumUInt64
                where enumHolder.EnumInt64 == firstEnumInt64 || enumHolder.EnumInt64 == secondEnumInt64 || enumHolder.EnumInt64 == thirdEnumInt64 || enumHolder.EnumInt64 == fourthEnumInt64
                where enumHolder.EnumUInt32 == firstEnumUInt32 || enumHolder.EnumUInt32 == secondEnumUInt32 || enumHolder.EnumUInt32 == thirdEnumUInt32 || enumHolder.EnumUInt32 == fourthEnumUInt32
                where enumHolder.EnumInt32 == firstEnumInt32 || enumHolder.EnumInt32 == secondEnumInt32 || enumHolder.EnumInt32 == thirdEnumInt32 || enumHolder.EnumInt32 == fourthEnumInt32
                where enumHolder.EnumUInt16 == firstEnumUInt16 || enumHolder.EnumUInt16 == secondEnumUInt16 || enumHolder.EnumUInt16 == thirdEnumUInt16 || enumHolder.EnumUInt16 == fourthEnumUInt16
                where enumHolder.EnumInt16 == firstEnumInt16 || enumHolder.EnumInt16 == secondEnumInt16 || enumHolder.EnumInt16 == thirdEnumInt16 || enumHolder.EnumInt16 == fourthEnumInt16
                where enumHolder.EnumUInt8 == firstEnumUInt8 || enumHolder.EnumUInt8 == secondEnumUInt8 || enumHolder.EnumUInt8 == thirdEnumUInt8 || enumHolder.EnumUInt8 == fourthEnumUInt8
                where enumHolder.EnumInt8 == firstEnumInt8 || enumHolder.EnumInt8 == secondEnumInt8 || enumHolder.EnumInt8 == thirdEnumInt8 || enumHolder.EnumInt8 == fourthEnumInt8
                select enumHolder;

            _ = from person in GetMongoQueryable<Person>()
                where person.Vehicle.VehicleType.Type == firstVehicleType || person.Vehicle.VehicleType.Type == secondVehicleType
                select person.Vehicle.LicenceNumber;
        }

        [MQL("aggregate([{ \"$match\" : { \"EnumInt8\" : 5 } }])")]
        [MQL("aggregate([{ \"$match\" : { \"EnumInt8\" : -5 } }])")]
        [MQL("aggregate([{ \"$match\" : { \"EnumInt16\" : 15 } }])")]
        [MQL("aggregate([{ \"$match\" : { \"EnumInt16\" : -15 } }])")]
        [MQL("aggregate([{ \"$match\" : { \"EnumInt32\" : 25 } }])")]
        [MQL("aggregate([{ \"$match\" : { \"EnumInt32\" : -25 } }])")]
        [MQL("aggregate([{ \"$match\" : { \"EnumInt64\" : NumberLong(35) } }])")]
        [MQL("aggregate([{ \"$match\" : { \"EnumInt64\" : NumberLong(-35) } }])")]
        public void Enum_arithmetic_operations()
        {
            _ = GetMongoQueryable<EnumHolder>()
                .Where(u => u.EnumInt8 == EnumInt8.Value0 + 5);

            _ = GetMongoQueryable<EnumHolder>()
                .Where(u => u.EnumInt8 == EnumInt8.Value0 - 5);

            _ = GetMongoQueryable<EnumHolder>()
                .Where(u => u.EnumInt16 == EnumInt16.Value0 + 15);

            _ = GetMongoQueryable<EnumHolder>()
                .Where(u => u.EnumInt16 == EnumInt16.Value0 - 15);

            _ = GetMongoQueryable<EnumHolder>()
                .Where(u => u.EnumInt32 == EnumInt32.Value0 + 25);

            _ = GetMongoQueryable<EnumHolder>()
                .Where(u => u.EnumInt32 == EnumInt32.Value0 - 25);

            _ = GetMongoQueryable<EnumHolder>()
                .Where(u => u.EnumInt64 == EnumInt64.Value0 + 35);

            _ = GetMongoQueryable<EnumHolder>()
                .Where(u => u.EnumInt64 == EnumInt64.Value0 - 35);
        }
    }
}
