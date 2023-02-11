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

// DO NOT INCLUDE 'MongoDB.Driver.Linq' USING IN THIS FILE.
// The tests test IMongoQueryable type inference without direct MongoDB.Driver.Linq reference.

namespace MongoDB.Analyzer.Tests.Common.TestCases.Jira
{
    internal sealed class VS78 : TestCasesBase
    {
        [MQL("aggregate([{ \"$match\" : { \"AbstractBaseData\" : \"base\", \"AbstractBaseDataT1\" : 0, \"AbstractBaseDataT2.Name\" : \"Bob\" } }, { \"$match\" : { \"NestedGenericClass1T1\" : 1, \"NestedGenericClass1T2.Name\" : \"Alice\" } }, { \"$match\" : { \"NestedGenericClass2T1\" : 0, \"NestedGenericClass2T2.Name\" : \"John\" } }])")]
        public void Complex_data_model_functional_syntax()
        {
            _ = GetMongoQueryable<NestedGenericClass2<EnumInt32, Person>>()
                .Where(t => t.AbstractBaseData == "base" && t.AbstractBaseDataT1 == EnumInt32.Value0 && t.AbstractBaseDataT2.Name == "Bob")
                .Where(t => t.NestedGenericClass1T1 == EnumInt32.Value1 && t.NestedGenericClass1T2.Name == "Alice")
                .Where(t => t.NestedGenericClass2T1 == EnumInt32.Value0 && t.NestedGenericClass2T2.Name == "John");
        }

        [MQL("aggregate([{ \"$match\" : { \"Data\" : 1 } }, { \"$match\" : { \"DataT1\" : 32 } }, { \"$match\" : { \"DataT2\" : \"dataString\" } }, { \"$match\" : { \"DataT3.Vehicle.LicenceNumber\" : \"LicenceNumber\" } }, { \"$match\" : { \"DataT4\" : NumberLong(999) } }])")]
        [MQL("aggregate([{ \"$match\" : { \"AbstractBaseData\" : \"base\", \"AbstractBaseDataT1\" : 0, \"AbstractBaseDataT2.Name\" : \"Bob\" } }, { \"$match\" : { \"NestedGenericClass1T1\" : 1, \"NestedGenericClass1T2.Name\" : \"Alice\" } }, { \"$match\" : { \"NestedGenericClass2T1\" : 0, \"NestedGenericClass2T2.Name\" : \"John\" } }])")]
        public void Complex_data_model_query_syntax()
        {
            _ = from multipleTypeGeneric in GetMongoQueryable<MultipleTypeGeneric<int, string, Person, EnumInt64>>()
                where multipleTypeGeneric.Data == 1
                where multipleTypeGeneric.DataT1 == 32
                where multipleTypeGeneric.DataT2 == "dataString"
                where multipleTypeGeneric.DataT3.Vehicle.LicenceNumber == "LicenceNumber"
                where multipleTypeGeneric.DataT4 == EnumInt64.Value999
                select multipleTypeGeneric;

            _ = from nestedGenericClass2 in GetMongoQueryable<NestedGenericClass2<EnumInt32, Person>>()
                where nestedGenericClass2.AbstractBaseData == "base" && nestedGenericClass2.AbstractBaseDataT1 == EnumInt32.Value0 && nestedGenericClass2.AbstractBaseDataT2.Name == "Bob"
                where nestedGenericClass2.NestedGenericClass1T1 == EnumInt32.Value1 && nestedGenericClass2.NestedGenericClass1T2.Name == "Alice"
                where nestedGenericClass2.NestedGenericClass2T1 == EnumInt32.Value0 && nestedGenericClass2.NestedGenericClass2T2.Name == "John"
                select nestedGenericClass2;
        }

        [MQL("aggregate([{ \"$match\" : { \"$or\" : [{ \"Name\" : \"BBB\" }, { \"Name\" : \"BBB\" }, { \"Name\" : \"BBB\" }, { \"Vehicle.VehicleType.Type\" : 0 }, { \"Vehicle.VehicleType.MPG\" : 234.43199999999999 }] } }])")]
        public void Constant_values_replacement_functional_syntax()
        {
            _ = GetMongoQueryable<Person>().Where(p =>
                p.Name == ConstantsHolder.ConstantString ||
                p.Name == DataModel.ConstantsHolder.ConstantString ||
                p.Name == MongoDB.Analyzer.Tests.Common.DataModel.ConstantsHolder.ConstantString ||
                p.Vehicle.VehicleType.Type == ConstantsHolder.ConstantEnum ||
                p.Vehicle.VehicleType.MPG == ConstantsHolder.ConstantDouble);
        }

