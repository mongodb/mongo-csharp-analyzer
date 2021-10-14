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
using MongoDB.Bson;
using MongoDB.Driver;

namespace MongoDB.Analyzer.Tests.Common.TestCases.Builders
{
    public sealed class BuildersBasic : TestCasesBase
    {
        [BuildersMQL("{ \"Address\" : { \"$exists\" : false } }")]
        public void Filters_single_expression_exists()
        {
            _ = Builders<User>.Filter.Exists(u => u.Address, false);
        }

        [BuildersMQL("{ \"Age\" : 1 }")]
        public void Sort_single_expression_ascending()
        {
            _ = Builders<User>.Sort.Ascending(u => u.Age);
        }

        [BuildersMQL("{ \"Age\" : -1 }")]
        public void Sort_single_expression_descending()
        {
            _ = Builders<User>.Sort.Descending(u => u.Age);
        }

        [BuildersMQL("{ \"$set\" : { \"Age\" : 22 } }")]
        public void Update_single_expression_set()
        {
            _ = Builders<User>.Update.Set(u => u.Age, 22);
        }

        [BuildersMQL("{ \"$rename\" : { \"Name\" : \"Mateo\" } }")]
        public void Update_single_expression_rename()
        {
            _ = Builders<User>.Update.Rename(u => u.Name, "Mateo");
        }

        [BuildersMQL("{ \"$or\" : [{ \"Age\" : { \"$lt\" : 10 } }, { \"Age\" : { \"$gt\" : 20 } }, { \"Name\" : { \"$ne\" : \"Bob\" }, \"LastName\" : { \"$exists\" : true } }] }")]
        public void Filters_combined_by_operators()
        {
            _ = Builders<User>.Filter.Lt(u => u.Age, 10) |
                Builders<User>.Filter.Gt(u => u.Age, 20) |
                (Builders<User>.Filter.Ne(u => u.Name, "Bob") &
                 Builders<User>.Filter.Exists(u => u.LastName));
        }

        [BuildersMQL("{ \"$set\" : { \"Age\" : 22 } }")]
        public void Expression_parenthesized_1()
        {
            _ = (Builders<User>.Update.Set(u => u.Age, 22));
        }

        [BuildersMQL("{ \"$set\" : { \"Age\" : 22 } }")]
        public void Expression_parenthesized_2()
        {
            _ = (((Builders<User>.Update.Set(u => u.Age, 22))));
        }

        [BuildersMQL("{ \"Scores.1\" : 100, \"$or\" : [{ \"Age\" : { \"$lt\" : 10 } }, { \"Age\" : { \"$gt\" : 20 } }, { \"Name\" : { \"$ne\" : \"Bob\" }, \"LastName\" : { \"$exists\" : true } }] }")]
        public void Filters_parenthesized_3()
        {
            _ = (Builders<User>.Filter.Eq(u => u.Scores[1], 100) &
                (((Builders<User>.Filter.Lt(u => u.Age, 10)) |
                 Builders<User>.Filter.Gt(u => u.Age, 20)) |
                (Builders<User>.Filter.Ne(u => u.Name, "Bob") &
                 Builders<User>.Filter.Exists(u => u.LastName))));
        }

        [BuildersMQL("{ \"$or\" : [{ \"Age\" : { \"$lt\" : 10 } }, { \"Age\" : { \"$gt\" : 20 } }] }")]
        public void Expression_in_lambda_1()
        {
            Func<FilterDefinition<User>> action = () => Builders<User>.Filter.Lt(u => u.Age, 10) | Builders<User>.Filter.Gt(u => u.Age, 20);
        }

        [BuildersMQL("{ \"$or\" : [{ \"Age\" : { \"$lt\" : 10 } }, { \"Age\" : { \"$gt\" : 20 } }, { \"Name\" : { \"$ne\" : \"Bob\" }, \"LastName\" : { \"$exists\" : true } }] }")]
        public void Expression_in_lambda_2()
        {
            Func<FilterDefinition<User>> action = () =>
            {
                return Builders<User>.Filter.Lt(u => u.Age, 10) |
                Builders<User>.Filter.Gt(u => u.Age, 20) |
                (Builders<User>.Filter.Ne(u => u.Name, "Bob") &
                 Builders<User>.Filter.Exists(u => u.LastName));
            };
        }

