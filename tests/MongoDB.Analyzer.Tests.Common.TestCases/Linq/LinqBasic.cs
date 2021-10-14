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
    }
}
