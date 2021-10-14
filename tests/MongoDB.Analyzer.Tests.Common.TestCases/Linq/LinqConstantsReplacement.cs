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
using System.Linq;
using MongoDB.Analyzer.Tests.Common.DataModel;
using MongoDB.Driver.Linq;

namespace MongoDB.Analyzer.Tests.Common.TestCases.Linq
{
    public sealed class LinqConstantsReplacement : TestCasesBase
    {
        [MQL("aggregate([{ \"$match\" : { \"$or\" : [{ \"Name\" : \"BBB\" }, { \"Name\" : \"BBB\" }, { \"Name\" : \"BBB\" }, { \"Vehicle.VehicleType.Type\" : 0 }, { \"Vehicle.VehicleType.MPG\" : 234.43199999999999 }] } }])")]
        public void Const_values_defined_in_regular_class_in_expression()
        {
            _ = GetMongoQueryable<Person>().Where(p =>
                p.Name == ConstantsHolder.ConstantString ||
                p.Name == DataModel.ConstantsHolder.ConstantString ||
                p.Name == MongoDB.Analyzer.Tests.Common.DataModel.ConstantsHolder.ConstantString ||
                p.Vehicle.VehicleType.Type == ConstantsHolder.ConstantEnum ||
                p.Vehicle.VehicleType.MPG == ConstantsHolder.ConstantDouble);
        }

        [MQL("aggregate([{ \"$match\" : { \"$or\" : [{ \"SiblingsCount\" : StaticHolder.ReadonlyByte }, { \"SiblingsCount\" : StaticHolder.ReadonlyShort }, { \"SiblingsCount\" : StaticHolder.ReadonlyInt }, { \"TicksSinceBirth\" : NumberLong(StaticHolder.ReadonlyLong) }, { \"Name\" : StaticHolder.ReadonlyString }, { \"Name\" : DataModel.StaticHolder.ReadonlyString }, { \"Name\" : DataModel.StaticHolder.ReadonlyString2 }, { \"Name\" : MongoDB.Analyzer.Tests.Common.DataModel.StaticHolder.ReadonlyString }, { \"Vehicle.VehicleType.Type\" : StaticHolder.ReadonlyEnum }, { \"Vehicle.VehicleType.MPG\" : StaticHolder.ReadonlyDouble }] } }])")]
        public void Readonly_values_defined_in_static_class_in_expression()
        {
            _ = GetMongoQueryable<Person>().Where(p =>
                p.SiblingsCount == StaticHolder.ReadonlyByte ||
                p.SiblingsCount == StaticHolder.ReadonlyShort ||
                p.SiblingsCount == StaticHolder.ReadonlyInt ||
                p.TicksSinceBirth == StaticHolder.ReadonlyLong ||
                p.Name == StaticHolder.ReadonlyString ||
                p.Name == DataModel.StaticHolder.ReadonlyString ||
                p.Name == DataModel.StaticHolder.ReadonlyString2 ||
                p.Name == MongoDB.Analyzer.Tests.Common.DataModel.StaticHolder.ReadonlyString ||
                p.Vehicle.VehicleType.Type == StaticHolder.ReadonlyEnum ||
                p.Vehicle.VehicleType.MPG == StaticHolder.ReadonlyDouble);
        }

