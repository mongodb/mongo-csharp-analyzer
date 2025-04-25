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
        [MQL("Aggregate([{ \"$match\" : { \"StringField\" : \"value\" } }])")]
        [MQL("Aggregate([{ \"$match\" : { \"StringField\" : \"value\" } }])")]
        public void Datatype_with_objectId()
        {
            _ = GetMongoQueryable<ClassWithObjectId>().Where(c => c.StringField == "value");

            _ = from classWithObjectId in GetMongoQueryable<ClassWithObjectId>()
                where classWithObjectId.StringField == "value"
            select classWithObjectId;
        }

        [MQL("Aggregate([{ \"$group\" : { \"_id\" : \"$SiblingsCount\", \"_elements\" : { \"$push\" : \"$$ROOT\" } } }])")]
        [MQL("Aggregate([{ \"$group\" : { \"_id\" : \"$SiblingsCount\", \"_elements\" : { \"$push\" : \"$$ROOT\" } } }])")]
        public void GroupBy()
        {
            _ = GetMongoQueryable<Person>()
                .GroupBy(p => p.SiblingsCount);

            _ = from person in GetMongoQueryable<Person>()
                group person by person.SiblingsCount into newGroup
                select newGroup;
        }

        [MQL("Aggregate([{ \"$match\" : { \"Root.Data\" : intVariable1 + intVariable2 } }])")]
        [MQL("Aggregate([{ \"$match\" : { \"VehicleType.MPG\" : intVariable1 + intVariable2 } }])")]
        [MQL("Aggregate([{ \"$match\" : { \"TicksSinceBirth\" : intVariable1 + intVariable2 } }])")]
        [MQL("Aggregate([{ \"$match\" : { \"VehicleType.MPG\" : uintVariable1 + uintVariable2 } }])")]
        [MQL("Aggregate([{ \"$match\" : { \"TicksSinceBirth\" : uintVariable1 + uintVariable2 } }])")]
        [MQL("Aggregate([{ \"$match\" : { \"Root.Data\" : shortVariable1 + shortVariable2 } }])")]
        [MQL("Aggregate([{ \"$match\" : { \"VehicleType.MPG\" : shortVariable1 + shortVariable2 } }])")]
        [MQL("Aggregate([{ \"$match\" : { \"TicksSinceBirth\" : shortVariable1 + shortVariable2 } }])")]
        [MQL("Aggregate([{ \"$match\" : { \"Root.Data\" : ushortVariable1 + ushortVariable2 } }])")]
        [MQL("Aggregate([{ \"$match\" : { \"VehicleType.MPG\" : ushortVariable1 + ushortVariable2 } }])")]
        [MQL("Aggregate([{ \"$match\" : { \"TicksSinceBirth\" : ushortVariable1 + ushortVariable2 } }])")]
        [MQL("Aggregate([{ \"$match\" : { \"Data\" : byteVariable1 + byteVariable2 } }])")]
        [MQL("Aggregate([{ \"$match\" : { \"VehicleType.MPG\" : byteVariable1 + byteVariable2 } }])")]
        [MQL("Aggregate([{ \"$match\" : { \"TicksSinceBirth\" : byteVariable1 + byteVariable2 } }])")]
        [MQL("Aggregate([{ \"$match\" : { \"Data\" : sbyteVariable1 + sbyteVariable2 } }])")]
        [MQL("Aggregate([{ \"$match\" : { \"VehicleType.MPG\" : sbyteVariable1 + sbyteVariable2 } }])")]
        [MQL("Aggregate([{ \"$match\" : { \"TicksSinceBirth\" : sbyteVariable1 + sbyteVariable2 } }])")]
        [MQL("Aggregate([{ \"$match\" : { \"Data\" : longVariable1 + longVariable2 } }])")]
        [MQL("Aggregate([{ \"$match\" : { \"VehicleType.MPG\" : longVariable1 + longVariable2 } }])")]
        [MQL("Aggregate([{ \"$match\" : { \"TicksSinceBirth\" : longVariable1 + longVariable2 } }])")]
        [MQL("Aggregate([{ \"$match\" : { \"VehicleType.MPG\" : ulongVariable1 + ulongVariable2 } }])")]
        [MQL("Aggregate([{ \"$match\" : { \"Root.Data\" : doubleVariable1 + doubleVariable2 } }])")]
        [MQL("Aggregate([{ \"$match\" : { \"VehicleType.MPG\" : doubleVariable1 + doubleVariable2 } }])")]
        [MQL("Aggregate([{ \"$match\" : { \"TicksSinceBirth\" : doubleVariable1 + doubleVariable2 } }])")]
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

        [MQL("Aggregate([{ \"$match\" : { \"FieldString\" : \"Bob\", \"PropertyArray.0\" : 1 } }, { \"$match\" : { \"FieldMixedDataMembers.FieldString\" : \"Alice\" } }])")]
        [MQL("Aggregate([{ \"$match\" : { \"FieldString\" : \"Bob\", \"PropertyArray.0\" : 1 } }, { \"$match\" : { \"FieldMixedDataMembers.FieldString\" : \"Alice\" } }])")]
        public void Mixed_fields_and_properties()
        {
            _ = GetMongoQueryable<MixedDataMembers>()
                .Where(u => u.FieldString == "Bob" && u.PropertyArray[0] == 1)
                .Where(u => u.FieldMixedDataMembers.FieldString == "Alice");

            _ = from mixedDataMember in GetMongoQueryable<MixedDataMembers>()
                where mixedDataMember.FieldString == "Bob" && mixedDataMember.PropertyArray[0] == 1
                where mixedDataMember.FieldMixedDataMembers.FieldString == "Alice"
                select mixedDataMember;
        }

        [MQL("Aggregate([{ \"$match\" : { \"Name\" : \"Bob\", \"Age\" : { \"$gt\" : 16, \"$lte\" : 21 } } }, { \"$match\" : { \"Height\" : 180 } }, { \"$project\" : { \"_v\" : \"$Scores\", \"_id\" : 0 } }])")]
        [MQL("Aggregate([{ \"$match\" : { \"Name\" : \"Bob\", \"Age\" : { \"$gt\" : 16, \"$lte\" : 21 } } }, { \"$match\" : { \"Height\" : 180 } }, { \"$project\" : { \"_v\" : \"$Scores\", \"_id\" : 0 } }])")]
        public void MultiLine_expression_1()
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

        [MQL("Aggregate([{ \"$match\" : { \"Name\" : \"Bob\", \"Age\" : { \"$gt\" : 16, \"$lte\" : 21 } } }, { \"$match\" : { \"Height\" : { \"$lt\" : 180 } } }, { \"$match\" : { \"LastName\" : \"Smith\" } }, { \"$project\" : { \"_v\" : \"$Height\", \"_id\" : 0 } }])")]
        [MQL("Aggregate([{ \"$match\" : { \"Name\" : \"Bob\", \"Age\" : { \"$gt\" : 16, \"$lte\" : 21 } } }, { \"$match\" : { \"Height\" : { \"$lt\" : 180 } } }, { \"$match\" : { \"LastName\" : \"Smith\" } }, { \"$project\" : { \"_v\" : \"$Height\", \"_id\" : 0 } }])")]
        public void MultiLine_expression_2()
        {
            _ = GetMongoCollection().AsQueryable()
                .Where(u => u.Name == "Bob" && u.Age > 16 && u.Age <= 21)
                .Where(u => u.Height < 180)
                .Where(u => u.LastName == "Smith")
                .Select(u => u.Height);

            _ = from user in GetMongoCollection().AsQueryable()
                where user.Name == "Bob" && user.Age > 16 && user.Age <= 21
                where user.Height < 180
                where user.LastName == "Smith"
                select user.Height;
        }

        [MQL("Aggregate([{ \"$match\" : { \"Name\" : \"Bob\", \"Age\" : { \"$gt\" : 16, \"$lte\" : 21 } } }])", 1)]
        [MQL("Aggregate([{ \"$match\" : { \"Name\" : \"John\", \"Age\" : { \"$gt\" : 22, \"$lte\" : 25 } } }])", 2)]
        [MQL("Aggregate([{ \"$match\" : { \"Name\" : \"Steve\", \"Age\" : { \"$gt\" : 27, \"$lte\" : 31 } } }])", 3)]
        [MQL("Aggregate([{ \"$match\" : { \"LastName\" : \"LastName\" } }])", 3)]
        [MQL("Aggregate([{ \"$match\" : { \"Address\" : \"Address\" } }])", 5)]
        [MQL("Aggregate([{ \"$match\" : { \"Height\" : 180 } }])", 6)]
        [MQL("Aggregate([{ \"$match\" : { \"Name\" : \"Bob\", \"Age\" : { \"$gt\" : 16, \"$lte\" : 21 } } }])", 8)]
        [MQL("Aggregate([{ \"$match\" : { \"Name\" : \"John\", \"Age\" : { \"$gt\" : 22, \"$lte\" : 25 } } }])", 12)]
        [MQL("Aggregate([{ \"$match\" : { \"Name\" : \"Steve\", \"Age\" : { \"$gt\" : 27, \"$lte\" : 31 } } }])", 16)]
        [MQL("Aggregate([{ \"$match\" : { \"LastName\" : \"LastName\" } }])", 19)]
        [MQL("Aggregate([{ \"$match\" : { \"Address\" : \"Address\" } }])", 23)]
        [MQL("Aggregate([{ \"$match\" : { \"Height\" : 180 } }])", 27)]
        public void Multiple_expression_variables_declaration_and_reassignment()
        {
            var x = GetMongoCollection().AsQueryable().Where(u => u.Name == "Bob" && u.Age > 16 && u.Age <= 21);
            var y = GetMongoCollection().AsQueryable().Where(u => u.Name == "John" && u.Age > 22 && u.Age <= 25);
            IQueryable<User> z = GetMongoCollection().AsQueryable().Where(u => u.Name == "Steve" && u.Age > 27 && u.Age <= 31), w = (GetMongoCollection().AsQueryable().Where(u => u.LastName == "LastName"));

            x = y = GetMongoCollection().AsQueryable().Where(u => u.Address == "Address");
            x = z = w = y = x = z = w = y = GetMongoCollection().AsQueryable().Where(u => u.Height == 180);

            var a = from user in GetMongoCollection().AsQueryable()
                    where user.Name == "Bob" && user.Age > 16 && user.Age <= 21
                    select user;

            var b = from user in GetMongoCollection().AsQueryable()
                    where user.Name == "John" && user.Age > 22 && user.Age <= 25
                    select user;

            IQueryable<User> c = from user in GetMongoCollection().AsQueryable()
                                      where user.Name == "Steve" && user.Age > 27 && user.Age <= 31
                                      select user,
                                      d = from user in GetMongoCollection().AsQueryable()
                                          where user.LastName == "LastName"
                                          select user;

            a = b = from user in GetMongoCollection().AsQueryable()
                    where user.Address == "Address"
                    select user;

            a = c = d = b = a = c = d = b = from user in GetMongoCollection().AsQueryable()
                                            where user.Height == 180
                                            select user;
        }

        [MQL("Aggregate([{ \"$match\" : { \"Age\" : 45 } }])", 1, 16)]
        [MQL("Aggregate([{ \"$match\" : { \"Name\" : \"John\" } }])", 2, 20)]
        [MQL("Aggregate([{ \"$match\" : { \"Name\" : \"Bob\", \"Age\" : { \"$gt\" : 16, \"$lte\" : 21 } } }])", 4, 24)]
        [MQL("Aggregate([{ \"$match\" : { \"Name\" : \"James\", \"Age\" : { \"$gt\" : 25, \"$lte\" : 99 } } }])", 5, 28)]
        [MQL("Aggregate([{ \"$match\" : { \"Name\" : \"Steve\" } }])", 7, 32)]
        [MQL("Aggregate([{ \"$match\" : { \"Height\" : 100 } }])", 8, 36)]
        [MQL("Aggregate([{ \"$match\" : { \"Age\" : 22 } }])", 10, 40)]
        [MQL("Aggregate([{ \"$match\" : { \"LastName\" : \"LastName\" } }])", 11, 44)]
        [MQL("Aggregate([{ \"$match\" : { \"Address\" : \"Address\" } }])", 13, 48)]
        [MQL("Aggregate([{ \"$match\" : { \"Age\" : { \"$lte\" : 122 } } }])", 14, 52)]
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

            x = from user in GetMongoCollection().AsQueryable()
                where user.Age == 45
                select user;

            y = from user in GetMongoCollection().AsQueryable()
                where user.Name == "John"
                select user;

            x = from user in GetMongoCollection().AsQueryable()
                where user.Name == "Bob" && user.Age > 16 && user.Age <= 21
                select user;

            z = from user in GetMongoCollection().AsQueryable()
                where user.Name == "James" && user.Age > 25 && user.Age <= 99
                select user;

            y = from user in GetMongoCollection().AsQueryable()
                where user.Name == "Steve"
                select user;

            x = from user in GetMongoCollection().AsQueryable()
                where user.Height == 100
                select user;

            w = from user in GetMongoCollection().AsQueryable()
                where user.Age == 22
                select user;

            z = from user in GetMongoCollection().AsQueryable()
                where user.LastName == "LastName"
                select user;

            y = from user in GetMongoCollection().AsQueryable()
                where user.Address == "Address"
                select user;

            w = from user in GetMongoCollection().AsQueryable()
                where user.Age <= 122
                select user;
        }

        [MQL("Aggregate([{ \"$sort\" : { \"Name\" : 1 } }])")]
        [MQL("Aggregate([{ \"$sort\" : { \"Name\" : 1 } }])")]
        public void OrderBy()
        {
            _ = GetMongoQueryable<Person>()
                .OrderBy(p => p.Name);

            _ = from person in GetMongoQueryable<Person>()
                orderby person.Name
                select person;
        }

        [MQL("Aggregate([{ \"$match\" : { \"LastName\" : lambdaUser.LastName } }, { \"$match\" : { \"Age\" : { \"$lt\" : lambdaUser.Age } } }])")]
        [MQL("Aggregate([{ \"$match\" : { \"LastName\" : lambdaUser.LastName } }, { \"$match\" : { \"Age\" : { \"$lt\" : lambdaUser.Age } } }])")]
        public void Referencing_lambda_complex_parameter()
        {
            _ = Enumerable.Range(0, 10).Select(i => new User()).Select(lambdaUser =>
            {
                var q = GetMongoQueryable()
                    .Where(c => c.LastName == lambdaUser.LastName)
                    .Where(c => c.Age < lambdaUser.Age);
                _ = from user in GetMongoQueryable()
                    where user.LastName == lambdaUser.LastName
                    where user.Age < lambdaUser.Age
                    select user;

                return 1;
            });
        }

        [MQL("Aggregate([{ \"$match\" : { \"$or\" : [{ \"Age\" : i }, { \"Name\" : i.ToString() }] } }])")]
        [MQL("Aggregate([{ \"$match\" : { \"$or\" : [{ \"Age\" : i }, { \"Name\" : i.ToString() }] } }])")]
        public void Referencing_lambda_parameter()
        {
            _ = Enumerable.Range(0, 10).Select(i =>
            {
                var q = GetMongoQueryable().Where(c => c.Age == i || c.Name == i.ToString());
                _ = from user in GetMongoQueryable()
                    where user.Age == i || user.Name == i.ToString()
                    select user;

                return 1;
            });
        }

        [MQL("Aggregate([{ \"$match\" : { \"$or\" : [{ \"Age\" : i }, { \"Scores.0\" : j }, { \"Scores\" : { \"$size\" : k } }, { \"Name\" : i.ToString() }, { \"LastName\" : j.ToString() }, { \"Address\" : k.ToString() }] } }])")]
        [MQL("Aggregate([{ \"$match\" : { \"$or\" : [{ \"Age\" : i }, { \"Scores.0\" : j }, { \"Scores\" : { \"$size\" : k } }, { \"Name\" : i.ToString() }, { \"LastName\" : j.ToString() }, { \"Address\" : k.ToString() }] } }])")]
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

                        _ = from user in GetMongoQueryable()
                            where user.Age == i ||
                                  user.Scores[0] == j ||
                                  user.Scores.Length == k ||
                                  user.Name == i.ToString() ||
                                  user.LastName == j.ToString() ||
                                  user.Address == k.ToString()
                            select user;

                        return 1;
                    });

                    return 1;
                });
                return 1;
            });
        }

        [MQL("Aggregate([{ \"$match\" : { \"$or\" : [{ \"FieldString\" : { \"$regularExpression\" : { \"pattern\" : \"Bob\", \"options\" : \"s\" } } }, { \"FieldString\" : { \"$regularExpression\" : { \"pattern\" : localVariable, \"options\" : \"s\" } } }] } }])")]
        [MQL("Aggregate([{ \"$match\" : { \"$or\" : [{ \"FieldString\" : { \"$regularExpression\" : { \"pattern\" : \"Bob\", \"options\" : \"s\" } } }, { \"FieldString\" : { \"$regularExpression\" : { \"pattern\" : localVariable, \"options\" : \"s\" } } }] } }])")]
        public void Regex()
        {
            var localVariable = "Alice";

            _ = GetMongoQueryable<MixedDataMembers>().Where(u =>
                u.FieldString.Contains("Bob") ||
                u.FieldString.Contains(localVariable));

            _ = from mixedDataMember in GetMongoQueryable<MixedDataMembers>()
                where mixedDataMember.FieldString.Contains("Bob") ||
                      mixedDataMember.FieldString.Contains(localVariable)
                select mixedDataMember;
        }

        [MQL("Aggregate([{ \"$project\" : { \"Name\" : \"$Name\", \"City\" : \"$Address.City\", \"_id\" : 0 } }])")]
        [MQL("Aggregate([{ \"$project\" : { \"Name\" : \"$Name\", \"City\" : \"$Address.City\", \"_id\" : 0 } }])")]
        public void Select_anonymous()
        {
            _ = GetMongoQueryable<Person>()
                .Select(p => new { p.Name, p.Address.City });

            _ = from person in GetMongoQueryable<Person>()
                select new { person.Name, person.Address.City };
        }

        [MQL("Aggregate([{ \"$project\" : { \"Name\" : \"$Name\", \"City\" : \"$Address.City\", \"MailCount\" : { \"$literal\" : variableInt }, \"Category\" : \"Residential\", \"_id\" : 0 } }])")]
        [MQL("Aggregate([{ \"$project\" : { \"Name\" : \"$Name\", \"City\" : \"$Address.City\", \"MailCount\" : { \"$literal\" : variableInt }, \"Category\" : \"Residential\", \"_id\" : 0 } }])")]
        public void Select_anonymous_with_constants_and_variables()
        {
            var variableInt = 123;

            _ = GetMongoQueryable<Person>()
                .Select(p => new { p.Name, p.Address.City, MailCount = variableInt, Category = "Residential" });

            _ = from person in GetMongoQueryable<Person>()
                select new { person.Name, person.Address.City, MailCount = variableInt, Category = "Residential" };
        }

        [MQL("Aggregate([{ \"$project\" : { \"_v\" : \"$Scores\", \"_id\" : 0 } }, { \"$unwind\" : \"$_v\" }])")]
        [MQL("Aggregate([{ \"$project\" : { \"_v\" : \"$Scores\", \"_id\" : 0 } }, { \"$unwind\" : \"$_v\" }])")]
        public void SelectMany()
        {
            _ = GetMongoQueryable<User>()
                .SelectMany(u => u.Scores);

            _ = from user in GetMongoQueryable<User>()
                from score in user.Scores
                select score;
        }

        [MQL("Aggregate([{ \"$match\" : { \"Name\" : \"Bob\", \"Age\" : { \"$gt\" : 16, \"$lte\" : 21 } } }])")]
        [MQL("Aggregate([{ \"$match\" : { \"Name\" : \"Bob\", \"Age\" : { \"$gt\" : 16, \"$lte\" : 21 } } }])")]
        public void SingleLine_expression()
        {
            _ = GetMongoCollection().AsQueryable().Where(u => u.Name == "Bob" && u.Age > 16 && u.Age <= 21);

            _ = from user in GetMongoCollection().AsQueryable()
                where user.Name == "Bob" && user.Age > 16 && user.Age <= 21
                select user;
        }

        [MQL("Aggregate([{ \"$project\" : { \"_v\" : \"$LastName\", \"_id\" : 0 } }])")]
        [MQL("Aggregate([{ \"$project\" : { \"_v\" : \"$LastName\", \"_id\" : 0 } }])")]
        public void SingleLine_expression_select_only()
        {
            _ = GetMongoQueryable().Select(u => u.LastName);

            _ = from user in GetMongoQueryable()
                select user.LastName;
        }

        [MQL("Aggregate([{ \"$match\" : { \"Name\" : \"Bob\", \"Age\" : { \"$gt\" : 16, \"$lte\" : 21 } } }])", 1)]
        [MQL("Aggregate([{ \"$match\" : { \"Name\" : \"John\", \"Age\" : { \"$gt\" : 45, \"$lte\" : 50 } } }])", 2)]
        [MQL("Aggregate([{ \"$match\" : { \"Name\" : \"Bob\", \"Age\" : { \"$gt\" : 16, \"$lte\" : 21 } } }])", 4)]
        [MQL("Aggregate([{ \"$match\" : { \"Name\" : \"John\", \"Age\" : { \"$gt\" : 45, \"$lte\" : 50 } } }])", 8)]
        public void Single_expression_variable_reassignment()
        {
            var x = GetMongoCollection().AsQueryable().Where(u => u.Name == "Bob" && u.Age > 16 && u.Age <= 21);
            x = GetMongoCollection().AsQueryable().Where(u => u.Name == "John" && u.Age > 45 && u.Age <= 50);

            x = from user in GetMongoCollection().AsQueryable()
                where user.Name == "Bob" && user.Age > 16 && user.Age <= 21
                select user;

            x = from user in GetMongoCollection().AsQueryable()
                where user.Name == "John" && user.Age > 45 && user.Age <= 50
                select user;
        }

        [MQL("Aggregate([{ \"$match\" : { \"Name\" : \"Bob\", \"Age\" : { \"$gt\" : 16, \"$lte\" : 21 } } }, { \"$match\" : { \"Height\" : 180 } }])")]
        [MQL("Aggregate([{ \"$match\" : { \"Name\" : \"Bob\", \"Age\" : { \"$gt\" : 16, \"$lte\" : 21 } } }, { \"$match\" : { \"Height\" : 180 } }])")]
        public void TwoLines_expression()
        {
            _ = GetMongoCollection().AsQueryable()
                .Where(u => u.Name == "Bob" && u.Age > 16 && u.Age <= 21)
                .Where(u => u.Height == 180);

            _ = from user in GetMongoCollection().AsQueryable()
                where user.Name == "Bob" && user.Age > 16 && user.Age <= 21
                where user.Height == 180
                select user;
        }
    }
}
