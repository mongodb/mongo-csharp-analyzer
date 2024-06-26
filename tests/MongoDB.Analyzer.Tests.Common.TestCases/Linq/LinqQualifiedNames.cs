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
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace MongoDB.Analyzer.Tests.Common.TestCases.Linq
{
    public sealed class LinqQualifiedNames : TestCasesBase
    {
        [MQL("aggregate([{ \"$match\" : { \"Data\" : 1 } }, { \"$match\" : { \"DataT1\" : 32 } }, { \"$match\" : { \"DataT2\" : \"dataString\" } }, { \"$match\" : { \"DataT3.Vehicle.LicenceNumber\" : \"LicenceNumber\" } }, { \"$match\" : { \"DataT4\" : NumberLong(999) } }])")]
        [MQL("aggregate([{ \"$match\" : { \"Data\" : 1 } }, { \"$match\" : { \"DataT1.DataT1.DataT1.DataT2.DataT\" : \"str1\" } }, { \"$match\" : { \"DataT2.DataT2.Name\" : \"Kate\" } }])")]
        [MQL("aggregate([{ \"$match\" : { \"AbstractBaseData\" : \"base\", \"AbstractBaseDataT1\" : 0, \"AbstractBaseDataT2.Name\" : \"Bob\" } }, { \"$match\" : { \"NestedGenericClass1T1\" : 1, \"NestedGenericClass1T2.Name\" : \"Alice\" } }, { \"$match\" : { \"NestedGenericClass2T1\" : 0, \"NestedGenericClass2T2.Name\" : \"John\" } }])")]
        [MQL("aggregate([{ \"$project\" : { \"Item1\" : \"$Name\", \"Item2\" : \"$LastName\", \"Item3\" : \"$Vehicle.VehicleType\", \"Item4\" : \"$Vehicle.VehicleType.VehicleMake\", \"_id\" : 0 } }])")]
        public void Qualified_generic_type()
        {
            _ = GetMongoQueryable<MongoDB.Analyzer.Tests.Common.DataModel.MultipleTypeGeneric<int, string, MongoDB.Analyzer.Tests.Common.DataModel.Person, MongoDB.Analyzer.Tests.Common.DataModel.EnumInt64>>()
                .Where(t => t.Data == 1)
                .Where(t => t.DataT1 == 32)
                .Where(t => t.DataT2 == "dataString")
                .Where(t => t.DataT3.Vehicle.LicenceNumber == "LicenceNumber")
                .Where(t => t.DataT4 == EnumInt64.Value999);

            _ = GetMongoQueryable<MongoDB.Analyzer.Tests.Common.DataModel.DualTypeGeneric<MongoDB.Analyzer.Tests.Common.DataModel.DualTypeGeneric<MongoDB.Analyzer.Tests.Common.DataModel.DualTypeGeneric<MongoDB.Analyzer.Tests.Common.DataModel.DualTypeGeneric<int, MongoDB.Analyzer.Tests.Common.DataModel.SingleTypeGeneric<string>>, int>, int>, MongoDB.Analyzer.Tests.Common.DataModel.DualTypeGeneric<int, MongoDB.Analyzer.Tests.Common.DataModel.Person>>>()
                .Where(t => t.Data == 1)
                .Where(t => t.DataT1.DataT1.DataT1.DataT2.DataT == "str1")
                .Where(t => t.DataT2.DataT2.Name == "Kate");

            _ = GetMongoQueryable<MongoDB.Analyzer.Tests.Common.DataModel.NestedGenericClass2<MongoDB.Analyzer.Tests.Common.DataModel.EnumInt32, MongoDB.Analyzer.Tests.Common.DataModel.Person>>()
                .Where(t => t.AbstractBaseData == "base" && t.AbstractBaseDataT1 == MongoDB.Analyzer.Tests.Common.DataModel.EnumInt32.Value0 && t.AbstractBaseDataT2.Name == "Bob")
                .Where(t => t.NestedGenericClass1T1 == EnumInt32.Value1 && t.NestedGenericClass1T2.Name == "Alice")
                .Where(t => t.NestedGenericClass2T1 == EnumInt32.Value0 && t.NestedGenericClass2T2.Name == "John");

            _ = GetMongoQueryable<MongoDB.Analyzer.Tests.Common.DataModel.Person>()
                .Select(u => new System.Tuple<string, string, MongoDB.Analyzer.Tests.Common.DataModel.VehicleType, MongoDB.Analyzer.Tests.Common.DataModel.VehicleMake>(u.Name, u.LastName, u.Vehicle.VehicleType, u.Vehicle.VehicleType.VehicleMake));
        }

        [MQL("aggregate([{ \"$match\" : { \"MPG\" : { \"$gt\" : 20.0 } } }, { \"$project\" : { \"__fld0\" : [0, 1, 2], \"_id\" : 0 } }])")]
        [MQL("aggregate([{ \"$match\" : { \"SiblingsCount\" : { \"$gt\" : 10 } } }, { \"$project\" : { \"__fld0\" : { \"Bus\" : 0, \"Car\" : 1 }, \"_id\" : 0 } }])")]
        [MQL("aggregate([{ \"$match\" : { \"$or\" : [{ \"SiblingsCount\" : MongoDB.Analyzer.Tests.Common.DataModel.StaticHolder.PropByte }, { \"SiblingsCount\" : MongoDB.Analyzer.Tests.Common.DataModel.StaticHolder.PropShort }, { \"SiblingsCount\" : MongoDB.Analyzer.Tests.Common.DataModel.StaticHolder.PropInt }, { \"TicksSinceBirth\" : NumberLong(MongoDB.Analyzer.Tests.Common.DataModel.StaticHolder.PropLong) }, { \"Name\" : MongoDB.Analyzer.Tests.Common.DataModel.StaticHolder.PropString }, { \"Name\" : MongoDB.Analyzer.Tests.Common.DataModel.StaticHolder.PropString }, { \"Name\" : MongoDB.Analyzer.Tests.Common.DataModel.StaticHolder.PropString2 }, { \"Name\" : MongoDB.Analyzer.Tests.Common.DataModel.StaticHolder.PropString2 }, { \"Vehicle.VehicleType.Type\" : MongoDB.Analyzer.Tests.Common.DataModel.StaticHolder.PropEnum }, { \"Vehicle.VehicleType.MPG\" : MongoDB.Analyzer.Tests.Common.DataModel.StaticHolder.PropDouble }] } }])")]
        public void Qualified_in_method()
        {
            _ = GetMongoCollection<MongoDB.Analyzer.Tests.Common.DataModel.VehicleType>().AsQueryable()
                .Where(v => v.MPG > 20)
                .Select(v => new System.Collections.Generic.List<MongoDB.Analyzer.Tests.Common.DataModel.VehicleTypeEnum> { MongoDB.Analyzer.Tests.Common.DataModel.VehicleTypeEnum.Bus, MongoDB.Analyzer.Tests.Common.DataModel.VehicleTypeEnum.Car, MongoDB.Analyzer.Tests.Common.DataModel.VehicleTypeEnum.Motorcylce });

            _ = GetMongoCollection<MongoDB.Analyzer.Tests.Common.DataModel.Person>().AsQueryable()
                .Where(p => p.SiblingsCount > 10)
                .Select(p => new System.Collections.Generic.Dictionary<string, MongoDB.Analyzer.Tests.Common.DataModel.VehicleTypeEnum> { { "Bus", MongoDB.Analyzer.Tests.Common.DataModel.VehicleTypeEnum.Bus }, { "Car", MongoDB.Analyzer.Tests.Common.DataModel.VehicleTypeEnum.Car } });

            _ = GetMongoQueryable<Person>().Where(p =>
                p.SiblingsCount == MongoDB.Analyzer.Tests.Common.DataModel.StaticHolder.PropByte ||
                p.SiblingsCount == MongoDB.Analyzer.Tests.Common.DataModel.StaticHolder.PropShort ||
                p.SiblingsCount == MongoDB.Analyzer.Tests.Common.DataModel.StaticHolder.PropInt ||
                p.TicksSinceBirth == MongoDB.Analyzer.Tests.Common.DataModel.StaticHolder.PropLong ||
                p.Name == MongoDB.Analyzer.Tests.Common.DataModel.StaticHolder.PropString ||
                p.Name == MongoDB.Analyzer.Tests.Common.DataModel.StaticHolder.PropString ||
                p.Name == MongoDB.Analyzer.Tests.Common.DataModel.StaticHolder.PropString2 ||
                p.Name == MongoDB.Analyzer.Tests.Common.DataModel.StaticHolder.PropString2 ||
                p.Vehicle.VehicleType.Type == MongoDB.Analyzer.Tests.Common.DataModel.StaticHolder.PropEnum ||
                p.Vehicle.VehicleType.MPG == MongoDB.Analyzer.Tests.Common.DataModel.StaticHolder.PropDouble);
        }

        [MQL("aggregate([{ \"$match\" : { \"MPG\" : { \"$gt\" : 20.0 } } }, { \"$project\" : { \"__fld0\" : [0, 1, 2], \"_id\" : 0 } }])")]
        [MQL("aggregate([{ \"$match\" : { \"SiblingsCount\" : { \"$gt\" : 10 } } }, { \"$project\" : { \"__fld0\" : { \"Bus\" : 0, \"Car\" : 1 }, \"_id\" : 0 } }])")]
        [MQL("aggregate([{ \"$match\" : { \"$or\" : [{ \"SiblingsCount\" : DataModel.StaticHolder.PropByte }, { \"SiblingsCount\" : DataModel.StaticHolder.PropShort }, { \"SiblingsCount\" : Analyzer.Tests.Common.DataModel.StaticHolder.PropInt }, { \"TicksSinceBirth\" : NumberLong(Analyzer.Tests.Common.DataModel.StaticHolder.PropLong) }, { \"Name\" : DataModel.StaticHolder.PropString }, { \"Name\" : DataModel.StaticHolder.PropString }, { \"Name\" : Common.DataModel.StaticHolder.PropString2 }, { \"Name\" : Common.DataModel.StaticHolder.PropString2 }, { \"Vehicle.VehicleType.Type\" : Tests.Common.DataModel.StaticHolder.PropEnum }, { \"Vehicle.VehicleType.MPG\" : Tests.Common.DataModel.StaticHolder.PropDouble }] } }])")]
        public void Qualified_in_namespace()
        {
            _ = GetMongoCollection<Tests.Common.DataModel.VehicleType>().AsQueryable()
                .Where(v => v.MPG > 20)
                .Select(v => new System.Collections.Generic.List<Tests.Common.DataModel.VehicleTypeEnum> { Tests.Common.DataModel.VehicleTypeEnum.Bus, Tests.Common.DataModel.VehicleTypeEnum.Car, Tests.Common.DataModel.VehicleTypeEnum.Motorcylce });

            _ = GetMongoCollection<Common.DataModel.Person>().AsQueryable()
                .Where(p => p.SiblingsCount > 10)
                .Select(p => new System.Collections.Generic.Dictionary<string, Common.DataModel.VehicleTypeEnum> { { "Bus", Common.DataModel.VehicleTypeEnum.Bus }, { "Car", Common.DataModel.VehicleTypeEnum.Car } });

            _ = GetMongoQueryable<Person>().Where(p =>
                p.SiblingsCount == DataModel.StaticHolder.PropByte ||
                p.SiblingsCount == DataModel.StaticHolder.PropShort ||
                p.SiblingsCount == Analyzer.Tests.Common.DataModel.StaticHolder.PropInt ||
                p.TicksSinceBirth == Analyzer.Tests.Common.DataModel.StaticHolder.PropLong ||
                p.Name == DataModel.StaticHolder.PropString ||
                p.Name == DataModel.StaticHolder.PropString ||
                p.Name == Common.DataModel.StaticHolder.PropString2 ||
                p.Name == Common.DataModel.StaticHolder.PropString2 ||
                p.Vehicle.VehicleType.Type == Tests.Common.DataModel.StaticHolder.PropEnum ||
                p.Vehicle.VehicleType.MPG == Tests.Common.DataModel.StaticHolder.PropDouble);
        }

        [MQL("aggregate([{ \"$match\" : { \"StringField\" : \"value\" } }])")]
        [MQL("aggregate([{ \"$match\" : { \"StringField\" : \"value\" } }])")]
        public void Qualified_type()
        {
            _ = GetMongoQueryable<MongoDB.Analyzer.Tests.Common.DataModel.ClassWithObjectId>().Where(c => c.StringField == "value");

            _ = from classWithObjectId in GetMongoQueryable<MongoDB.Analyzer.Tests.Common.DataModel.ClassWithObjectId>()
                where classWithObjectId.StringField == "value"
                select classWithObjectId;
        }
    }
}