        [MQL("aggregate([{ \"$match\" : { \"$or\" : [{ \"SiblingsCount\" : StaticHolder.PropByte }, { \"SiblingsCount\" : StaticHolder.PropShort }, { \"SiblingsCount\" : StaticHolder.PropInt }, { \"TicksSinceBirth\" : NumberLong(StaticHolder.PropLong) }, { \"Name\" : StaticHolder.PropString }, { \"Name\" : DataModel.StaticHolder.PropString }, { \"Name\" : DataModel.StaticHolder.PropString2 }, { \"Name\" : MongoDB.Analyzer.Tests.Common.DataModel.StaticHolder.PropString2 }, { \"Vehicle.VehicleType.Type\" : StaticHolder.PropEnum }, { \"Vehicle.VehicleType.MPG\" : StaticHolder.PropDouble }] } }])")]
        public void Simple_typed_properties_defined_in_static_class_in_expression()
        {
            _ = GetMongoQueryable<Person>().Where(p =>
                p.SiblingsCount == StaticHolder.PropByte ||
                p.SiblingsCount == StaticHolder.PropShort ||
                p.SiblingsCount == StaticHolder.PropInt ||
                p.TicksSinceBirth == StaticHolder.PropLong ||
                p.Name == StaticHolder.PropString ||
                p.Name == DataModel.StaticHolder.PropString ||
                p.Name == DataModel.StaticHolder.PropString2 ||
                p.Name == MongoDB.Analyzer.Tests.Common.DataModel.StaticHolder.PropString2 ||
                p.Vehicle.VehicleType.Type == StaticHolder.PropEnum ||
                p.Vehicle.VehicleType.MPG == StaticHolder.PropDouble);
        }

        [MQL("aggregate([{ \"$match\" : { \"Name\" : StaticHolder.Person.Name, \"Vehicle.LicenceNumber\" : StaticHolder.Person.Vehicle.LicenceNumber, \"Vehicle.VehicleType.MPG\" : StaticHolder.Person.Vehicle.VehicleType.MPG } }])")]
        public void Custom_typed_properties_defined_in_static_class_in_expression()
        {
            _ = GetMongoQueryable<Person>().Where(p =>
                p.Name == StaticHolder.Person.Name &&
                p.Vehicle.LicenceNumber == StaticHolder.Person.Vehicle.LicenceNumber &&
                p.Vehicle.VehicleType.MPG == StaticHolder.Person.Vehicle.VehicleType.MPG);
        }

        [MQL("aggregate([{ \"$match\" : { \"Name\" : name, \"Vehicle.VehicleType.Type\" : vehicleTypeEnum, \"Vehicle.VehicleType.MPG\" : mpg } }])")]
        public void Simple_local_variables_referenced_in_expression()
        {
            var name = "Alice";
            var vehicleTypeEnum = VehicleTypeEnum.Bus;
            var mpg = 123.123;

            _ = GetMongoQueryable<Person>().Where(p =>
                p.Name == name &&
                p.Vehicle.VehicleType.Type == vehicleTypeEnum &&
                p.Vehicle.VehicleType.MPG == mpg);
        }

        [MQL("aggregate([{ \"$match\" : { \"Name\" : personA.Name, \"Vehicle.VehicleType.Type\" : personA.Vehicle.VehicleType.Type, \"Vehicle.VehicleType.MPG\" : personA.Vehicle.VehicleType.MPG } }, { \"$match\" : { \"Name\" : personB.Name, \"Vehicle.VehicleType.Type\" : personB.Vehicle.VehicleType.Type, \"SiblingsCount\" : personB.SiblingsCount, \"TicksSinceBirth\" : NumberLong(personB.TicksSinceBirth) } }])")]
        public void Custom_types_variables_referenced_in_expression()
        {
            var personA = new Person();
            var personB = new Person();

            _ = GetMongoQueryable<Person>()
                .Where(p =>
                    p.Name == personA.Name &&
                    p.Vehicle.VehicleType.Type == personA.Vehicle.VehicleType.Type &&
                    p.Vehicle.VehicleType.MPG == personA.Vehicle.VehicleType.MPG)
                .Where(p =>
                    p.Name == personB.Name &&
                    p.Vehicle.VehicleType.Type == personB.Vehicle.VehicleType.Type &&
                    p.SiblingsCount == personB.SiblingsCount &&
                    p.TicksSinceBirth == personB.TicksSinceBirth);
        }

