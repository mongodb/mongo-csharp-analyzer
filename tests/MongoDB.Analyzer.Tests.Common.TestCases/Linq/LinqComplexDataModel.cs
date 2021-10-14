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
using MongoDB.Driver.Linq;

namespace MongoDB.Analyzer.Tests.Common.TestCases.Linq
{
    public sealed class LinqComplexDataModel : TestCasesBase
    {
        [MQL("aggregate([{ \"$match\" : { \"Name\" : \"Bob\", \"Address.City\" : \"Vancouver\" } }, { \"$project\" : { \"Province\" : \"$Address.Province\", \"_id\" : 0 } }])")]
        public void Nested_class_depth_of_1()
        {
            _ = GetMongoQueryable<Person>()
                .Where(u => u.Name == "Bob" && u.Address.City == "Vancouver")
                .Select(u => u.Address.Province);
        }

        [MQL("aggregate([{ \"$match\" : { \"Vehicle.VehicleType.Category\" : \"Motorized\" } }, { \"$project\" : { \"LicenceNumber\" : \"$Vehicle.LicenceNumber\", \"_id\" : 0 } }])")]
        public void Nested_class_depth_of_2()
        {
            _ = GetMongoQueryable<Person>()
                .Where(u => u.Vehicle.VehicleType.Category == "Motorized")
                .Select(u => u.Vehicle.LicenceNumber);
        }

        [MQL("aggregate([{ \"$match\" : { \"Vehicle.VehicleType.VehicleMake.Name\" : \"Honda\" } }, { \"$project\" : { \"LicenceNumber\" : \"$Vehicle.LicenceNumber\", \"_id\" : 0 } }])")]
        public void Nested_class_depth_of_3()
        {
            _ = GetMongoQueryable<Person>()
                .Where(u => u.Vehicle.VehicleType.VehicleMake.Name == "Honda")
                .Select(u => u.Vehicle.LicenceNumber);
        }

        [MQL("aggregate([{ \"$match\" : { \"Left.Data\" : 1, \"Right.Data\" : 2 } }])")]
        public void Self_referencing_class_depth_1()
        {
            _ = GetMongoQueryable<TreeNode>()
               .Where(t => t.Left.Data == 1 && t.Right.Data == 2);
        }

        [MQL("aggregate([{ \"$match\" : { \"Root.Left.Tree.Root.Right.Data\" : 1 } }])")]
        public void Self_referencing_class_depth_2()
        {
            _ = GetMongoQueryable<Tree>()
                .Where(t => t.Root.Left.Tree.Root.Right.Data == 1);
        }

        [MQL("aggregate([{ \"$match\" : { \"$or\" : [{ \"Data\" : 1 }, { \"DataT\" : 32 }] } }])")]
        public void Generics_single_predefined_type()
        {
            _ = GetMongoQueryable<SingleTypeGeneric<int>>()
                .Where(t => t.Data == 1 || t.DataT == 32);
        }

        [MQL("aggregate([{ \"$match\" : { \"$or\" : [{ \"Data\" : 1 }, { \"DataT.LastName\" : \"Smith\" }] } }])")]
        public void Generics_single_custom_type()
        {
            _ = GetMongoQueryable<SingleTypeGeneric<Person>>()
                .Where(t => t.Data == 1 || t.DataT.LastName == "Smith");
        }

        [MQL("aggregate([{ \"$match\" : { \"Data\" : 1 } }, { \"$match\" : { \"DataT1\" : 32 } }, { \"$match\" : { \"DataT2\" : \"dataString\" } }, { \"$match\" : { \"DataT3.Vehicle.LicenceNumber\" : \"LicenceNumber\" } }, { \"$match\" : { \"DataT4\" : NumberLong(999) } }])")]
        public void Generics_multiple_mixed_types()
        {
            _ = GetMongoQueryable<MultipleTypeGeneric<int, string, Person, EnumInt64>>()
                .Where(t => t.Data == 1)
                .Where(t => t.DataT1 == 32)
                .Where(t => t.DataT2 == "dataString")
                .Where(t => t.DataT3.Vehicle.LicenceNumber == "LicenceNumber")
                .Where(t => t.DataT4 == EnumInt64.Value999);
        }