        [MQLLinq3("db.coll.Aggregate([{ \"$match\" : { \"$expr\" : { \"$eq\" : [{ \"$add\" : [\"$SiblingsCount\", count1, 1] }, { \"$add\" : [\"$SiblingsCount\", count2] }] } } }, { \"$match\" : { \"$expr\" : { \"$eq\" : [{ \"$concat\" : [\"_prefix\", \"$Name\", \"_suffix\"] }, { \"$concat\" : [suffix, \"$Name\", prefix] }] } } }, { \"$match\" : { \"$expr\" : { \"$eq\" : [{ \"$concat\" : [prefix, \"$Name\"] }, { \"$concat\" : [\"$LastName\", Concate(prefix, Concate(prefix, suffix))] }] } } }, { \"$match\" : { \"$expr\" : { \"$eq\" : [{ \"$concat\" : [prefix, \"$Name\", Concate(\"abc\", suffix)] }, { \"$concat\" : [Concate(\"bca\", prefix), \"$LastName\", { \"$toString\" : \"$SiblingsCount\" }, suffix] }] } } }])")]
        public void Constant_values_replacement_query_syntax()
        {
            var count1 = 1;
            var count2 = 2;
            var suffix = "suf";
            var prefix = "pre";

            _ = from person in GetMongoQueryable<Person>()
                where person.SiblingsCount + count1 + 1 == person.SiblingsCount + count2
                where "_prefix" + person.Name + "_suffix" == suffix + person.Name + prefix
                where prefix + person.Name == person.LastName + Concate(prefix, Concate(prefix, suffix))
                where prefix + person.Name + Concate("abc", suffix) == Concate("bca", prefix) + person.LastName + person.SiblingsCount.ToString() + suffix
                select person;
        }

        [MQL("aggregate([{ \"$match\" : { \"Name\" : \"Bob\", \"Age\" : { \"$gt\" : 18, \"$lte\" : 21 } } }, { \"$match\" : { \"LastName\" : /^.{0,9}$/s } }, { \"$project\" : { \"Name\" : \"$Name\", \"_id\" : 0 } }])")]
        [MQL("aggregate([{ \"$match\" : { \"Name\" : \"Bob\", \"Age\" : { \"$gt\" : 18, \"$lte\" : 21 } } }, { \"$match\" : { \"LastName\" : /^.{0,9}$/s } }, { \"$project\" : { \"Name\" : \"$Name\", \"_id\" : 0 } }])")]
        [MQL("aggregate([{ \"$match\" : { \"Name\" : \"Bob\", \"Age\" : { \"$gt\" : 16, \"$lte\" : 21 } } }])")]
        [MQL("aggregate([{ \"$match\" : { \"Name\" : \"Bob2\", \"Age\" : { \"$gt\" : 18, \"$lte\" : 21 } } }, { \"$match\" : { \"LastName\" : /^.{0,10}$/s } }, { \"$project\" : { \"Name\" : \"$Name\", \"_id\" : 0 } }])")]
        [MQL("aggregate([{ \"$match\" : { \"Name\" : \"Bob\", \"Age\" : { \"$gt\" : 18, \"$lte\" : 21 } } }, { \"$match\" : { \"LastName\" : /^.{0,9}$/s } }, { \"$project\" : { \"Name\" : \"$Name\", \"_id\" : 0 } }])")]
        public void IMongoQueryable_from_different_sources()
        {
            _ = GetMongoCollection().AsQueryable()
                .Where(u => u.Name == "Bob" && u.Age > 18 && u.Age <= 21)
                .Where(u => u.LastName.Length < 10)
                .Select(u => u.Name);

            var collection = GetMongoCollection();
            var queryable = collection.AsQueryable();
            _ = queryable
                .Where(u => u.Name == "Bob" && u.Age > 18 && u.Age <= 21)
                .Where(u => u.LastName.Length < 10)
                .Select(u => u.Name);

            _ = ReturnArgument(ReturnArgument(ReturnArgument(GetMongoCollection())).AsQueryable())
                .Where(u => u.Name == "Bob" && u.Age > 16 && u.Age <= 21);

            _ = from u in ReturnArgument(ReturnArgument(GetThis().GetThis()
                .ReturnArgument(GetMongoCollection()))
                .AsQueryable())
                where u.Name == "Bob2" && u.Age > 18 && u.Age <= 21
                where u.LastName.Length < 11
                select u.Name;

            _ = from u in queryable
                where u.Name == "Bob" && u.Age > 18 && u.Age <= 21
                where u.LastName.Length < 10
                select u.Name;
        }