        [MQL("aggregate([{ \"$match\" : { \"$or\" : [{ \"$or\" : [{ \"Name\" : GetName() }, { \"Name\" : GetName(\"Smith\", 12) }], \"Vehicle.VehicleType.MPG\" : GetMpg(), \"TicksSinceBirth\" : NumberLong(GetPerson(13).TicksSinceBirth), \"Vehicle.VehicleType.Type\" : GetPerson().Vehicle.VehicleType.Type }, { \"Vehicle.VehicleType.Type\" : GetVehicleTypeEnum() }, { \"Vehicle.VehicleType.Type\" : GetVehicleTypeEnum(VehicleTypeEnum.Bus) }] } }])")]
        public void Method_referenced_in_expression()
        {
            _ = GetMongoQueryable<Person>()
                .Where(p =>
                    (p.Name == GetName() || p.Name == GetName("Smith", 12)) &&
                    p.Vehicle.VehicleType.MPG == GetMpg() &&
                    p.TicksSinceBirth == GetPerson(13).TicksSinceBirth &&
                    p.Vehicle.VehicleType.Type == GetPerson().Vehicle.VehicleType.Type ||
                    p.Vehicle.VehicleType.Type == GetVehicleTypeEnum() ||
                    p.Vehicle.VehicleType.Type == GetVehicleTypeEnum(VehicleTypeEnum.Bus));
        }

        [MQL("aggregate([{ \"$match\" : { \"$or\" : [{ \"Name\" : objA.GetString() }, { \"Name\" : objB.GetString() }, { \"Address.City\" : objA.GetString(\"AddressA\", 123) }, { \"Address.City\" : objB.GetString(\"AddressB\", 321) }, { \"Address.City\" : objA.GetSelf().GetSelf().GetString(\"Address\", 123) }, { \"Vehicle.VehicleType.VehicleMake.Name\" : objA.GetSelf().GetSelf(\"random\").GetSelf().GetPerson(\"Bob\").Vehicle.VehicleType.VehicleMake.Name }, { \"Vehicle.VehicleType.Type\" : objA.GetSelf().GetPerson().Vehicle.VehicleType.Type }, { \"Vehicle.VehicleType.Type\" : objB.GetSelf().GetSelf().GetPerson(\"NewName\").Vehicle.VehicleType.Type }] } }])")]
        public void Method_from_local_variable_referenced_in_expression()
        {
            var objA = new ClassWithMethods();
            var objB = new ClassWithMethods();

            _ = GetMongoQueryable<Person>().Where(p =>
                p.Name == objA.GetString() ||
                p.Name == objB.GetString() ||
                p.Address.City == objA.GetString("AddressA", 123) ||
                p.Address.City == objB.GetString("AddressB", 321) ||
                p.Address.City == objA.GetSelf().GetSelf().GetString("Address", 123) ||
                p.Vehicle.VehicleType.VehicleMake.Name == objA.GetSelf().GetSelf("random").GetSelf().GetPerson("Bob").Vehicle.VehicleType.VehicleMake.Name ||
                p.Vehicle.VehicleType.Type == objA.GetSelf().GetPerson().Vehicle.VehicleType.Type ||
                p.Vehicle.VehicleType.Type == objB.GetSelf().GetSelf().GetPerson("NewName").Vehicle.VehicleType.Type);
        }

        [MQL("aggregate([{ \"$match\" : { \"$or\" : [{ \"Point.X\" : 1 }, { \"Rectangle.Width\" : 123 }] } }])")]
        public void Poco_containing_system_types()
        {
            _ = GetMongoQueryable<ClassWithSystemTypes>().Where(o =>
                o.Point.X == 1 || o.Rectangle.Width == 123);
        }

        [MQL("aggregate([{ \"$match\" : { \"$or\" : [{ \"Nested.Nested.Point.X\" : Point.Empty.X }, { \"Nested.Point.Y\" : 123 }, { \"Nested.Point.X\" : GetDrawingPoint().X }] } }])")]
        public void Query_containing_system_types()
        {
            _ = GetMongoQueryable<ClassWithSystemTypes>().Where(o =>
                o.Nested.Nested.Point.X == Point.Empty.X ||
                o.Nested.Point.Y == 123 ||
                o.Nested.Point.X == GetDrawingPoint().X);
        }

