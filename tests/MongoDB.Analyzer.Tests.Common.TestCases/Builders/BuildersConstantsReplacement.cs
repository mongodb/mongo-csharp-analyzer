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

using System.Drawing;
using MongoDB.Analyzer.Tests.Common.DataModel;
using MongoDB.Driver;

namespace MongoDB.Analyzer.Tests.Common.TestCases.Builders
{
    public sealed class BuildersConstantsReplacement : TestCasesBase
    {
        [BuildersMQL("{ \"$or\" : [{ \"Name\" : \"BBB\" }, { \"Name\" : \"BBB\" }, { \"Name\" : \"BBB\" }, { \"Vehicle.VehicleType.Type\" : 0 }, { \"Vehicle.VehicleType.MPG\" : 234.43199999999999 }] }")]
        public void Const_values_defined_in_regular_class_in_expression()
        {
            _ = Builders<Person>.Filter.Eq(p => p.Name, ConstantsHolder.ConstantString) |
                Builders<Person>.Filter.Eq(p => p.Name, DataModel.ConstantsHolder.ConstantString) |
                Builders<Person>.Filter.Eq(p => p.Name, MongoDB.Analyzer.Tests.Common.DataModel.ConstantsHolder.ConstantString) |
                Builders<Person>.Filter.Eq(p => p.Vehicle.VehicleType.Type, ConstantsHolder.ConstantEnum) |
                Builders<Person>.Filter.Eq(p => p.Vehicle.VehicleType.MPG, ConstantsHolder.ConstantDouble);
        }

        [BuildersMQL("{ \"$or\" : [{ \"SiblingsCount\" : StaticHolder.ReadonlyByte }, { \"SiblingsCount\" : StaticHolder.ReadonlyShort }, { \"SiblingsCount\" : StaticHolder.ReadonlyInt }, { \"TicksSinceBirth\" : NumberLong(StaticHolder.ReadonlyLong) }, { \"Name\" : StaticHolder.ReadonlyString }, { \"Name\" : DataModel.StaticHolder.ReadonlyString }, { \"Name\" : DataModel.StaticHolder.ReadonlyString2 }, { \"Name\" : MongoDB.Analyzer.Tests.Common.DataModel.StaticHolder.ReadonlyString }, { \"Vehicle.VehicleType.Type\" : StaticHolder.ReadonlyEnum }, { \"Vehicle.VehicleType.MPG\" : StaticHolder.ReadonlyDouble }] }")]
        public void Readonly_values_defined_in_static_class_in_expression()
        {
            _ = Builders<Person>.Filter.Eq(p => p.SiblingsCount, StaticHolder.ReadonlyByte) |
                Builders<Person>.Filter.Eq(p => p.SiblingsCount, StaticHolder.ReadonlyShort) |
                Builders<Person>.Filter.Eq(p => p.SiblingsCount, StaticHolder.ReadonlyInt) |
                Builders<Person>.Filter.Eq(p => p.TicksSinceBirth, StaticHolder.ReadonlyLong) |
                Builders<Person>.Filter.Eq(p => p.Name, StaticHolder.ReadonlyString) |
                Builders<Person>.Filter.Eq(p => p.Name, DataModel.StaticHolder.ReadonlyString) |
                Builders<Person>.Filter.Eq(p => p.Name, DataModel.StaticHolder.ReadonlyString2) |
                Builders<Person>.Filter.Eq(p => p.Name, MongoDB.Analyzer.Tests.Common.DataModel.StaticHolder.ReadonlyString) |
                Builders<Person>.Filter.Eq(p => p.Vehicle.VehicleType.Type, StaticHolder.ReadonlyEnum) |
                Builders<Person>.Filter.Eq(p => p.Vehicle.VehicleType.MPG, StaticHolder.ReadonlyDouble);
        }