        [BuildersMQL("{ \"Age\" : { \"$gt\" : 20 } }")]
        public void Filters_combined_by_operators_containing_arbitrary_filter()
        {
            var arbitraryFilter = GetFilterUser();
            var filter = arbitraryFilter & Builders<User>.Filter.Gt(u => u.Age, 20);
        }

        [BuildersMQL("{ \"Age\" : { \"$gt\" : 20 } }")]
        public void Filters_combined_by_operators_containing_arbitrary_filter_2()
        {
            var arbitraryFilter = GetFilterUser();
            var filter = ((arbitraryFilter) & Builders<User>.Filter.Gt(u => u.Age, 20));
        }

        [BuildersMQL("{ \"$or\" : [{ \"Age\" : { \"$lt\" : 10 } }, { \"Age\" : { \"$gt\" : 20 } }, { \"Name\" : { \"$ne\" : \"Bob\" }, \"LastName\" : { \"$exists\" : true } }] }")]
        public void Filters_within_complex_expression()
        {
            var arbitraryFilter1 = GetFilterUser();
            var arbitraryFilter2 = GetFilterUser();
            var arbitraryFilter3 = GetFilterUser();

            Func<FilterDefinition<User>> action = () =>
            {
                return (arbitraryFilter1 | arbitraryFilter2) &
                    (arbitraryFilter3 |
                     (Builders<User>.Filter.Lt(u => u.Age, 10) |
                      Builders<User>.Filter.Gt(u => u.Age, 20) |
                      (Builders<User>.Filter.Ne(u => u.Name, "Bob") &
                      Builders<User>.Filter.Exists(u => u.LastName))));
            };
        }

        [BuildersMQL("{ \"$or\" : [{ \"Age\" : { \"$lt\" : 10 } }, { \"Age\" : { \"$gt\" : 20 } }, { \"Name\" : { \"$ne\" : \"Bob\" }, \"LastName\" : { \"$exists\" : true } }] }")]
        public void Filters_combined_by_Filter_And_Or()
        {
            _ = Builders<User>.Filter.Or(
                    Builders<User>.Filter.Lt(u => u.Age, 10),
                    Builders<User>.Filter.Gt(u => u.Age, 20),
                    Builders<User>.Filter.And(
                        Builders<User>.Filter.Ne(u => u.Name, "Bob"),
                        Builders<User>.Filter.Exists(u => u.LastName)));
        }

        [BuildersMQL("{ \"$or\" : [{ \"Age\" : { \"$lt\" : 10 } }, { \"Age\" : { \"$gt\" : 20 } }, { \"Name\" : { \"$ne\" : \"Bob\" }, \"LastName\" : { \"$exists\" : true } }] }")]
        public void Filters_combined_inside_collection_find()
        {
            GetMongoCollection().Find(Builders<User>.Filter.Lt(u => u.Age, 10) |
                Builders<User>.Filter.Gt(u => u.Age, 20) |
                (Builders<User>.Filter.Ne(u => u.Name, "Bob") &
                 Builders<User>.Filter.Exists(u => u.LastName)));
        }

        [BuildersMQL("{ \"Age\" : -1, \"Name\" : 1, \"Address\" : 1 }")]
        public void Sort_combined()
        {
            _ = Builders<User>.Sort.Combine(
                    Builders<User>.Sort.Descending(u => u.Age),
                    Builders<User>.Sort.Ascending(u => u.Name),
                    Builders<User>.Sort.Ascending(u => u.Address));
        }

        [BuildersMQL("{ \"Age\" : -1, \"Name\" : 1, \"Address\" : 1 }")]
        public void Sort_combined_inside_collection_find()
        {
            _ = Builders<User>.Sort.Combine(
                    Builders<User>.Sort.Descending(u => u.Age),
                    Builders<User>.Sort.Ascending(u => u.Name),
                    Builders<User>.Sort.Ascending(u => u.Address));
        }