        //[MQL("aggregate([{ \"$match\" : { \"$or\" : [{ \"Age\" : byteConstant }, { \"Age\" : 0 }, { \"Age\" : byteConstant58 }, { "Age" : byteConstant56 }, { "Age" : byteConstant59 }, { "Age" : byteConstant57 }, { "Name" : stringConstant }, { "Name" : "s__257" }] } }])")]
        [MQL("aggregate([{ \"$match\" : { \"$or\" : [{ \"SiblingsCount\" : byteConstant }, { \"SiblingsCount\" : 0 }, { \"SiblingsCount\" : intConstant1 }, { \"SiblingsCount\" : 1 }, { \"SiblingsCount\" : intConstant2 }, { \"SiblingsCount\" : 2 }, { \"TicksSinceBirth\" : NumberLong(longConstant1) }, { \"TicksSinceBirth\" : NumberLong(3) }, { \"TicksSinceBirth\" : NumberLong(longConstant2) }, { \"TicksSinceBirth\" : NumberLong(4) }, { \"Name\" : stringConstant1 }, { \"Name\" : \"s__5\" }, { \"Name\" : stringConstant2 }, { \"Name\" : \"s__6\" }] } }])")]
        public void Colliding_constants_and_variables()
        {
            const byte byteConstant = 0;
            const int intConstant1 = 1;
            const int intConstant2 = 2;
            const long longConstant1 = 3;
            const long longConstant2 = 4;
            const string stringConstant1 = "s__5";
            const string stringConstant2 = "s__6";

            _ = GetMongoQueryable<Person>().Where(p =>
                p.SiblingsCount == byteConstant ||
                p.SiblingsCount == 0 ||
                p.SiblingsCount == intConstant1 ||
                p.SiblingsCount == 1 ||
                p.SiblingsCount == intConstant2 ||
                p.SiblingsCount == 2 ||
                p.TicksSinceBirth == longConstant1 ||
                p.TicksSinceBirth == 3L ||
                p.TicksSinceBirth == longConstant2 ||
                p.TicksSinceBirth == 4L ||
                p.Name == stringConstant1 ||
                p.Name == "s__5" ||
                p.Name == stringConstant2 ||
                p.Name == "s__6");
        }

        [MQL("aggregate([{ \"$match\" : { \"$or\" : [{ \"Name\" : s__1 }, { \"Name\" : s__2 }, { \"Age\" : intConstant1 }, { \"Age\" : intConstant2 }, { \"Age\" : varConstant33 }, { \"Age\" : varConstant44 }, { \"Name\" : \"s__1\" }, { \"Name\" : \"s__2\" }, { \"Name\" : \"3\" }, { \"Name\" : \"4\" }, { \"Name\" : \"5\" }, { \"Name\" : \"6\" }] } }])")]
        public void Constant_string_containing_const_remapped_values()
        {
            const int intConstant1 = 1;
            const int intConstant2 = 2;
            int varConstant33 = 33;
            int varConstant44 = 44;
            string s__1 = "s__1";
            string s__2 = "s__2";

            _ = GetMongoQueryable<User>().Where(u =>
                u.Name == s__1 ||
                u.Name == s__2 ||
                u.Age == intConstant1 ||
                u.Age == intConstant2 ||
                u.Age == varConstant33 ||
                u.Age == varConstant44 ||
                u.Name == "s__1" ||
                u.Name == "s__2" ||
                u.Name == "3" ||
                u.Name == "4" ||
                u.Name == "5" ||
                u.Name == "6");
        }

        [MQL("aggregate([{ \"$match\" : { \"SiblingsCount\" : varConstant0, \"Vehicle.VehicleType.Type\" : varConstant0 } }])")]
        public void Constants_replacing_interfering_with_enum_default()
        {
            int varConstant0 = 0;

            _ = GetMongoQueryable<Person>().Where(p =>
                p.SiblingsCount == varConstant0 &&
                p.Vehicle.VehicleType.Type == VehicleTypeEnum.Bus);
        }

        [MQL("aggregate([{ \"$match\" : { \"$or\" : [{ \"Name\" : stringParam1 }, { \"SiblingsCount\" : intParam }, { \"SiblingsCount\" : longParam }, { \"Vehicle.VehicleType.Type\" : enumParam }] } }])")]
        public void Methods_with_simple_type_parameters(int intParam, string stringParam1, VehicleTypeEnum enumParam, long longParam)
        {
            _ = GetMongoQueryable<Person>().Where(p =>
                p.Name == stringParam1 ||
                p.SiblingsCount == intParam ||
                p.SiblingsCount == longParam ||
                p.Vehicle.VehicleType.Type == enumParam);
        }