        [BuildersMQL("{ \"$or\" : [{ \"SiblingsCount\" : StaticHolder.PropByte }, { \"SiblingsCount\" : StaticHolder.PropShort }, { \"SiblingsCount\" : StaticHolder.PropInt }, { \"TicksSinceBirth\" : NumberLong(StaticHolder.PropLong) }, { \"Name\" : StaticHolder.PropString }, { \"Name\" : DataModel.StaticHolder.PropString }, { \"Name\" : DataModel.StaticHolder.PropString2 }, { \"Name\" : MongoDB.Analyzer.Tests.Common.DataModel.StaticHolder.PropString2 }, { \"Vehicle.VehicleType.Type\" : StaticHolder.PropEnum }, { \"Vehicle.VehicleType.MPG\" : StaticHolder.PropDouble }] }")]
        public void Simple_typed_properties_defined_in_static_class_in_expression()
        {
            _ = Builders<Person>.Filter.Eq(p => p.SiblingsCount, StaticHolder.PropByte) |
                Builders<Person>.Filter.Eq(p => p.SiblingsCount, StaticHolder.PropShort) |
                Builders<Person>.Filter.Eq(p => p.SiblingsCount, StaticHolder.PropInt) |
                Builders<Person>.Filter.Eq(p => p.TicksSinceBirth, StaticHolder.PropLong) |
                Builders<Person>.Filter.Eq(p => p.Name, StaticHolder.PropString) |
                Builders<Person>.Filter.Eq(p => p.Name, DataModel.StaticHolder.PropString) |
                Builders<Person>.Filter.Eq(p => p.Name, DataModel.StaticHolder.PropString2) |
                Builders<Person>.Filter.Eq(p => p.Name, MongoDB.Analyzer.Tests.Common.DataModel.StaticHolder.PropString2) |
                Builders<Person>.Filter.Eq(p => p.Vehicle.VehicleType.Type, StaticHolder.PropEnum) |
                Builders<Person>.Filter.Eq(p => p.Vehicle.VehicleType.MPG, StaticHolder.PropDouble);
        }

        [BuildersMQL("{ \"Name\" : StaticHolder.Person.Name, \"Vehicle.LicenceNumber\" : StaticHolder.Person.Vehicle.LicenceNumber, \"Vehicle.VehicleType.MPG\" : StaticHolder.Person.Vehicle.VehicleType.MPG }")]
        public void Custom_typed_properties_defined_in_static_class_in_expression()
        {
            _ = Builders<Person>.Filter.Eq(p => p.Name, StaticHolder.Person.Name) &
                Builders<Person>.Filter.Eq(p => p.Vehicle.LicenceNumber, StaticHolder.Person.Vehicle.LicenceNumber) &
                Builders<Person>.Filter.Eq(p => p.Vehicle.VehicleType.MPG, StaticHolder.Person.Vehicle.VehicleType.MPG);
        }

        [BuildersMQL("{ \"Name\" : name, \"Vehicle.VehicleType.Type\" : vehicleTypeEnum, \"Vehicle.VehicleType.MPG\" : mpg }")]
        public void Simple_local_variables_referenced_in_expression()
        {
            var name = "Alice";
            var vehicleTypeEnum = VehicleTypeEnum.Bus;
            var mpg = 123.123;

            _ = Builders<Person>.Filter.Eq(p => p.Name, name) &
                Builders<Person>.Filter.Eq(p => p.Vehicle.VehicleType.Type, vehicleTypeEnum) &
                Builders<Person>.Filter.Eq(p => p.Vehicle.VehicleType.MPG, mpg);
        }

        [BuildersMQL("{ \"$or\" : [{ \"Name\" : personA.Name }, { \"Name\" : personB.Name }, { \"Vehicle.VehicleType.MPG\" : personA.Vehicle.VehicleType.MPG }, { \"Vehicle.VehicleType.MPG\" : personB.Vehicle.VehicleType.MPG }, { \"Vehicle.VehicleType.Type\" : personA.Vehicle.VehicleType.Type }, { \"Vehicle.VehicleType.Type\" : personB.Vehicle.VehicleType.Type }, { \"TicksSinceBirth\" : NumberLong(personA.TicksSinceBirth) }, { \"TicksSinceBirth\" : NumberLong(personB.TicksSinceBirth) }, { \"SiblingsCount\" : personA.SiblingsCount }, { \"SiblingsCount\" : personB.SiblingsCount }] }")]
        public void Custom_types_variables_referenced_in_expression()
        {
            var personA = new Person();
            var personB = new Person();

            _ = Builders<Person>.Filter.Eq(p => p.Name, personA.Name) |
                Builders<Person>.Filter.Eq(p => p.Name, personB.Name) |
                Builders<Person>.Filter.Eq(p => p.Vehicle.VehicleType.MPG, personA.Vehicle.VehicleType.MPG) |
                Builders<Person>.Filter.Eq(p => p.Vehicle.VehicleType.MPG, personB.Vehicle.VehicleType.MPG) |
                Builders<Person>.Filter.Eq(p => p.Vehicle.VehicleType.Type, personA.Vehicle.VehicleType.Type) |
                Builders<Person>.Filter.Eq(p => p.Vehicle.VehicleType.Type, personB.Vehicle.VehicleType.Type) |
                Builders<Person>.Filter.Eq(p => p.TicksSinceBirth, personA.TicksSinceBirth) |
                Builders<Person>.Filter.Eq(p => p.TicksSinceBirth, personB.TicksSinceBirth) |
                Builders<Person>.Filter.Eq(p => p.SiblingsCount, personA.SiblingsCount) |
                Builders<Person>.Filter.Eq(p => p.SiblingsCount, personB.SiblingsCount);
        }

