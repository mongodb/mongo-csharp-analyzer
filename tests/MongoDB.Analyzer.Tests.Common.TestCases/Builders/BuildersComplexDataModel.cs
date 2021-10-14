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

namespace MongoDB.Analyzer.Tests.Common.TestCases.Builders
{
    public sealed class BuildersComplexDataModel : TestCasesBase
    {
        [BuildersMQL("{ \"Address.City.122\" : { \"$exists\" : false } }")]
        public void Nested_class_depth_of_1()
        {
            _ = Builders<Person>.Filter.SizeLt(u => u.Address.City, 123);
        }

        [BuildersMQL("{ \"Vehicle.VehicleType.Category\" : \"Motorized\" }")]
        public void Nested_class_depth_of_2()
        {
            _ = Builders<Person>.Filter.Eq(u => u.Vehicle.VehicleType.Category, "Motorized");
        }

        [BuildersMQL("{ \"Vehicle.VehicleType.VehicleMake.Name\" : \"Honda\" }")]
        public void Nested_class_depth_of_3()
        {
            _ = Builders<Person>.Filter.Eq(u => u.Vehicle.VehicleType.VehicleMake.Name, "Honda");
        }

        [BuildersMQL("{ \"Root.Left.Data\" : 1, \"Root.Right.Data\" : 2 }")]
        public void Self_referencing_class_depth_1()
        {
            _ = Builders<Tree>.Filter.Eq(t => t.Root.Left.Data, 1) &
                Builders<Tree>.Filter.Eq(t => t.Root.Right.Data, 2);
        }

        [BuildersMQL("{ \"Root.Left.Tree.Root.Right.Data\" : 1 }")]
        public void Self_referencing_class_depth_2()
        {
            _ = Builders<Tree>.Sort.Ascending(t => t.Root.Left.Tree.Root.Right.Data);
        }

        [BuildersMQL("{ \"$or\" : [{ \"Data\" : { \"$lte\" : 1 } }, { \"DataT\" : { \"$mod\" : [NumberLong(10), NumberLong(2)] } }] }")]
        public void Generics_single_predefined_type()
        {
            _ = Builders<SingleTypeGeneric<int>>.Filter.Lte(t => t.Data, 1) |
                Builders<SingleTypeGeneric<int>>.Filter.Mod(t => t.DataT, 10, 2);
        }

        [BuildersMQL("{ \"$or\" : [{ \"Data\" : { \"$gte\" : 1 } }, { \"DataT.Address.City\" : { \"$gte\" : \"Berlin\" } }] }")]
        public void Generics_single_custom_type()
        {
            _ = Builders<SingleTypeGeneric<Person>>.Filter.Gte(t => t.Data, 1) |
                Builders<SingleTypeGeneric<Person>>.Filter.Gte(t => t.DataT.Address.City, "Berlin");
        }

        [BuildersMQL("{ \"$or\" : [{ \"Data\" : 1 }, { \"DataT1\" : 32 }, { \"DataT2\" : \"dataString\" }, { \"DataT3.Vehicle.LicenceNumber\" : \"LicenceNumber\" }] }")]
        public void Generics_multiple_mixed_types()
        {
            _ = Builders<MultipleTypeGeneric<int, string, Person, EnumInt64>>.Filter.Eq(t => t.Data, 1) |
                Builders<MultipleTypeGeneric<int, string, Person, EnumInt64>>.Filter.Eq(t => t.DataT1, 32) |
                Builders<MultipleTypeGeneric<int, string, Person, EnumInt64>>.Filter.Eq(t => t.DataT2, "dataString") |
                Builders<MultipleTypeGeneric<int, string, Person, EnumInt64>>.Filter.Eq(t => t.DataT3.Vehicle.LicenceNumber, "LicenceNumber");
        }