        [MQL("aggregate([{ \"$match\" : { \"$or\" : [{ \"Name\" : person.Name }, { \"SiblingsCount\" : person.SiblingsCount }, { \"Vehicle.VehicleType.Type\" : person.Vehicle.VehicleType.Type }] } }])")]
        public void Methods_with_custom_type_parameters(Person person)
        {
            _ = GetMongoQueryable<Person>().Where(p =>
                p.Name == person.Name ||
                p.SiblingsCount == person.SiblingsCount ||
                p.Vehicle.VehicleType.Type == person.Vehicle.VehicleType.Type);
        }

        [MQL("aggregate([{ \"$match\" : { \"Name\" : genericType.DataT2, \"Address.City\" : genericType.DataT3.Address.City, \"Vehicle.LicenceNumber\" : genericType.DataT3.Vehicle.LicenceNumber, \"Vehicle.VehicleType.Type\" : genericType.DataT4 } }])")]
        public void Methods_with_generic_type_parameters(MultipleTypeGeneric<int, string, Person, VehicleTypeEnum> genericType)
        {
            _ = GetMongoQueryable<Person>().Where(p =>
                p.Name == genericType.DataT2 &&
                p.Address.City == genericType.DataT3.Address.City &&
                p.Vehicle.LicenceNumber == genericType.DataT3.Vehicle.LicenceNumber &&
                p.Vehicle.VehicleType.Type == genericType.DataT4);
        }

        [MQL("aggregate([{ \"$match\" : { \"Name\" : _fieldPerson.Name, \"LastName\" : _fieldString, \"Address.City\" : _fieldPerson.Address.City, \"Vehicle.VehicleType.MPG\" : _fieldDouble } }])")]
        public void Local_field_reference()
        {
            _ = GetMongoQueryable<Person>().Where(p =>
                p.Name == _fieldPerson.Name &&
                p.LastName == _fieldString &&
                p.Address.City == _fieldPerson.Address.City &&
                p.Vehicle.VehicleType.MPG == _fieldDouble);
        }

        [MQL("aggregate([{ \"$match\" : { \"Name\" : PropertyPerson.Name, \"LastName\" : PropertyString, \"Address.City\" : PropertyPerson.Address.City, \"Vehicle.VehicleType.MPG\" : PropertyDouble } }])")]
        public void Local_property_reference()
        {
            _ = GetMongoQueryable<Person>().Where(p =>
                p.Name == PropertyPerson.Name &&
                p.LastName == PropertyString &&
                p.Address.City == PropertyPerson.Address.City &&
                p.Vehicle.VehicleType.MPG == PropertyDouble);
        }

        [MQL("aggregate([{ \"$match\" : { \"SiblingsCount\" : 10 } }])")]
        public void Constants_expressions_referenced_in_expression_1()
        {
            _ = GetMongoQueryable<Person>().Where(p => p.SiblingsCount == 1 + 2 + 3 + 4);
        }

        [MQL("aggregate([{ \"$match\" : { \"Name\" : \"Joe\" } }])")]
        public void Constants_expressions_referenced_in_expression_2()
        {
            _ = GetMongoQueryable<Person>().Where(p => p.Name == "Jo" + "e");
        }

        [MQL("aggregate([{ \"$match\" : { \"SiblingsCount\" : count1 + count2 } }])")]
        public void Dynamic_expressions_referenced_in_expression_1()
        {
            var count1 = 1;
            var count2 = 2;
            _ = GetMongoQueryable<Person>().Where(p => p.SiblingsCount == count1 + count2);
        }