        [BuildersMQL("{ \"$or\" : [{ \"Name\" : GetName() }, { \"Name\" : GetName(\"Smith\", 12) }, { \"Vehicle.VehicleType.MPG\" : GetMpg() }, { \"TicksSinceBirth\" : NumberLong(GetPerson(13).TicksSinceBirth) }, { \"Vehicle.VehicleType.Type\" : GetPerson().Vehicle.VehicleType.Type }, { \"Vehicle.VehicleType.Type\" : GetVehicleTypeEnum() }, { \"Vehicle.VehicleType.Type\" : GetVehicleTypeEnum(VehicleTypeEnum.Bus) }] }")]
        public void Method_referenced_in_expression()
        {
            _ = Builders<Person>.Filter.Eq(p => p.Name, GetName()) |
                Builders<Person>.Filter.Eq(p => p.Name, GetName("Smith", 12)) |
                Builders<Person>.Filter.Eq(p => p.Vehicle.VehicleType.MPG, GetMpg()) |
                Builders<Person>.Filter.Eq(p => p.TicksSinceBirth, GetPerson(13).TicksSinceBirth) |
                Builders<Person>.Filter.Eq(p => p.Vehicle.VehicleType.Type, GetPerson().Vehicle.VehicleType.Type) |
                Builders<Person>.Filter.Eq(p => p.Vehicle.VehicleType.Type, GetVehicleTypeEnum()) |
                Builders<Person>.Filter.Eq(p => p.Vehicle.VehicleType.Type, GetVehicleTypeEnum(VehicleTypeEnum.Bus));
        }

        [BuildersMQL("{ \"$or\" : [{ \"Name\" : objA.GetString() }, { \"Name\" : objB.GetString() }, { \"Address.City\" : objA.GetString(\"AddressA\", 123) }, { \"Address.City\" : objB.GetString(\"AddressB\", 321) }, { \"Address.City\" : objA.GetSelf().GetSelf().GetString(\"Address\", 123) }, { \"Vehicle.VehicleType.VehicleMake.Name\" : objA.GetSelf().GetSelf(\"random\").GetSelf().GetPerson(\"Bob\").Vehicle.VehicleType.VehicleMake.Name }, { \"Vehicle.VehicleType.Type\" : objA.GetSelf().GetPerson().Vehicle.VehicleType.Type }, { \"Vehicle.VehicleType.Type\" : objB.GetSelf().GetSelf().GetPerson(\"NewName\").Vehicle.VehicleType.Type }] }")]
        public void Method_from_local_variable_referenced_in_expression()
        {
            var objA = new ClassWithMethods();
            var objB = new ClassWithMethods();

            _ = Builders<Person>.Filter.Eq(p => p.Name, objA.GetString()) |
                Builders<Person>.Filter.Eq(p => p.Name, objB.GetString()) |
                Builders<Person>.Filter.Eq(p => p.Address.City, objA.GetString("AddressA", 123)) |
                Builders<Person>.Filter.Eq(p => p.Address.City, objB.GetString("AddressB", 321)) |
                Builders<Person>.Filter.Eq(p => p.Address.City, objA.GetSelf().GetSelf().GetString("Address", 123)) |
                Builders<Person>.Filter.Eq(p => p.Vehicle.VehicleType.VehicleMake.Name, objA.GetSelf().GetSelf("random").GetSelf().GetPerson("Bob").Vehicle.VehicleType.VehicleMake.Name) |
                Builders<Person>.Filter.Eq(p => p.Vehicle.VehicleType.Type, objA.GetSelf().GetPerson().Vehicle.VehicleType.Type) |
                Builders<Person>.Filter.Eq(p => p.Vehicle.VehicleType.Type, objB.GetSelf().GetSelf().GetPerson("NewName").Vehicle.VehicleType.Type);
        }