        [MQL("aggregate([{ \"$match\" : { \"Name\" : \"Bob\", \"Age\" : { \"$gt\" : 16, \"$lte\" : 21 } } }, { \"$match\" : { \"LastName\" : /^.{0,9}$/s } }])")]
        public void IMongoQueryable_extensions_prefix()
        {
            _ = GetMongoQueryable()
                .ApplyPaging(1, 0)
                .Where(u => u.Name == "Bob" && u.Age > 16 && u.Age <= 21)
                .Where(u => u.LastName.Length < 10);
        }

        [MQL("aggregate([{ \"$match\" : { \"Name\" : \"Bob\", \"Age\" : { \"$gt\" : 16, \"$lte\" : 21 } } }, { \"$match\" : { \"LastName\" : /^.{0,9}$/s } }])")]
        [MQL("aggregate([{ \"$match\" : { \"Name\" : \"Bob\", \"Age\" : { \"$gt\" : 16, \"$lte\" : 21 } } }, { \"$match\" : { \"LastName\" : /^.{0,9}$/s } }])")]
        public void IQueryable_extensions_suffix()
        {
            _ = GetMongoQueryable()
                .Where(u => u.Name == "Bob" && u.Age > 16 && u.Age <= 21)
                .Where(u => u.LastName.Length < 10)
                .ApplyPagingIQueryable(1, 0);

            _ = GetMongoQueryable()
                .ApplyPaging(1, 0)
                .Where(u => u.Name == "Bob" && u.Age > 16 && u.Age <= 21)
                .Where(u => u.LastName.Length < 10)
                .ApplyPagingIQueryable(1, 0);
        }

        [NoDiagnostics]
        public void IQueryable_extensions_in_middle_and_prefix_should_be_ignored()
        {
            _ = GetMongoQueryable()
                .Where(u => u.Name == "Bob" && u.Age > 16 && u.Age <= 21)
                .Where(u => u.LastName.Length < 10)
                .ApplyPagingIQueryable(1, 0)
                .Where(u => u.LastName == "Smith");

            _ = GetMongoQueryable()
                .ApplyPagingIQueryable(1, 0)
                .Where(u => u.Name == "Bob" && u.Age > 16 && u.Age <= 21)
                .Where(u => u.LastName.Length < 10);

            _ = GetMongoQueryable()
                .ApplyPagingIQueryable(1, 0)
                .Where(u => u.Name == "Bob" && u.Age > 16 && u.Age <= 21)
                .Where(u => u.LastName.Length < 10);

            _ = GetMongoQueryable()
              .ApplyPaging(1, 0)
              .ApplyPagingIQueryable(1, 0)
              .Where(u => u.Name == "Bob" && u.Age > 16 && u.Age <= 21)
              .Where(u => u.LastName.Length < 10);
        }