        [MQL("aggregate([{ \"$match\" : { \"SiblingsCount\" : count1 + (count2 + 2) >> 12 + count3 - 123 * count4 + 0 - count5 } }])")]
        public void Dynamic_expressions_referenced_in_expression_2()
        {
            var count1 = 1;
            var count2 = 2;
            var count3 = 3;
            var count4 = 4;
            var count5 = 5;
            _ = GetMongoQueryable<Person>().Where(p => p.SiblingsCount == count1 + (count2 + 2) >> 12 + count3 - 123 * count4 + 0 - count5);
        }

        [MQL("aggregate([{ \"$match\" : { \"SiblingsCount\" : count1 + _fieldDouble + PropertyDouble - 0 + 9 + _fieldPerson.SiblingsCount } }])")]
        public void Dynamic_expressions_referenced_in_expression_3()
        {
            var count1 = 1;
            _ = GetMongoQueryable<Person>().Where(p => p.SiblingsCount == count1 + _fieldDouble + PropertyDouble - 0 + 9 + _fieldPerson.SiblingsCount);
        }

        [MQL("aggregate([{ \"$match\" : { \"SiblingsCount\" : Add(Add(count1 + 2, count2), 2) + Add(count3, count3) + 2 } }])")]
        public void Dynamic_expressions_referenced_in_expression_4()
        {
            // Binary expression is top most
            var count1 = 1;
            var count2 = 2;
            var count3 = 2;
            _ = GetMongoQueryable<Person>().Where(p => p.SiblingsCount == Add(Add(count1 + 2, count2), 2) + Add(count3, count3) + 2);
        }

        [MQL("aggregate([{ \"$match\" : { \"SiblingsCount\" : Add(Add(count1 + 2, count2) + Add(count3, count3), 123) } }])")]
        public void Dynamic_expressions_referenced_in_expression_5()
        {
            // Method invocation is top most
            var count1 = 1;
            var count2 = 2;
            var count3 = 2;
            _ = GetMongoQueryable<Person>().Where(p => p.SiblingsCount == Add(Add(count1 + 2, count2) + Add(count3, count3), 123));
        }

        [MQL("aggregate([{ \"$match\" : { \"SiblingsCount\" : GetThis().Add(Add(count1 + 2, count2) + GetThis().Add(count3, count3), GetThis()._fieldInt) } }])")]
        public void Dynamic_expressions_referenced_in_expression_6()
        {
            // Method invocation is top most
            var count1 = 1;
            var count2 = 2;
            var count3 = 2;
            _ = GetMongoQueryable<Person>().Where(p => p.SiblingsCount == GetThis().Add(Add(count1 + 2, count2) + GetThis().Add(count3, count3), GetThis()._fieldInt));
        }

        [MQL("aggregate([{ \"$match\" : { \"Name\" : GetThis().Concate(Concate(str1 + \"123\", str2) + GetThis().Concate(str2, GetThis().PropertyString), GetThis()._fieldString) } }])")]
        public void Dynamic_expressions_referenced_in_expression_7()
        {
            // Method invocation is top most
            var str1 = "str1";
            var str2 = "str2";
            _ = GetMongoQueryable<Person>().Where(p => p.Name == GetThis().Concate(Concate(str1 + "123", str2) + GetThis().Concate(str2, GetThis().PropertyString), GetThis()._fieldString));
        }

        [MQLLinq3("db.coll.Aggregate([{ \"$match\" : { \"$expr\" : { \"$eq\" : [{ \"$add\" : [\"$SiblingsCount\", count1, 1] }, GetPerson().SiblingsCount] } } }])")]
        public void Property_transformation_variable()
        {
            var count1 = 1;
            _ = GetMongoQueryable<Person>().Where(u => u.SiblingsCount + count1 + 1 == GetPerson().SiblingsCount);
        }

