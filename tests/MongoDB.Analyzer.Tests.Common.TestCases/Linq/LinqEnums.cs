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

            VehicleTypeEnum? nullibleVehicleType = VehicleTypeEnum.Bus;

            _ = GetMongoQueryable<Person>()
                .Where(u => u.Vehicle.VehicleType.Type == nullibleVehicleType)
                .Select(u => u.Vehicle.LicenceNumber);
        }

        [MQL("aggregate([{ \"$match\" : { \"Vehicle.VehicleType.Type\" : vehicleType } }, { \"$project\" : { \"LicenceNumber\" : \"$Vehicle.LicenceNumber\", \"_id\" : vehicleType } }])")]
        public void Enum_single_variable_query_syntax()
        {
            var vehicleType = VehicleTypeEnum.Bus;

            _ = from person in GetMongoQueryable<Person>()
                where person.Vehicle.VehicleType.Type == vehicleType
                select person.Vehicle.LicenceNumber;

            VehicleTypeEnum? nullibleVehicleType = VehicleTypeEnum.Bus;

            _ = from person in GetMongoQueryable<Person>()
                where person.Vehicle.VehicleType.Type == nullibleVehicleType
                select person.Vehicle.LicenceNumber;
        }

        [MQL("aggregate([{ \"$match\" : { \"$or\" : [{ \"EnumUInt64\" : NumberLong(firstUnsignedEnum) }, { \"EnumUInt64\" : NumberLong(secondUnsignedEnum) }, { \"EnumUInt64\" : NumberLong(thirdUnsignedEnum) }] } }])")]
        [MQL("aggregate([{ \"$match\" : { \"$or\" : [{ \"EnumInt64\" : NumberLong(firstSignedEnum) }, { \"EnumInt64\" : NumberLong(secondSignedEnum) }, { \"EnumInt64\" : NumberLong(thirdSignedEnum) }] } }])")]
        [MQL("aggregate([{ \"$match\" : { \"$or\" : [{ \"Vehicle.VehicleType.Type\" : firstVehicleType }, { \"Vehicle.VehicleType.Type\" : secondVehicleType }] } }, { \"$project\" : { \"LicenceNumber\" : \"$Vehicle.LicenceNumber\", \"_id\" : firstVehicleType } }])")]
        public void Enum_multiple_variables()
        {
            var firstUnsignedEnum = EnumUInt64.Value0;
            var secondUnsignedEnum = EnumUInt64.Value999;
            var thirdUnsignedEnum = EnumUInt64.MaxValue;

            var firstSignedEnum = EnumInt64.Value0;
            var secondSignedEnum = EnumInt64.Value999;
            var thirdSignedEnum = EnumInt64.MaxValue;

            var firstVehicleType = VehicleTypeEnum.Bus;
            var secondVehicleType = VehicleTypeEnum.Motorcylce;

            _ = GetMongoQueryable<EnumHolder>()
                .Where(u => u.EnumUInt64 == firstUnsignedEnum || u.EnumUInt64 == secondUnsignedEnum || u.EnumUInt64 == thirdUnsignedEnum)
                .Select(u => u);

            _ = GetMongoQueryable<EnumHolder>()
                .Where(u => u.EnumInt64 == firstSignedEnum || u.EnumInt64 == secondSignedEnum || u.EnumInt64 == thirdSignedEnum)
                .Select(u => u);

            _ = GetMongoQueryable<Person>()
                .Where(u => u.Vehicle.VehicleType.Type == firstVehicleType || u.Vehicle.VehicleType.Type == secondVehicleType)
                .Select(u => u.Vehicle.LicenceNumber);
        }

        [MQL("aggregate([{ \"$match\" : { \"$or\" : [{ \"EnumUInt64\" : NumberLong(firstUnsignedEnum) }, { \"EnumUInt64\" : NumberLong(secondUnsignedEnum) }, { \"EnumUInt64\" : NumberLong(thirdUnsignedEnum) }] } }])")]
        [MQL("aggregate([{ \"$match\" : { \"$or\" : [{ \"EnumInt64\" : NumberLong(firstSignedEnum) }, { \"EnumInt64\" : NumberLong(secondSignedEnum) }, { \"EnumInt64\" : NumberLong(thirdSignedEnum) }] } }])")]
        [MQL("aggregate([{ \"$match\" : { \"$or\" : [{ \"Vehicle.VehicleType.Type\" : firstVehicleType }, { \"Vehicle.VehicleType.Type\" : secondVehicleType }] } }, { \"$project\" : { \"LicenceNumber\" : \"$Vehicle.LicenceNumber\", \"_id\" : firstVehicleType } }])")]
        public void Enum_multiple_variables_query_syntax()
        {
            var firstUnsignedEnum = EnumUInt64.Value0;
            var secondUnsignedEnum = EnumUInt64.Value999;
            var thirdUnsignedEnum = EnumUInt64.MaxValue;

            var firstSignedEnum = EnumInt64.Value0;
            var secondSignedEnum = EnumInt64.Value999;
            var thirdSignedEnum = EnumInt64.MaxValue;

            var firstVehicleType = VehicleTypeEnum.Bus;
            var secondVehicleType = VehicleTypeEnum.Motorcylce;

            _ = from enumHolder in GetMongoQueryable<EnumHolder>()
                where enumHolder.EnumUInt64 == firstUnsignedEnum || enumHolder.EnumUInt64 == secondUnsignedEnum || enumHolder.EnumUInt64 == thirdUnsignedEnum
                select enumHolder;

            _ = from enumHolder in GetMongoQueryable<EnumHolder>()
                where enumHolder.EnumInt64 == firstSignedEnum || enumHolder.EnumInt64 == secondSignedEnum || enumHolder.EnumInt64 == thirdSignedEnum
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