        [BuildersMQL("{ \"Age\" : -1, \"Name\" : 1, \"Address\" : 1 }")]
        public void Sort_chained()
        {
            _ = Builders<User>.Sort
                .Descending(u => u.Age)
                .Ascending(u => u.Name)
                .Ascending(u => u.Address);
        }

        [BuildersMQL("{ \"$or\" : [{ \"Name\" : /Bob/ }, { \"Name\" : /localVariable/ }] }")]
        public void Regex()
        {
            var localVariable = "Alice";

            _ = Builders<User>.Filter.Regex(u => u.Name, "Bob") |
                Builders<User>.Filter.Regex(u => u.Name, localVariable);
        }

        [BuildersMQL("{ \"field\" : { \"$elemMatch\" : { \"field\" : fieldValue } } }")]
        public void ElemMatch()
        {
            var fieldValue = "fieldValue";
            _ = Builders<BsonDocument>.Filter.ElemMatch("field", Builders<BsonDocument>.Filter.Eq("field", fieldValue));
        }

        [BuildersMQL("{ \"$pull\" : { \"Children\" : { \"Data\" : 1 } } }")]
        public void Pull()
        {
            _ = Builders<NestedArrayHolder>.Update.PullFilter(t => t.Children, c => c.Data == 1);
        }

        [BuildersMQL("{ \"Name\" : { \"$type\" : 2 } }")]
        public void Type()
        {
            _ = Builders<Person>.Filter.Type(c => c.Name, BsonType.String);
        }

        [BuildersMQL("{ \"field\" : { \"$elemMatch\" : { \"field\" : fieldValue } } }")]
        public void BsonValue()
        {
            var fieldValue = "fieldValue";
            _ = Builders<BsonDocument>.Filter.ElemMatch("field", Builders<BsonValue>.Filter.Eq("field", fieldValue));
        }

        [BuildersMQL("{ \"$or\" : [{ \"name\" : \"Alice\" }, { \"lastName\" : /berg/ }] }")]
        public void BsonDocument()
        {
            _ = Builders<BsonDocument>.Filter.Eq("name", "Alice") |
                Builders<BsonDocument>.Filter.Regex("lastName", "berg");
        }

        [BuildersMQL("{ \"StringField\" : \"value\" }")]
        public void Datatype_with_ObjectId_property()
        {
            _ = Builders<ClassWithObjectId>.Filter.Eq(c => c.StringField, "value");
        }

        [BuildersMQL("{ \"Address.ZipCode\" : i.ToString() }")]
        public void Referencing_lambda_parameter()
        {
            _ = Enumerable.Range(0, 10).Select(i =>
            {
                var filter = Builders<Person>.Filter.Eq(o => o.Address.ZipCode, i.ToString());
                return 1;
            });
        }

        [BuildersMQL("{ \"LastName\" : lambdaPerson.LastName }")]
        public void Referencing_lambda_complex_parameter()
        {
            _ = Enumerable.Range(0, 10).Select(i => new Person()).Select(lambdaPerson =>
            {
                var filter = Builders<Person>.Filter.Eq(o => o.LastName, lambdaPerson.LastName);
                return 1;
            });
        }

        [BuildersMQL("{ \"$or\" : [{ \"Age\" : i }, { \"Scores.1\" : j }, { \"Scores.2\" : k }, { \"Address\" : i.ToString() }, { \"Name\" : j.ToString() }, { \"LastName\" : j.ToString() }] }")]
        public void Referencing_nested_lambda_parameter()
        {
            var ___ = Enumerable.Range(0, 10).Select(k =>
            {
                var __ = Enumerable.Range(0, 10).Select(j =>
                {
                    _ = Enumerable.Range(0, 10).Select(i =>
                    {
                        var f = Builders<User>.Filter.Eq(o => o.Age, i) |
                            Builders<User>.Filter.Eq(o => o.Scores[1], j) |
                            Builders<User>.Filter.Eq(o => o.Scores[2], k) |
                            Builders<User>.Filter.Eq(o => o.Address, i.ToString()) |
                            Builders<User>.Filter.Eq(o => o.Name, j.ToString()) |
                            Builders<User>.Filter.Eq(o => o.LastName, j.ToString());

                        return 1;
                    });

                    return 1;
                });
                return 1;
            });
        }
    }
}