        [BuildersMQL("{ \"$or\" : [{ \"Nested.Nested.Point.X\" : Point.Empty.X }, { \"Nested.Point.Y\" : 123 }, { \"Nested.Point.X\" : GetDrawingPoint().X }] }")]
        public void Query_containing_system_types()
        {
            _ = Builders<ClassWithSystemTypes>.Filter.Eq(o => o.Nested.Nested.Point.X, Point.Empty.X) |
                Builders<ClassWithSystemTypes>.Filter.Eq(o => o.Nested.Point.Y, 123) |
                Builders<ClassWithSystemTypes>.Filter.Eq(o => o.Nested.Point.X, GetDrawingPoint().X);
        }

        [BuildersMQL("{ \"$or\" : [{ \"SiblingsCount\" : byteConstant }, { \"SiblingsCount\" : 0 }, { \"SiblingsCount\" : intConstant1 }, { \"SiblingsCount\" : 1 }, { \"SiblingsCount\" : intConstant2 }, { \"SiblingsCount\" : 2 }, { \"TicksSinceBirth\" : NumberLong(longConstant1) }, { \"TicksSinceBirth\" : NumberLong(3) }, { \"TicksSinceBirth\" : NumberLong(longConstant2) }, { \"Name\" : stringConstant1 }, { \"Name\" : \"s__5\" }, { \"Name\" : stringConstant2 }, { \"Name\" : \"s__6\" }] }")]
        public void Colliding_constants_and_variables()
        {
            const byte byteConstant = 0;
            const int intConstant1 = 1;
            const int intConstant2 = 2;
            const long longConstant1 = 3;
            const long longConstant2 = 4;
            const string stringConstant1 = "s__5";
            const string stringConstant2 = "s__6";

            _ = Builders<Person>.Filter.Eq(p => p.SiblingsCount, byteConstant) |
                Builders<Person>.Filter.Eq(p => p.SiblingsCount, 0) |
                Builders<Person>.Filter.Eq(p => p.SiblingsCount, intConstant1) |
                Builders<Person>.Filter.Eq(p => p.SiblingsCount, 1) |
                Builders<Person>.Filter.Eq(p => p.SiblingsCount, intConstant2) |
                Builders<Person>.Filter.Eq(p => p.SiblingsCount, 2) |
                Builders<Person>.Filter.Eq(p => p.TicksSinceBirth, longConstant1) |
                Builders<Person>.Filter.Eq(p => p.TicksSinceBirth, 3L) |
                Builders<Person>.Filter.Eq(p => p.TicksSinceBirth, longConstant2) |
                Builders<Person>.Filter.Eq(p => p.Name, stringConstant1) |
                Builders<Person>.Filter.Eq(p => p.Name, "s__5") |
                Builders<Person>.Filter.Eq(p => p.Name, stringConstant2) |
                Builders<Person>.Filter.Eq(p => p.Name, "s__6");
        }

        [BuildersMQL("{ \"$or\" : [{ \"Name\" : s__1 }, { \"Name\" : s__2 }, { \"Age\" : intConstant1 }, { \"Age\" : intConstant2 }, { \"Age\" : varConstant33 }, { \"Age\" : varConstant44 }, { \"Name\" : \"s__1\" }, { \"Name\" : \"s__2\" }, { \"Name\" : \"3\" }, { \"Name\" : \"4\" }, { \"Name\" : \"5\" }, { \"Name\" : \"6\" }] }")]
        public void Constant_string_containing_const_remapped_values()
        {
            const int intConstant1 = 1;
            const int intConstant2 = 2;
            int varConstant33 = 33;
            int varConstant44 = 44;
            string s__1 = "s__1";
            string s__2 = "s__2";

            _ = Builders<User>.Filter.Eq(u => u.Name, s__1) |
                Builders<User>.Filter.Eq(u => u.Name, s__2) |
                Builders<User>.Filter.Eq(u => u.Age, intConstant1) |
                Builders<User>.Filter.Eq(u => u.Age, intConstant2) |
                Builders<User>.Filter.Eq(u => u.Age, varConstant33) |
                Builders<User>.Filter.Eq(u => u.Age, varConstant44) |
                Builders<User>.Filter.Eq(u => u.Name, "s__1") |
                Builders<User>.Filter.Eq(u => u.Name, "s__2") |
                Builders<User>.Filter.Eq(u => u.Name, "3") |
                Builders<User>.Filter.Eq(u => u.Name, "4") |
                Builders<User>.Filter.Eq(u => u.Name, "5") |
                Builders<User>.Filter.Eq(u => u.Name, "6");
        }

