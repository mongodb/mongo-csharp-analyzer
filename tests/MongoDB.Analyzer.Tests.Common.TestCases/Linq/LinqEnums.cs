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
    }
}