        [MQLLinq3("db.coll.Aggregate([{ \"$match\" : { \"$expr\" : { \"$eq\" : [{ \"$add\" : [\"$SiblingsCount\", count1, 1] }, { \"$add\" : [\"$SiblingsCount\", count2] }] } } }, { \"$match\" : { \"$expr\" : { \"$eq\" : [{ \"$concat\" : [\"_prefix\", \"$Name\", \"_suffix\"] }, { \"$concat\" : [suffix, \"$Name\", prefix] }] } } }, { \"$match\" : { \"$expr\" : { \"$eq\" : [{ \"$concat\" : [prefix, \"$Name\"] }, { \"$concat\" : [\"$LastName\", Concate(prefix, Concate(prefix, suffix))] }] } } }, { \"$match\" : { \"$expr\" : { \"$eq\" : [{ \"$concat\" : [prefix, \"$Name\", Concate(\"abc\", suffix)] }, { \"$concat\" : [Concate(\"bca\", prefix), \"$LastName\", { \"$toString\" : \"$SiblingsCount\" }, suffix] }] } } }])")]
        public void Property_transformation_variable_2()
        {
            var count1 = 1;
            var count2 = 2;
            var suffix = "suf";
            var prefix = "pre";

            _ = GetMongoQueryable<Person>()
                .Where(u => u.SiblingsCount + count1 + 1 == u.SiblingsCount + count2)
                .Where(u => "_prefix" + u.Name + "_suffix" == suffix + u.Name + prefix)
                .Where(u => prefix + u.Name == u.LastName + Concate(prefix, Concate(prefix, suffix)))
                .Where(u => prefix + u.Name + Concate("abc", suffix) == Concate("bca", prefix) + u.LastName + u.SiblingsCount.ToString() + suffix);
        }

        [MQLLinq3("db.coll.Aggregate([{ \"$match\" : { \"$expr\" : { \"$eq\" : [{ \"$add\" : [\"$SiblingsCount\", count1] }, { \"$add\" : [\"$SiblingsCount\", { \"$strLenCP\" : \"$Vehicle.LicenceNumber\" }, { \"$strLenCP\" : \"$LastName\" }] }] } } }, { \"$match\" : { \"$expr\" : { \"$eq\" : [{ \"$add\" : [count2, \"$SiblingsCount\", count1, 1] }, { \"$subtract\" : [Add(count1, count2), { \"$divide\" : [{ \"$multiply\" : [\"$SiblingsCount\", { \"$strLenCP\" : \"$Vehicle.LicenceNumber\" }] }, { \"$strLenCP\" : \"$LastName\" }] }] }] } } }])")]
        public void Property_transformation_variable_3()
        {
            var count1 = 1;
            var count2 = 2;

            _ = GetMongoQueryable<Person>()
                .Where(u => u.SiblingsCount + count1 == u.SiblingsCount + u.Vehicle.LicenceNumber.Length + u.LastName.Length)
                .Where(u => count2 + u.SiblingsCount + count1 + 1 == Add(count1, count2) - u.SiblingsCount * u.Vehicle.LicenceNumber.Length / u.LastName.Length);
        }

        [MQLLinq3("db.coll.Aggregate([{ \"$match\" : { \"$and\" : [{ \"$expr\" : { \"$lte\" : [{ \"$add\" : [\"$SiblingsCount\", { \"$strLenCP\" : \"$LastName\" }, count2] }, { \"$add\" : [\"$SiblingsCount\", GetPerson().SiblingsCount * _fieldPerson.TicksSinceBirth * count1] }] } }, { \"$expr\" : { \"$eq\" : [{ \"$concat\" : [\"$Name\", _fieldPerson.LastName, count2.ToString()] }, { \"$concat\" : [Concate(_fieldPerson.Name, _fieldPerson.LastName) + \"suffix\", \"$Address.City\"] }] } }] } }])")]
        public void Property_transformation_variable_4()
        {
            var count1 = 1;
            var count2 = 2;

            _ = GetMongoQueryable<Person>().Where(u =>
                u.SiblingsCount + u.LastName.Length + count2 <= u.SiblingsCount + GetPerson().SiblingsCount * _fieldPerson.TicksSinceBirth * count1 &&
                u.Name + _fieldPerson.LastName + count2.ToString() == Concate(_fieldPerson.Name, _fieldPerson.LastName) + "suffix" + u.Address.City);
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

        private LinqConstantsReplacement GetThis() => this;

        private int Add(int a, int b) => a + b;
        private string Concate(string a, string b) => a + b;
    }
}