        [BuildersMQL("{ \"$or\" : [{ \"Name\" : stringParam1 }, { \"SiblingsCount\" : intParam }, { \"SiblingsCount\" : longParam }, { \"Vehicle.VehicleType.Type\" : enumParam }] }")]
        public void Methods_with_simple_type_parameters(int intParam, string stringParam1, VehicleTypeEnum enumParam, long longParam)
        {
            _ = Builders<Person>.Filter.Eq(p => p.Name, stringParam1) |
                Builders<Person>.Filter.Eq(p => p.SiblingsCount, intParam) |
                Builders<Person>.Filter.Eq(p => p.SiblingsCount, longParam) |
                Builders<Person>.Filter.Eq(p => p.Vehicle.VehicleType.Type, enumParam);
        }

        [BuildersMQL("{ \"$or\" : [{ \"Name\" : person.Name }, { \"SiblingsCount\" : person.SiblingsCount }, { \"Vehicle.VehicleType.Type\" : person.Vehicle.VehicleType.Type }] }")]
        public void Methods_with_custom_type_parameters(Person person)
        {
            _ = Builders<Person>.Filter.Eq(p => p.Name, person.Name) |
                Builders<Person>.Filter.Eq(p => p.SiblingsCount, person.SiblingsCount) |
                Builders<Person>.Filter.Eq(p => p.Vehicle.VehicleType.Type, person.Vehicle.VehicleType.Type);
        }

        [BuildersMQL("{ \"Name\" : genericType.DataT2, \"Address.City\" : genericType.DataT3.Address.City, \"Vehicle.LicenceNumber\" : genericType.DataT3.Vehicle.LicenceNumber, \"Vehicle.VehicleType.Type\" : genericType.DataT4 }")]
        public void Methods_with_generic_type_parameters(MultipleTypeGeneric<int, string, Person, VehicleTypeEnum> genericType)
        {
            _ = Builders<Person>.Filter.Eq(t => t.Name, genericType.DataT2) &
                Builders<Person>.Filter.Eq(t => t.Address.City, genericType.DataT3.Address.City) &
                Builders<Person>.Filter.Eq(t => t.Vehicle.LicenceNumber, genericType.DataT3.Vehicle.LicenceNumber) &
                Builders<Person>.Filter.Eq(t => t.Vehicle.VehicleType.Type, genericType.DataT4);
        }

        [BuildersMQL("{ \"Name\" : _fieldPerson.Name, \"LastName\" : _fieldString, \"Address.City\" : _fieldPerson.Address.City, \"Vehicle.VehicleType.MPG\" : _fieldDouble }")]
        public void Local_field_reference()
        {
            _ = Builders<Person>.Filter.Eq(t => t.Name, _fieldPerson.Name) &
                Builders<Person>.Filter.Eq(t => t.LastName, _fieldString) &
                Builders<Person>.Filter.Eq(t => t.Address.City, _fieldPerson.Address.City) &
                Builders<Person>.Filter.Eq(t => t.Vehicle.VehicleType.MPG, _fieldDouble);
        }

        [BuildersMQL("{ \"Name\" : PropertyPerson.Name, \"LastName\" : PropertyString, \"Address.City\" : PropertyPerson.Address.City, \"Vehicle.VehicleType.MPG\" : PropertyDouble }")]
        public void Local_property_reference()
        {
            _ = Builders<Person>.Filter.Eq(t => t.Name, PropertyPerson.Name) &
                Builders<Person>.Filter.Eq(t => t.LastName, PropertyString) &
                Builders<Person>.Filter.Eq(t => t.Address.City, PropertyPerson.Address.City) &
                Builders<Person>.Filter.Eq(t => t.Vehicle.VehicleType.MPG, PropertyDouble);
        }

        [BuildersMQL("{ \"SiblingsCount\" : 10 }")]
        public void Constants_expressions_referenced_in_expression_1()
        {
            _ = Builders<Person>.Filter.Eq(p => p.SiblingsCount, 1 + 2 + 3 + 4);
        }

        [BuildersMQL("{ \"Name\" : \"Joe\" }")]
        public void Constants_expressions_referenced_in_expression_2()
        {
            _ = Builders<Person>.Filter.Eq(p => p.Name, "Jo" + "e");
        }

        [BuildersMQL("{ \"SiblingsCount\" : count1 + count2 }")]
        public void Dynamic_expressions_referenced_in_expression_1()
        {
            var count1 = 1;
            var count2 = 2;
            _ = Builders<Person>.Filter.Eq(p => p.SiblingsCount, count1 + count2);
        }

