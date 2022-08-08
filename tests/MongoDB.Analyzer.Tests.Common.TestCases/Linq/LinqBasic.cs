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
    public sealed class LinqBasic : TestCasesBase
    {
        [MQL("aggregate([{ \"$match\" : { \"Name\" : \"Bob\", \"Age\" : { \"$gt\" : 16, \"$lte\" : 21 } } }])")]
        public void SingleLine_expression()
        {
            _ = GetMongoCollection().AsQueryable().Where(u => u.Name == "Bob" && u.Age > 16 && u.Age <= 21);
        }

        [MQL("aggregate([{ \"$project\" : { \"LastName\" : \"$LastName\", \"_id\" : 0 } }])")]
        public void SingleLine_expression_select_only()
        {
            _ = GetMongoQueryable().Select(u => u.LastName);
        }

        [MQL("aggregate([{ \"$match\" : { \"Name\" : \"Bob\", \"Age\" : { \"$gt\" : 16, \"$lte\" : 21 } } }, { \"$match\" : { \"Height\" : 180 } }])")]
        public void TwoLines_expression()
        {
            _ = GetMongoCollection().AsQueryable()
                .Where(u => u.Name == "Bob" && u.Age > 16 && u.Age <= 21)
                .Where(u => u.Height == 180);
        }

        [MQL("aggregate([{ \"$match\" : { \"Name\" : \"Bob\", \"Age\" : { \"$gt\" : 16, \"$lte\" : 21 } } }, { \"$match\" : { \"Height\" : 180 } }, { \"$project\" : { \"Scores\" : \"$Scores\", \"_id\" : 0 } }])")]
        public void MultiLine_expression_1()
        {
            _ = GetMongoCollection().AsQueryable()
                .Where(u => u.Name == "Bob" && u.Age > 16 && u.Age <= 21)
                .Where(u => u.Height == 180)
                .Select(u => u.Scores);
        }

        [MQL("aggregate([{ \"$match\" : { \"Name\" : \"Bob\", \"Age\" : { \"$gt\" : 16, \"$lte\" : 21 } } }, { \"$match\" : { \"Height\" : { \"$lt\" : 180 } } }, { \"$match\" : { \"LastName\" : \"Smith\" } }, { \"$project\" : { \"Height\" : \"$Height\", \"_id\" : 0 } }])")]
        public void MultiLine_expression_2()
        {
            _ = GetMongoCollection().AsQueryable()
                .Where(u => u.Name == "Bob" && u.Age > 16 && u.Age <= 21)
                .Where(u => u.Height < 180)
                .Where(u => u.LastName == "Smith")
                .Select(u => u.Height);
        }

        [MQL("aggregate([{ \"$match\" : { \"FieldString\" : \"Bob\", \"PropertyArray.0\" : 1 } }, { \"$match\" : { \"FieldMixedDataMembers.FieldString\" : \"Alice\" } }])")]
        public void Mixed_fields_and_properties()
        {
            _ = GetMongoQueryable<MixedDataMembers>()
                .Where(u => u.FieldString == "Bob" && u.PropertyArray[0] == 1)
                .Where(u => u.FieldMixedDataMembers.FieldString == "Alice");
        }

        [MQL("aggregate([{ \"$match\" : { \"$or\" : [{ \"FieldString\" : /Bob/s }, { \"FieldString\" : /localVariable/s }] } }])")]
        public void Regex()
        {
            var localVariable = "Alice";

            _ = GetMongoQueryable<MixedDataMembers>().Where(u =>
                u.FieldString.Contains("Bob") ||
                u.FieldString.Contains(localVariable));
        }

        [MQL("aggregate([{ \"$group\" : { \"_id\" : \"$SiblingsCount\" } }])")]
        public void GroupBy()
        {
            _ = GetMongoQueryable<Person>()
                .GroupBy(p => p.SiblingsCount);
        }

        [MQL("aggregate([{ \"$sort\" : { \"Name\" : 1 } }])")]
        public void OrderBy()
        {
            _ = GetMongoQueryable<Person>()
                .OrderBy(p => p.Name);
        }

        [MQL("aggregate([{ \"$unwind\" : \"$Scores\" }, { \"$project\" : { \"Scores\" : \"$Scores\", \"_id\" : 0 } }])")]
        public void SelectMany()
        {
            _ = GetMongoQueryable<User>()
                .SelectMany(u => u.Scores);
        }

        [MQL("aggregate([{ \"$project\" : { \"Name\" : \"$Name\", \"City\" : \"$Address.City\", \"_id\" : 0 } }])")]
        public void Select_anonymous()
        {
            _ = GetMongoQueryable<Person>()
                .Select(p => new { p.Name, p.Address.City });
        }

        [MQL("aggregate([{ \"$project\" : { \"Name\" : \"$Name\", \"City\" : \"$Address.City\", \"MailCount\" : variableInt, \"Category\" : \"Residential\", \"_id\" : variableInt } }])")]
        public void Select_anonymous_with_constants_and_variables()
        {
            var variableInt = 123;

            _ = GetMongoQueryable<Person>()
                .Select(p => new { p.Name, p.Address.City, MailCount = variableInt, Category = "Residential" });
        }

        [MQL("aggregate([{ \"$match\" : { \"StringField\" : \"value\" } }])")]
        public void Datatype_with_objectId()
        {
            _ = GetMongoQueryable<ClassWithObjectId>().Where(c => c.StringField == "value");
        }

        [MQL("aggregate([{ \"$match\" : { \"$or\" : [{ \"Age\" : i }, { \"Name\" : i.ToString() }] } }])")]
        public void Referencing_lambda_parameter()
        {
            _ = Enumerable.Range(0, 10).Select(i =>
            {
                var q = GetMongoQueryable().Where(c => c.Age == i || c.Name == i.ToString());
                return 1;
            });
        }

        [MQL("aggregate([{ \"$match\" : { \"LastName\" : lambdaUser.LastName } }, { \"$match\" : { \"Age\" : { \"$lt\" : lambdaUser.Age } } }])")]
        public void Referencing_lambda_complex_parameter()
        {
            _ = Enumerable.Range(0, 10).Select(i => new User()).Select(lambdaUser =>
            {
                var q = GetMongoQueryable()
                    .Where(c => c.LastName == lambdaUser.LastName)
                    .Where(c => c.Age < lambdaUser.Age);
                return 1;
            });
        }

        [MQL("aggregate([{ \"$match\" : { \"$or\" : [{ \"Age\" : i }, { \"Scores.0\" : j }, { \"Scores\" : { \"$size\" : k } }, { \"Name\" : i.ToString() }, { \"LastName\" : j.ToString() }, { \"Address\" : k.ToString() }] } }])")]
        public void Referencing_nested_lambda_parameter()
        {
            var ___ = Enumerable.Range(0, 10).Select(k =>
            {
                var __ = Enumerable.Range(0, 10).Select(j =>
                {
                    _ = Enumerable.Range(0, 10).Select(i =>
                    {
                        var q = GetMongoQueryable().Where(c =>
                            c.Age == i ||
                            c.Scores[0] == j ||
                            c.Scores.Length == k ||
                            c.Name == i.ToString() ||
                            c.LastName == j.ToString() ||
                            c.Address == k.ToString());
                        return 1;
                    });

                    return 1;
                });
                return 1;
            });
        }

        [MQL("aggregate([{ \"$match\" : { \"Name\" : \"Bob\", \"Age\" : { \"$gt\" : 16, \"$lte\" : 21 } } }])", startLine: 174)]
        [MQL("aggregate([{ \"$match\" : { \"Name\" : \"John\", \"Age\" : { \"$gt\" : 45, \"$lte\" : 50 } } }])", startLine: 175)]
        public void Single_expression_variable_reassignment()
        {
            var x = GetMongoCollection().AsQueryable().Where(u => u.Name == "Bob" && u.Age > 16 && u.Age <= 21);
            x = GetMongoCollection().AsQueryable().Where(u => u.Name == "John" && u.Age > 45 && u.Age <= 50);
        }

        [MQL("aggregate([{ \"$match\" : { \"Age\" : 45 } }])", startLine: 190)]
        [MQL("aggregate([{ \"$match\" : { \"Name\" : \"John\" } }])", startLine: 191)]
        [MQL("aggregate([{ \"$match\" : { \"Name\" : \"Bob\", \"Age\" : { \"$gt\" : 16, \"$lte\" : 21 } } }])", startLine: 193)]
        [MQL("aggregate([{ \"$match\" : { \"Name\" : \"James\", \"Age\" : { \"$gt\" : 25, \"$lte\" : 99 } } }])", startLine: 194)]
        [MQL("aggregate([{ \"$match\" : { \"Name\" : \"Steve\" } }])", startLine: 196)]
        [MQL("aggregate([{ \"$match\" : { \"Height\" : 100 } }])", startLine: 197)]
        [MQL("aggregate([{ \"$match\" : { \"Age\" : 22 } }])", startLine: 199)]
        [MQL("aggregate([{ \"$match\" : { \"LastName\" : \"LastName\" } }])", startLine: 200)]
        [MQL("aggregate([{ \"$match\" : { \"Address\" : \"Address\" } }])", startLine: 202)]
        [MQL("aggregate([{ \"$match\" : { \"Age\" : { \"$lte\" : 122 } } }])", startLine: 203)]
        public void Multiple_expression_variables_reassignment()
        {
            var x = GetMongoCollection().AsQueryable().Where(u => u.Age == 45);
            var y = GetMongoCollection().AsQueryable().Where(u => u.Name == "John");

            x = GetMongoCollection().AsQueryable().Where(u => u.Name == "Bob" && u.Age > 16 && u.Age <= 21);
            var z = GetMongoCollection().AsQueryable().Where(u => u.Name == "James" && u.Age > 25 && u.Age <= 99);

            y = GetMongoCollection().AsQueryable().Where(u => u.Name == "Steve");
            x = GetMongoCollection().AsQueryable().Where(u => u.Height == 100);

            var w = GetMongoCollection().AsQueryable().Where(u => u.Age == 22);
            z = GetMongoCollection().AsQueryable().Where(u => u.LastName == "LastName");

            y = GetMongoCollection().AsQueryable().Where(u => u.Address == "Address");
            w = GetMongoCollection().AsQueryable().Where(u => u.Age <= 122);
        }

        [MQL("aggregate([{ \"$match\" : { \"Name\" : \"Bob\", \"Age\" : { \"$gt\" : 16, \"$lte\" : 21 } } }])", startLine: 214)]
        [MQL("aggregate([{ \"$match\" : { \"Name\" : \"John\", \"Age\" : { \"$gt\" : 22, \"$lte\" : 25 } } }])", startLine: 215)]
        [MQL("aggregate([{ \"$match\" : { \"Name\" : \"Steve\", \"Age\" : { \"$gt\" : 27, \"$lte\" : 31 } } }])", startLine: 216)]
        [MQL("aggregate([{ \"$match\" : { \"LastName\" : \"LastName\" } }])", startLine: 216)]
        [MQL("aggregate([{ \"$match\" : { \"Address\" : \"Address\" } }])", startLine: 218)]
        [MQL("aggregate([{ \"$match\" : { \"Height\" : 180 } }])", startLine: 219)]
        public void Multiple_expression_variables_declaration_and_reassignment()
        {
            var x = GetMongoCollection().AsQueryable().Where(u => u.Name == "Bob" && u.Age > 16 && u.Age <= 21);
            var y = GetMongoCollection().AsQueryable().Where(u => u.Name == "John" && u.Age > 22 && u.Age <= 25);
            IMongoQueryable<User> z = GetMongoCollection().AsQueryable().Where(u => u.Name == "Steve" && u.Age > 27 && u.Age <= 31), w = (GetMongoCollection().AsQueryable().Where(u => u.LastName == "LastName"));

            x = y = GetMongoCollection().AsQueryable().Where(u => u.Address == "Address");
            x = z = w = y = x = z = w = y = GetMongoCollection().AsQueryable().Where(u => u.Height == 180);
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
    }
}