        [BuildersMQL("{ \"Data\" : { \"$lte\" : 1 }, \"DataT1.DataT1.DataT1.DataT2.DataT\" : \"str1\", \"DataT2.DataT2.Name\" : \"Kate\" }")]
        public void Generics_nested_generic_types()
        {
            _ = Builders<DualTypeGeneric<DualTypeGeneric<DualTypeGeneric<DualTypeGeneric<int, SingleTypeGeneric<string>>, int>, int>, DualTypeGeneric<int, Person>>>
                    .Filter.Lte(t => t.Data, 1) &
                Builders<DualTypeGeneric<DualTypeGeneric<DualTypeGeneric<DualTypeGeneric<int, SingleTypeGeneric<string>>, int>, int>, DualTypeGeneric<int, Person>>>
                    .Filter.Eq(t => t.DataT1.DataT1.DataT1.DataT2.DataT, "str1") &
                Builders<DualTypeGeneric<DualTypeGeneric<DualTypeGeneric<DualTypeGeneric<int, SingleTypeGeneric<string>>, int>, int>, DualTypeGeneric<int, Person>>>
                    .Filter.Eq(t => t.DataT2.DataT2.Name, "Kate");
        }

        [BuildersMQL("{ \"AbstractBaseData\" : \"base\", \"NestedClass1Data\" : \"nested1\", \"NestedClass2Data\" : \"nested2\", \"NestedClass3Data\" : \"nested3\" }")]
        public void Inheritance_predifined_types()
        {
            _ = Builders<NestedClass3>.Filter.Eq(t => t.AbstractBaseData, "base") &
                Builders<NestedClass3>.Filter.Eq(t => t.NestedClass1Data, "nested1") &
                Builders<NestedClass3>.Filter.Eq(t => t.NestedClass2Data, "nested2") &
                Builders<NestedClass3>.Filter.Eq(t => t.NestedClass3Data, "nested3");
        }

        [BuildersMQL("{ \"$or\" : [{ \"AbstractBaseData\" : \"base\", \"AbstractBaseDataT1\" : 0, \"AbstractBaseDataT2.Name\" : \"Bob\" }, { \"NestedGenericClass1T1\" : 1, \"NestedGenericClass1T2.Name\" : \"Alice\" }, { \"NestedGenericClass2T1\" : 0, \"NestedGenericClass2T2.Name\" : \"John\" }] }")]
        public void Inheritance_generics_mixed_types()
        {
            _ = Builders<NestedGenericClass2<EnumInt32, Person>>.Filter.Eq(t => t.AbstractBaseData, "base") &
                Builders<NestedGenericClass2<EnumInt32, Person>>.Filter.Eq(t => t.AbstractBaseDataT1, EnumInt32.Value0) &
                Builders<NestedGenericClass2<EnumInt32, Person>>.Filter.Eq(t => t.AbstractBaseDataT2.Name, "Bob") |
                Builders<NestedGenericClass2<EnumInt32, Person>>.Filter.Eq(t => t.NestedGenericClass1T1, EnumInt32.Value1) &
                Builders<NestedGenericClass2<EnumInt32, Person>>.Filter.Eq(t => t.NestedGenericClass1T2.Name, "Alice") |
                Builders<NestedGenericClass2<EnumInt32, Person>>.Filter.Eq(t => t.NestedGenericClass2T1, EnumInt32.Value0) &
                Builders<NestedGenericClass2<EnumInt32, Person>>.Filter.Eq(t => t.NestedGenericClass2T2.Name, "John");
        }

        [BuildersMQL("{ \"IntProp\" : 123 }")]
        public void Properties_with_intefaces_and_indexers_should_be_ingored()
        {
            _ = Builders<ClassWithNonTrivialProperties>.Filter.Eq(t => t.IntProp, 123);
        }

        [BuildersMQL("{ \"IntProp\" : 123 }")]
        public void Properties_with_bson_types_should_work()
        {
            _ = Builders<ClassWithBsonTypes>.Filter.Eq(t => t.IntProp, 123);
        }
    }
}