        [MQL("aggregate([{ \"$match\" : { \"Root.Data\" : intVariable1 + intVariable2 } }])")]
        [MQL("aggregate([{ \"$match\" : { \"VehicleType.MPG\" : intVariable1 + intVariable2 } }])")]
        [MQL("aggregate([{ \"$match\" : { \"TicksSinceBirth\" : NumberLong(intVariable1 + intVariable2) } }])")]
        [MQL("aggregate([{ \"$match\" : { \"VehicleType.MPG\" : uintVariable1 + uintVariable2 } }])")]
        [MQL("aggregate([{ \"$match\" : { \"TicksSinceBirth\" : NumberLong(uintVariable1 + uintVariable2) } }])")]
        [MQL("aggregate([{ \"$match\" : { \"Root.Data\" : shortVariable1 + shortVariable2 } }])")]
        [MQL("aggregate([{ \"$match\" : { \"VehicleType.MPG\" : shortVariable1 + shortVariable2 } }])")]
        [MQL("aggregate([{ \"$match\" : { \"TicksSinceBirth\" : NumberLong(shortVariable1 + shortVariable2) } }])")]
        [MQL("aggregate([{ \"$match\" : { \"Root.Data\" : ushortVariable1 + ushortVariable2 } }])")]
        [MQL("aggregate([{ \"$match\" : { \"VehicleType.MPG\" : ushortVariable1 + ushortVariable2 } }])")]
        [MQL("aggregate([{ \"$match\" : { \"TicksSinceBirth\" : NumberLong(ushortVariable1 + ushortVariable2) } }])")]
        [MQL("aggregate([{ \"$match\" : { \"Data\" : byteVariable1 + byteVariable2 } }])")]
        [MQL("aggregate([{ \"$match\" : { \"VehicleType.MPG\" : byteVariable1 + byteVariable2 } }])")]
        [MQL("aggregate([{ \"$match\" : { \"TicksSinceBirth\" : NumberLong(byteVariable1 + byteVariable2) } }])")]
        [MQL("aggregate([{ \"$match\" : { \"Data\" : sbyteVariable1 + sbyteVariable2 } }])")]
        [MQL("aggregate([{ \"$match\" : { \"VehicleType.MPG\" : sbyteVariable1 + sbyteVariable2 } }])")]
        [MQL("aggregate([{ \"$match\" : { \"TicksSinceBirth\" : NumberLong(sbyteVariable1 + sbyteVariable2) } }])")]
        [MQL("aggregate([{ \"$match\" : { \"Data\" : longVariable1 + longVariable2 } }])")]
        [MQL("aggregate([{ \"$match\" : { \"VehicleType.MPG\" : longVariable1 + longVariable2 } }])")]
        [MQL("aggregate([{ \"$match\" : { \"TicksSinceBirth\" : NumberLong(longVariable1 + longVariable2) } }])")]
        [MQL("aggregate([{ \"$match\" : { \"VehicleType.MPG\" : ulongVariable1 + ulongVariable2 } }])")]
        [MQL("aggregate([{ \"$match\" : { \"Root.Data\" : doubleVariable1 + doubleVariable2 } }])")]
        [MQL("aggregate([{ \"$match\" : { \"VehicleType.MPG\" : doubleVariable1 + doubleVariable2 } }])")]
        [MQL("aggregate([{ \"$match\" : { \"TicksSinceBirth\" : doubleVariable1 + doubleVariable2 } }])")]
        public void Mixed_data_types()
        {
            int intVariable1 = 10;
            int intVariable2 = 11;

            long longVariable1 = 12L;
            long longVariable2 = 13L;

            double doubleVariable1 = 2.5;
            double doubleVariable2 = 3.5;

            uint uintVariable1 = 22;
            uint uintVariable2 = 23;

            short shortVariable1 = 24;
            short shortVariable2 = 25;

            ushort ushortVariable1 = 26;
            ushort ushortVariable2 = 27;

            byte byteVariable1 = 28;
            byte byteVariable2 = 29;

            sbyte sbyteVariable1 = 30;
            sbyte sbyteVariable2 = 31;

            ulong ulongVariable1 = 32L;
            ulong ulongVariable2 = 33L;

            _ = GetMongoCollection<Tree>().AsQueryable().Where(t => t.Root.Data == intVariable1 + intVariable2);
            _ = GetMongoCollection<Vehicle>().AsQueryable().Where(v => v.VehicleType.MPG == intVariable1 + intVariable2);
            _ = GetMongoCollection<Person>().AsQueryable().Where(p => p.TicksSinceBirth == intVariable1 + intVariable2);

            _ = GetMongoCollection<Vehicle>().AsQueryable().Where(v => v.VehicleType.MPG == uintVariable1 + uintVariable2);
            _ = GetMongoCollection<Person>().AsQueryable().Where(p => p.TicksSinceBirth == uintVariable1 + uintVariable2);

            _ = GetMongoCollection<Tree>().AsQueryable().Where(t => t.Root.Data == shortVariable1 + shortVariable2);
            _ = GetMongoCollection<Vehicle>().AsQueryable().Where(v => v.VehicleType.MPG == shortVariable1 + shortVariable2);
            _ = GetMongoCollection<Person>().AsQueryable().Where(p => p.TicksSinceBirth == shortVariable1 + shortVariable2);

            _ = GetMongoCollection<Tree>().AsQueryable().Where(t => t.Root.Data == ushortVariable1 + ushortVariable2);
            _ = GetMongoCollection<Vehicle>().AsQueryable().Where(v => v.VehicleType.MPG == ushortVariable1 + ushortVariable2);
            _ = GetMongoCollection<Person>().AsQueryable().Where(p => p.TicksSinceBirth == ushortVariable1 + ushortVariable2);


            _ = GetMongoCollection<TreeNode>().AsQueryable().Where(n => n.Data == byteVariable1 + byteVariable2);
            _ = GetMongoCollection<Vehicle>().AsQueryable().Where(v => v.VehicleType.MPG == byteVariable1 + byteVariable2);
            _ = GetMongoCollection<Person>().AsQueryable().Where(p => p.TicksSinceBirth == byteVariable1 + byteVariable2);

            _ = GetMongoCollection<TreeNode>().AsQueryable().Where(n => n.Data == sbyteVariable1 + sbyteVariable2);
            _ = GetMongoCollection<Vehicle>().AsQueryable().Where(v => v.VehicleType.MPG == sbyteVariable1 + sbyteVariable2);
            _ = GetMongoCollection<Person>().AsQueryable().Where(p => p.TicksSinceBirth == sbyteVariable1 + sbyteVariable2);

            _ = GetMongoCollection<TreeNode>().AsQueryable().Where(n => n.Data == longVariable1 + longVariable2);
            _ = GetMongoCollection<Vehicle>().AsQueryable().Where(v => v.VehicleType.MPG == longVariable1 + longVariable2);
            _ = GetMongoCollection<Person>().AsQueryable().Where(p => p.TicksSinceBirth == longVariable1 + longVariable2);

            _ = GetMongoCollection<Vehicle>().AsQueryable().Where(v => v.VehicleType.MPG == ulongVariable1 + ulongVariable2);

            _ = GetMongoCollection<Tree>().AsQueryable().Where(t => t.Root.Data == doubleVariable1 + doubleVariable2);
            _ = GetMongoCollection<Vehicle>().AsQueryable().Where(v => v.VehicleType.MPG == doubleVariable1 + doubleVariable2);
            _ = GetMongoCollection<Person>().AsQueryable().Where(p => p.TicksSinceBirth == doubleVariable1 + doubleVariable2);
        }