        [BuildersMQL("{ \"SiblingsCount\" : count1 + (count2 + 2) >> 12 + count3 - 123 * count4 + 0 - count5 }")]
        public void Dynamic_expressions_referenced_in_expression_2()
        {
            var count1 = 1;
            var count2 = 2;
            var count3 = 3;
            var count4 = 4;
            var count5 = 5;
            _ = Builders<Person>.Filter.Eq(p => p.SiblingsCount, count1 + (count2 + 2) >> 12 + count3 - 123 * count4 + 0 - count5);
        }

        [BuildersMQL("{ \"SiblingsCount\" : count1 + _fieldDouble + PropertyDouble - 0 + 9 + _fieldPerson.SiblingsCount }")]
        public void Dynamic_expressions_referenced_in_expression_3()
        {
            var count1 = 1;
            _ = Builders<Person>.Filter.Eq(p => p.SiblingsCount, count1 + _fieldDouble + PropertyDouble - 0 + 9 + _fieldPerson.SiblingsCount);
        }

        [BuildersMQL("{ \"SiblingsCount\" : Add(Add(count1 + 2, count2), 2) + Add(count3, count3) + 2 }")]
        public void Dynamic_expressions_referenced_in_expression_4()
        {
            // Binary expression is top most
            var count1 = 1;
            var count2 = 2;
            var count3 = 2;
            _ = Builders<Person>.Filter.Eq(p => p.SiblingsCount, Add(Add(count1 + 2, count2), 2) + Add(count3, count3) + 2);
        }

        [BuildersMQL("{ \"SiblingsCount\" : Add(Add(count1 + 2, count2) + Add(count3, count3), 123) }")]
        public void Dynamic_expressions_referenced_in_expression_5()
        {
            // Method invocation is top most
            var count1 = 1;
            var count2 = 2;
            var count3 = 2;
            _ = Builders<Person>.Filter.Eq(p => p.SiblingsCount, Add(Add(count1 + 2, count2) + Add(count3, count3), 123));
        }

        [BuildersMQL("{ \"SiblingsCount\" : GetThis().Add(Add(count1 + 2, count2) + GetThis().Add(count3, count3), GetThis()._fieldInt) }")]
        public void Dynamic_expressions_referenced_in_expression_6()
        {
            // Method invocation is top most
            var count1 = 1;
            var count2 = 2;
            var count3 = 2;
            _ = Builders<Person>.Filter.Eq(p => p.SiblingsCount, GetThis().Add(Add(count1 + 2, count2) + GetThis().Add(count3, count3), GetThis()._fieldInt));
        }

        [BuildersMQL("{ \"Name\" : GetThis().Concate(Concate(str1 + \"123\", str2) + GetThis().Concate(str2, GetThis().PropertyString), GetThis()._fieldString) }")]
        public void Dynamic_expressions_referenced_in_expression_7()
        {
            // Method invocation is top most
            var str1 = "str1";
            var str2 = "str2";
            _ = Builders<Person>.Filter.Eq(p => p.Name, GetThis().Concate(Concate(str1 + "123", str2) + GetThis().Concate(str2, GetThis().PropertyString), GetThis()._fieldString));
        }

        private int _fieldInt = 222;
        private double _fieldDouble = 333.3;
        private double PropertyDouble { get; } = 333.3;

        private string _fieldString = "fieldString";
        private string PropertyString { get; } = "PropertyString";

        private Person _fieldPerson = new() { Name = "fieldPersonName" };
        private Person PropertyPerson { get; } = new Person() { Name = "propertyPersonName" };

        private Person GetPerson() => new();
        private Person GetPerson(long ticksSinceBirth) => new() { TicksSinceBirth = ticksSinceBirth };

        private string GetName() => "Alice";
        private string GetName(string suffix, int count) => "Alice " + suffix + count;

        private double GetMpg() => 123.123;
        private VehicleTypeEnum GetVehicleTypeEnum() => VehicleTypeEnum.Motorcylce;
        private VehicleTypeEnum GetVehicleTypeEnum(VehicleTypeEnum vehicleTypeEnum) => VehicleTypeEnum.Motorcylce;

        private Point GetDrawingPoint() => new(1, 2);

        private BuildersConstantsReplacement GetThis() => this;

        private int Add(int a, int b) => a + b;
        private string Concate(string a, string b) => a + b;
    }
}