        [MQL("aggregate([{ \"$match\" : { \"Data\" : 1 } }, { \"$match\" : { \"DataT1.DataT1.DataT1.DataT2.DataT\" : \"str1\" } }, { \"$match\" : { \"DataT2.DataT2.Name\" : \"Kate\" } }])")]
        public void Generics_nested_generic_types()
        {
            _ = GetMongoQueryable<DualTypeGeneric<DualTypeGeneric<DualTypeGeneric<DualTypeGeneric<int, SingleTypeGeneric<string>>, int>, int>, DualTypeGeneric<int, Person>>>()
                .Where(t => t.Data == 1)
                .Where(t => t.DataT1.DataT1.DataT1.DataT2.DataT == "str1")
                .Where(t => t.DataT2.DataT2.Name == "Kate");
        }

        [MQL("aggregate([{ \"$match\" : { \"AbstractBaseData\" : \"base\" } }, { \"$match\" : { \"NestedClass1Data\" : \"nested1\" } }, { \"$match\" : { \"NestedClass2Data\" : \"nested2\" } }, { \"$match\" : { \"NestedClass3Data\" : \"nested3\" } }])")]
        public void Inheritance_predifined_types()
        {
            _ = GetMongoQueryable<NestedClass3>()
                .Where(t => t.AbstractBaseData == "base")
                .Where(t => t.NestedClass1Data == "nested1")
                .Where(t => t.NestedClass2Data == "nested2")
                .Where(t => t.NestedClass3Data == "nested3");
        }

        [MQL("aggregate([{ \"$match\" : { \"AbstractBaseData\" : \"base\", \"AbstractBaseDataT1\" : 0, \"AbstractBaseDataT2.Name\" : \"Bob\" } }, { \"$match\" : { \"NestedGenericClass1T1\" : 1, \"NestedGenericClass1T2.Name\" : \"Alice\" } }, { \"$match\" : { \"NestedGenericClass2T1\" : 0, \"NestedGenericClass2T2.Name\" : \"John\" } }])")]
        public void Inheritance_generics_mixed_types()
        {
            _ = GetMongoQueryable<NestedGenericClass2<EnumInt32, Person>>()
                .Where(t => t.AbstractBaseData == "base" && t.AbstractBaseDataT1 == EnumInt32.Value0 && t.AbstractBaseDataT2.Name == "Bob")
                .Where(t => t.NestedGenericClass1T1 == EnumInt32.Value1 && t.NestedGenericClass1T2.Name == "Alice")
                .Where(t => t.NestedGenericClass2T1 == EnumInt32.Value0 && t.NestedGenericClass2T2.Name == "John");
        }

        [MQL("aggregate([{ \"$project\" : { \"Item1\" : \"$Name\", \"Item2\" : \"$LastName\", \"_id\" : 0 } }])")]
        public void Tuples_size_2_with_simple_types()
        {
            _ = GetMongoQueryable<Person>()
                .Select(u => new Tuple<string, string>(u.Name, u.LastName));
        }

        [MQL("aggregate([{ \"$project\" : { \"Item1\" : \"$Name\", \"Item2\" : \"$LastName\", \"Item3\" : \"$Vehicle.VehicleType\", \"Item4\" : \"$Vehicle.VehicleType.VehicleMake\", \"_id\" : 0 } }])")]
        public void Tuples_size_4_with_custom_class_and_enum()
        {
            _ = GetMongoQueryable<Person>()
                .Select(u => new Tuple<string, string, VehicleType, VehicleMake>(u.Name, u.LastName, u.Vehicle.VehicleType, u.Vehicle.VehicleType.VehicleMake));
        }

        [MQL("aggregate([{ \"$match\" : { \"IntProp\" : 123 } }])")]
        public void Properties_with_intefaces_and_indexers_should_be_ingored()
        {
            _ = GetMongoQueryable<ClassWithNonTrivialProperties>().Where(t => t.IntProp == 123);
        }
    }
}