        [MQL("aggregate([{ \"$match\" : { \"Name\" : \"Bob\", \"Age\" : { \"$gt\" : 16, \"$lte\" : 21 } } }, { \"$match\" : { \"Height\" : 180 } }, { \"$project\" : { \"Scores\" : \"$Scores\", \"_id\" : 0 } }])")]
        [MQL("aggregate([{ \"$match\" : { \"Name\" : \"Bob\", \"Age\" : { \"$gt\" : 16, \"$lte\" : 21 } } }, { \"$match\" : { \"Height\" : 180 } }, { \"$project\" : { \"Scores\" : \"$Scores\", \"_id\" : 0 } }])")]
        public void MultiLine_expression()
        {
            _ = GetMongoCollection().AsQueryable()
                .Where(u => u.Name == "Bob" && u.Age > 16 && u.Age <= 21)
                .Where(u => u.Height == 180)
                .Select(u => u.Scores);

            _ = from user in GetMongoCollection().AsQueryable()
                where user.Name == "Bob" && user.Age > 16 && user.Age <= 21
                where user.Height == 180
                select user.Scores;
        }

        [MQL("aggregate([{ \"$match\" : { \"Name\" : \"Bob\", \"Age\" : { \"$gt\" : 16, \"$lte\" : 21 } } }])")]
        [MQL("aggregate([{ \"$match\" : { \"Name\" : \"Bob\", \"Age\" : { \"$gt\" : 16, \"$lte\" : 21 } } }])")]
        public void SingleLine_expression()
        {
            _ = GetMongoCollection().AsQueryable()
                .Where(u => u.Name == "Bob" && u.Age > 16 && u.Age <= 21);

            _ = from user in GetMongoCollection().AsQueryable()
                where user.Name == "Bob" && user.Age > 16 && user.Age <= 21
                select user;
        }

        [InvalidLinq("{document}{Name}.LastIndexOf(1) is not supported.")]
        [InvalidLinq3("Expression not supported: u.Name.LastIndexOf(1).")]
        public void Unsupported_string_method_LastIndexOf()
        {
            _ = GetMongoQueryable()
               .Where(u => u.Name.LastIndexOf('1') == 1);
        }

        private string Concate(string a, string b) => a + b;

        private VS78 GetThis() => this;
    }
}
