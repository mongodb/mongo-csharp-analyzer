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
        [MQL("aggregate([{ \"$match\" : { \"Name\" : \"Bob\", \"Age\" : { \"$gt\" : 16, \"$lte\" : 21 } } }])")]
        public void SingleLine_expression()
        {
            _ = GetMongoCollection().AsQueryable().Where(u => u.Name == "Bob" && u.Age > 16 && u.Age <= 21);

            _ = from user in GetMongoCollection().AsQueryable()
                where user.Name == "Bob" && user.Age > 16 && user.Age <= 21
                select user;
        }

        [MQL("aggregate([{ \"$project\" : { \"LastName\" : \"$LastName\", \"_id\" : 0 } }])")]
        [MQL("aggregate([{ \"$project\" : { \"LastName\" : \"$LastName\", \"_id\" : 0 } }])")]
        public void SingleLine_expression_select_only()
        {
            _ = GetMongoQueryable().Select(u => u.LastName);

            _ = from user in GetMongoQueryable()
                select user.LastName;
        }

        [MQL("aggregate([{ \"$match\" : { \"Name\" : \"Bob\", \"Age\" : { \"$gt\" : 16, \"$lte\" : 21 } } }, { \"$match\" : { \"Height\" : 180 } }])")]
        [MQL("aggregate([{ \"$match\" : { \"Name\" : \"Bob\", \"Age\" : { \"$gt\" : 16, \"$lte\" : 21 } } }, { \"$match\" : { \"Height\" : 180 } }])")]
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

        [MQL("aggregate([{ \"$match\" : { \"Name\" : \"Bob\", \"Age\" : { \"$gt\" : 16, \"$lte\" : 21 } } }, { \"$match\" : { \"Height\" : 180 } }, { \"$project\" : { \"Scores\" : \"$Scores\", \"_id\" : 0 } }])")]
        [MQL("aggregate([{ \"$match\" : { \"Name\" : \"Bob\", \"Age\" : { \"$gt\" : 16, \"$lte\" : 21 } } }, { \"$match\" : { \"Height\" : 180 } }, { \"$project\" : { \"Scores\" : \"$Scores\", \"_id\" : 0 } }])")]
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

        [MQL("aggregate([{ \"$match\" : { \"Name\" : \"Bob\", \"Age\" : { \"$gt\" : 16, \"$lte\" : 21 } } }, { \"$match\" : { \"Height\" : { \"$lt\" : 180 } } }, { \"$match\" : { \"LastName\" : \"Smith\" } }, { \"$project\" : { \"Height\" : \"$Height\", \"_id\" : 0 } }])")]
        [MQL("aggregate([{ \"$match\" : { \"Name\" : \"Bob\", \"Age\" : { \"$gt\" : 16, \"$lte\" : 21 } } }, { \"$match\" : { \"Height\" : { \"$lt\" : 180 } } }, { \"$match\" : { \"LastName\" : \"Smith\" } }, { \"$project\" : { \"Height\" : \"$Height\", \"_id\" : 0 } }])")]
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

        [MQL("aggregate([{ \"$match\" : { \"FieldString\" : \"Bob\", \"PropertyArray.0\" : 1 } }, { \"$match\" : { \"FieldMixedDataMembers.FieldString\" : \"Alice\" } }])")]
        [MQL("aggregate([{ \"$match\" : { \"FieldString\" : \"Bob\", \"PropertyArray.0\" : 1 } }, { \"$match\" : { \"FieldMixedDataMembers.FieldString\" : \"Alice\" } }])")]
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

        [MQL("aggregate([{ \"$match\" : { \"$or\" : [{ \"FieldString\" : /Bob/s }, { \"FieldString\" : /localVariable/s }] } }])")]
        [MQL("aggregate([{ \"$match\" : { \"$or\" : [{ \"FieldString\" : /Bob/s }, { \"FieldString\" : /localVariable/s }] } }])")]
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

        [MQL("aggregate([{ \"$group\" : { \"_id\" : \"$SiblingsCount\" } }])")]
        [MQL("aggregate([{ \"$group\" : { \"_id\" : \"$SiblingsCount\" } }])")]
        public void GroupBy()
        {
            _ = GetMongoQueryable<Person>()
                .GroupBy(p => p.SiblingsCount);

            _ = from person in GetMongoQueryable<Person>()
                group person by person.SiblingsCount into newGroup
                select newGroup;
        }

        [MQL("aggregate([{ \"$sort\" : { \"Name\" : 1 } }])")]
        [MQL("aggregate([{ \"$sort\" : { \"Name\" : 1 } }])")]
        public void OrderBy()
        {
            _ = GetMongoQueryable<Person>()
                .OrderBy(p => p.Name);

            _ = from person in GetMongoQueryable<Person>()
                orderby person.Name
                select person;
        }

        [MQL("aggregate([{ \"$unwind\" : \"$Scores\" }, { \"$project\" : { \"Scores\" : \"$Scores\", \"_id\" : 0 } }])")]
        [MQL("aggregate([{ \"$unwind\" : \"$Scores\" }, { \"$project\" : { \"Scores\" : \"$Scores\", \"_id\" : 0 } }])")]
        public void SelectMany()
        {
            _ = GetMongoQueryable<User>()
                .SelectMany(u => u.Scores);

            _ = from user in GetMongoQueryable<User>()
                from score in user.Scores
                select score;
        }

        [MQL("aggregate([{ \"$project\" : { \"Name\" : \"$Name\", \"City\" : \"$Address.City\", \"_id\" : 0 } }])")]
        [MQL("aggregate([{ \"$project\" : { \"Name\" : \"$Name\", \"City\" : \"$Address.City\", \"_id\" : 0 } }])")]
        public void Select_anonymous()
        {
            _ = GetMongoQueryable<Person>()
                .Select(p => new { p.Name, p.Address.City });

            _ = from person in GetMongoQueryable<Person>()
                select new { person.Name, person.Address.City };
        }

        [MQL("aggregate([{ \"$project\" : { \"Name\" : \"$Name\", \"City\" : \"$Address.City\", \"MailCount\" : variableInt, \"Category\" : \"Residential\", \"_id\" : variableInt } }])")]
        [MQL("aggregate([{ \"$project\" : { \"Name\" : \"$Name\", \"City\" : \"$Address.City\", \"MailCount\" : variableInt, \"Category\" : \"Residential\", \"_id\" : variableInt } }])")]
        public void Select_anonymous_with_constants_and_variables()
        {
            var variableInt = 123;

            _ = GetMongoQueryable<Person>()
                .Select(p => new { p.Name, p.Address.City, MailCount = variableInt, Category = "Residential" });

            _ = from person in GetMongoQueryable<Person>()
                select new { person.Name, person.Address.City, MailCount = variableInt, Category = "Residential" };
        }

        [MQL("aggregate([{ \"$match\" : { \"StringField\" : \"value\" } }])")]
        [MQL("aggregate([{ \"$match\" : { \"StringField\" : \"value\" } }])")]
        public void Datatype_with_objectId()
        {
            _ = GetMongoQueryable<ClassWithObjectId>().Where(c => c.StringField == "value");

            _ = from classWithObjectId in GetMongoQueryable<ClassWithObjectId>()
                where classWithObjectId.StringField == "value"
                select classWithObjectId;
        }

        [MQL("aggregate([{ \"$match\" : { \"$or\" : [{ \"Age\" : i }, { \"Name\" : i.ToString() }] } }])")]
        [MQL("aggregate([{ \"$match\" : { \"$or\" : [{ \"Age\" : i }, { \"Name\" : i.ToString() }] } }])")]
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

        [MQL("aggregate([{ \"$match\" : { \"LastName\" : lambdaUser.LastName } }, { \"$match\" : { \"Age\" : { \"$lt\" : lambdaUser.Age } } }])")]
        [MQL("aggregate([{ \"$match\" : { \"LastName\" : lambdaUser.LastName } }, { \"$match\" : { \"Age\" : { \"$lt\" : lambdaUser.Age } } }])")]
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

        [MQL("aggregate([{ \"$match\" : { \"$or\" : [{ \"Age\" : i }, { \"Scores.0\" : j }, { \"Scores\" : { \"$size\" : k } }, { \"Name\" : i.ToString() }, { \"LastName\" : j.ToString() }, { \"Address\" : k.ToString() }] } }])")]
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

        [MQL("aggregate([{ \"$match\" : { \"Name\" : \"Bob\", \"Age\" : { \"$gt\" : 16, \"$lte\" : 21 } } }])", startLine: 266)]
        [MQL("aggregate([{ \"$match\" : { \"Name\" : \"John\", \"Age\" : { \"$gt\" : 45, \"$lte\" : 50 } } }])", startLine: 267)]
        [MQL("aggregate([{ \"$match\" : { \"Name\" : \"Bob\", \"Age\" : { \"$gt\" : 16, \"$lte\" : 21 } } }])", startLine: 269)]
        [MQL("aggregate([{ \"$match\" : { \"Name\" : \"John\", \"Age\" : { \"$gt\" : 45, \"$lte\" : 50 } } }])", startLine: 273)]
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

        [MQL("aggregate([{ \"$match\" : { \"Age\" : 45 } }])", startLine: 300)]
        [MQL("aggregate([{ \"$match\" : { \"Name\" : \"John\" } }])", startLine: 301)]
        [MQL("aggregate([{ \"$match\" : { \"Name\" : \"Bob\", \"Age\" : { \"$gt\" : 16, \"$lte\" : 21 } } }])", startLine: 303)]
        [MQL("aggregate([{ \"$match\" : { \"Name\" : \"James\", \"Age\" : { \"$gt\" : 25, \"$lte\" : 99 } } }])", startLine: 304)]
        [MQL("aggregate([{ \"$match\" : { \"Name\" : \"Steve\" } }])", startLine: 306)]
        [MQL("aggregate([{ \"$match\" : { \"Height\" : 100 } }])", startLine: 307)]
        [MQL("aggregate([{ \"$match\" : { \"Age\" : 22 } }])", startLine: 309)]
        [MQL("aggregate([{ \"$match\" : { \"LastName\" : \"LastName\" } }])", startLine: 310)]
        [MQL("aggregate([{ \"$match\" : { \"Address\" : \"Address\" } }])", startLine: 312)]
        [MQL("aggregate([{ \"$match\" : { \"Age\" : { \"$lte\" : 122 } } }])", startLine: 313)]
        [MQL("aggregate([{ \"$match\" : { \"Age\" : 45 } }])", startLine: 315)]
        [MQL("aggregate([{ \"$match\" : { \"Name\" : \"John\" } }])", startLine: 319)]
        [MQL("aggregate([{ \"$match\" : { \"Name\" : \"Bob\", \"Age\" : { \"$gt\" : 16, \"$lte\" : 21 } } }])", startLine: 323)]
        [MQL("aggregate([{ \"$match\" : { \"Name\" : \"James\", \"Age\" : { \"$gt\" : 25, \"$lte\" : 99 } } }])", startLine: 327)]
        [MQL("aggregate([{ \"$match\" : { \"Name\" : \"Steve\" } }])", startLine: 331)]
        [MQL("aggregate([{ \"$match\" : { \"Height\" : 100 } }])", startLine: 335)]
        [MQL("aggregate([{ \"$match\" : { \"Age\" : 22 } }])", startLine: 339)]
        [MQL("aggregate([{ \"$match\" : { \"LastName\" : \"LastName\" } }])", startLine: 343)]
        [MQL("aggregate([{ \"$match\" : { \"Address\" : \"Address\" } }])", startLine: 347)]
        [MQL("aggregate([{ \"$match\" : { \"Age\" : { \"$lte\" : 122 } } }])", startLine: 351)]
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

        [MQL("aggregate([{ \"$match\" : { \"Name\" : \"Bob\", \"Age\" : { \"$gt\" : 16, \"$lte\" : 21 } } }])", startLine: 370)]
        [MQL("aggregate([{ \"$match\" : { \"Name\" : \"John\", \"Age\" : { \"$gt\" : 22, \"$lte\" : 25 } } }])", startLine: 371)]
        [MQL("aggregate([{ \"$match\" : { \"Name\" : \"Steve\", \"Age\" : { \"$gt\" : 27, \"$lte\" : 31 } } }])", startLine: 372)]
        [MQL("aggregate([{ \"$match\" : { \"LastName\" : \"LastName\" } }])", startLine: 372)]
        [MQL("aggregate([{ \"$match\" : { \"Address\" : \"Address\" } }])", startLine: 374)]
        [MQL("aggregate([{ \"$match\" : { \"Height\" : 180 } }])", startLine: 375)]
        [MQL("aggregate([{ \"$match\" : { \"Name\" : \"Bob\", \"Age\" : { \"$gt\" : 16, \"$lte\" : 21 } } }])", startLine: 377)]
        [MQL("aggregate([{ \"$match\" : { \"Name\" : \"John\", \"Age\" : { \"$gt\" : 22, \"$lte\" : 25 } } }])", startLine: 381)]
        [MQL("aggregate([{ \"$match\" : { \"Name\" : \"Steve\", \"Age\" : { \"$gt\" : 27, \"$lte\" : 31 } } }])", startLine: 385)]
        [MQL("aggregate([{ \"$match\" : { \"LastName\" : \"LastName\" } }])", startLine: 388)]
        [MQL("aggregate([{ \"$match\" : { \"Address\" : \"Address\" } }])", startLine: 392)]
        [MQL("aggregate([{ \"$match\" : { \"Height\" : 180 } }])", startLine: 396)]
        public void Multiple_expression_variables_declaration_and_reassignment()
        {
            var x = GetMongoCollection().AsQueryable().Where(u => u.Name == "Bob" && u.Age > 16 && u.Age <= 21);
            var y = GetMongoCollection().AsQueryable().Where(u => u.Name == "John" && u.Age > 22 && u.Age <= 25);
            IMongoQueryable<User> z = GetMongoCollection().AsQueryable().Where(u => u.Name == "Steve" && u.Age > 27 && u.Age <= 31), w = (GetMongoCollection().AsQueryable().Where(u => u.LastName == "LastName"));

            x = y = GetMongoCollection().AsQueryable().Where(u => u.Address == "Address");
            x = z = w = y = x = z = w = y = GetMongoCollection().AsQueryable().Where(u => u.Height == 180);

            var a = from user in GetMongoCollection().AsQueryable()
                    where user.Name == "Bob" && user.Age > 16 && user.Age <= 21
                    select user;

            var b = from user in GetMongoCollection().AsQueryable()
                    where user.Name == "John" && user.Age > 22 && user.Age <= 25
                    select user;

            IMongoQueryable<User> c = from user in GetMongoCollection().AsQueryable()
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
    }
}
