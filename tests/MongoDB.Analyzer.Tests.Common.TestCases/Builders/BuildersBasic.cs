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
        [BuildersMQL("{ \"$or\" : [{ \"name\" : \"Alice\" }, { \"lastName\" : { \"$regularExpression\" : { \"pattern\" : \"berg\", \"options\" : \"\" } } }] }")]
        public void BsonDocument()
        {
            _ = Builders<BsonDocument>.Filter.Eq("name", "Alice") |
                Builders<BsonDocument>.Filter.Regex("lastName", "berg");
        }

        [BuildersMQL("{ \"field\" : { \"$elemMatch\" : { \"field\" : fieldValue } } }")]
        public void BsonValue()
        {
            var fieldValue = "fieldValue";
            _ = Builders<BsonDocument>.Filter.ElemMatch("field", Builders<BsonValue>.Filter.Eq("field", fieldValue));
        }

        [BuildersMQL("{ \"StringField\" : \"value\" }")]
        public void Datatype_with_ObjectId_property()
        {
            _ = Builders<ClassWithObjectId>.Filter.Eq(c => c.StringField, "value");
        }

        [BuildersMQL("{ \"field\" : { \"$elemMatch\" : { \"field\" : fieldValue } } }")]
        public void ElemMatch()
        {
            var fieldValue = "fieldValue";
            _ = Builders<BsonDocument>.Filter.ElemMatch("field", Builders<BsonDocument>.Filter.Eq("field", fieldValue));
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
        public void Filters_combined_by_operators()
        {
            _ = Builders<User>.Filter.Lt(u => u.Age, 10) |
                Builders<User>.Filter.Gt(u => u.Age, 20) |
                (Builders<User>.Filter.Ne(u => u.Name, "Bob") &
                 Builders<User>.Filter.Exists(u => u.LastName));
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

        [BuildersMQL("find({ \"$or\" : [{ \"Age\" : { \"$lt\" : 10 } }, { \"Age\" : { \"$gt\" : 20 } }, { \"Name\" : { \"$ne\" : \"Bob\" }, \"LastName\" : { \"$exists\" : true } }] })")]
        public void Filters_combined_inside_collection_find()
        {
            GetMongoCollection().Find(Builders<User>.Filter.Lt(u => u.Age, 10) |
                Builders<User>.Filter.Gt(u => u.Age, 20) |
                (Builders<User>.Filter.Ne(u => u.Name, "Bob") &
                 Builders<User>.Filter.Exists(u => u.LastName)));
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

        [BuildersMQL("{ \"Address\" : { \"$exists\" : false } }")]
        public void Filters_single_expression_exists()
        {
            _ = Builders<User>.Filter.Exists(u => u.Address, false);
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

        [BuildersMQL("{ \"Root.Data\" : intVariable1 + intVariable2 }")]
        [BuildersMQL("{ \"VehicleType.MPG\" : intVariable1 + intVariable2 }")]
        [BuildersMQL("{ \"TicksSinceBirth\" : intVariable1 + intVariable2 }")]
        [BuildersMQL("{ \"VehicleType.MPG\" : uintVariable1 + uintVariable2 }")]
        [BuildersMQL("{ \"TicksSinceBirth\" : uintVariable1 + uintVariable2 }")]
        [BuildersMQL("{ \"Root.Data\" : shortVariable1 + shortVariable2 }")]
        [BuildersMQL("{ \"VehicleType.MPG\" : shortVariable1 + shortVariable2 }")]
        [BuildersMQL("{ \"TicksSinceBirth\" : shortVariable1 + shortVariable2 }")]
        [BuildersMQL("{ \"Root.Data\" : ushortVariable1 + ushortVariable2 }")]
        [BuildersMQL("{ \"VehicleType.MPG\" : ushortVariable1 + ushortVariable2 }")]
        [BuildersMQL("{ \"TicksSinceBirth\" : ushortVariable1 + ushortVariable2 }")]
        [BuildersMQL("{ \"Data\" : byteVariable1 + byteVariable2 }")]
        [BuildersMQL("{ \"VehicleType.MPG\" : byteVariable1 + byteVariable2 }")]
        [BuildersMQL("{ \"TicksSinceBirth\" : byteVariable1 + byteVariable2 }")]
        [BuildersMQL("{ \"Data\" : sbyteVariable1 + sbyteVariable2 }")]
        [BuildersMQL("{ \"VehicleType.MPG\" : sbyteVariable1 + sbyteVariable2 }")]
        [BuildersMQL("{ \"TicksSinceBirth\" : sbyteVariable1 + sbyteVariable2 }")]
        [BuildersMQL("{ \"Data\" : longVariable1 + longVariable2 }")]
        [BuildersMQL("{ \"VehicleType.MPG\" : longVariable1 + longVariable2 }")]
        [BuildersMQL("{ \"TicksSinceBirth\" : longVariable1 + longVariable2 }")]
        [BuildersMQL("{ \"VehicleType.MPG\" : ulongVariable1 + ulongVariable2 }")]
        [BuildersMQL("{ \"Root.Data\" : doubleVariable1 + doubleVariable2 }")]
        [BuildersMQL("{ \"VehicleType.MPG\" : doubleVariable1 + doubleVariable2 }")]
        [BuildersMQL("{ \"TicksSinceBirth\" : doubleVariable1 + doubleVariable2 }")]
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

            _ = Builders<Tree>.Filter.Eq(t => t.Root.Data, intVariable1 + intVariable2);
            _ = Builders<Vehicle>.Filter.Eq(v => v.VehicleType.MPG, intVariable1 + intVariable2);
            _ = Builders<Person>.Filter.Eq(p => p.TicksSinceBirth, intVariable1 + intVariable2);

            _ = Builders<Vehicle>.Filter.Eq(v => v.VehicleType.MPG, uintVariable1 + uintVariable2);
            _ = Builders<Person>.Filter.Eq(p => p.TicksSinceBirth, uintVariable1 + uintVariable2);

            _ = Builders<Tree>.Filter.Eq(t => t.Root.Data, shortVariable1 + shortVariable2);
            _ = Builders<Vehicle>.Filter.Eq(v => v.VehicleType.MPG, shortVariable1 + shortVariable2);
            _ = Builders<Person>.Filter.Eq(p => p.TicksSinceBirth, shortVariable1 + shortVariable2);

            _ = Builders<Tree>.Filter.Eq(t => t.Root.Data, ushortVariable1 + ushortVariable2);
            _ = Builders<Vehicle>.Filter.Eq(v => v.VehicleType.MPG, ushortVariable1 + ushortVariable2);
            _ = Builders<Person>.Filter.Eq(p => p.TicksSinceBirth, ushortVariable1 + ushortVariable2);

            _ = Builders<TreeNode>.Filter.Eq(n => n.Data, byteVariable1 + byteVariable2);
            _ = Builders<Vehicle>.Filter.Eq(v => v.VehicleType.MPG, byteVariable1 + byteVariable2);
            _ = Builders<Person>.Filter.Eq(p => p.TicksSinceBirth, byteVariable1 + byteVariable2);

            _ = Builders<TreeNode>.Filter.Eq(n => n.Data, sbyteVariable1 + sbyteVariable2);
            _ = Builders<Vehicle>.Filter.Eq(v => v.VehicleType.MPG, sbyteVariable1 + sbyteVariable2);
            _ = Builders<Person>.Filter.Eq(p => p.TicksSinceBirth, sbyteVariable1 + sbyteVariable2);

            _ = Builders<TreeNode>.Filter.Eq(n => n.Data, longVariable1 + longVariable2);
            _ = Builders<Vehicle>.Filter.Eq(v => v.VehicleType.MPG, longVariable1 + longVariable2);
            _ = Builders<Person>.Filter.Eq(p => p.TicksSinceBirth, longVariable1 + longVariable2);

            _ = Builders<Vehicle>.Filter.Eq(v => v.VehicleType.MPG, ulongVariable1 + ulongVariable2);

            _ = Builders<Tree>.Filter.Eq(t => t.Root.Data, doubleVariable1 + doubleVariable2);
            _ = Builders<Vehicle>.Filter.Eq(v => v.VehicleType.MPG, doubleVariable1 + doubleVariable2);
            _ = Builders<Person>.Filter.Eq(p => p.TicksSinceBirth, doubleVariable1 + doubleVariable2);
        }

        [BuildersMQL("{ \"Address\" : \"1\" }", 1)]
        [BuildersMQL("{ \"Address\" : \"2\" }", 2)]
        [BuildersMQL("{ \"Address\" : \"3\" }", 3)]
        [BuildersMQL("{ \"Address\" : \"4\" }", 3)]
        [BuildersMQL("{ \"Age\" : { \"$lt\" : 15, \"$gt\" : 65 } }", 5)]
        [BuildersMQL("{ \"Age\" : { \"$lt\" : 17, \"$gt\" : 18 } }", 6)]
        public void Multiple_expression_variables_declaration_and_reassignment()
        {
            var x = Builders<User>.Filter.Eq(u => u.Address, "1");
            var y = Builders<User>.Filter.Eq(u => u.Address, "2");
            FilterDefinition<User> z = Builders<User>.Filter.Eq(u => u.Address, "3"), w = (Builders<User>.Filter.Eq(u => u.Address, "4"));

            x = y = Builders<User>.Filter.Lt(u => u.Age, 15) & Builders<User>.Filter.Gt(u => u.Age, 65);
            x = z = w = y = x = z = w = y = Builders<User>.Filter.Lt(u => u.Age, 17) & Builders<User>.Filter.Gt(u => u.Age, 18);
        }

        [BuildersMQL("{ \"Age\" : 25 }", 1)]
        [BuildersMQL("{ \"Age\" : { \"$lt\" : 65 } }", 2)]
        [BuildersMQL("{ \"Age\" : { \"$gt\" : 11 } }", 4)]
        [BuildersMQL("{ \"Age\" : { \"$lt\" : 25 } }", 5)]
        [BuildersMQL("{ \"Age\" : 100 }", 7)]
        [BuildersMQL("{ \"Age\" : 11 }", 8)]
        [BuildersMQL("{ \"Age\" : 1 }", 10)]
        [BuildersMQL("{ \"Age\" : { \"$gt\" : 1 } }", 11)]
        [BuildersMQL("{ \"Age\" : 200 }", 13)]
        [BuildersMQL("{ \"Age\" : -1 }", 14)]
        public void Multiple_expression_variables_reassignment()
        {
            var x = Builders<User>.Filter.Eq(u => u.Age, 25);
            var y = Builders<User>.Filter.Lt(u => u.Age, 65);

            x = Builders<User>.Filter.Gt(u => u.Age, 11);
            var z = Builders<User>.Filter.Lt(u => u.Age, 25);

            y = Builders<User>.Filter.Eq(u => u.Age, 100);
            x = Builders<User>.Filter.Eq(u => u.Age, 11);

            var w = Builders<User>.Sort.Ascending(u => u.Age);
            z = Builders<User>.Filter.Gt(u => u.Age, 1);

            y = Builders<User>.Filter.Eq(u => u.Age, 200);
            w = Builders<User>.Sort.Descending(u => u.Age);
        }

        [BuildersMQL("{ \"$pull\" : { \"Children\" : { \"Data\" : 1 } } }")]
        public void Pull()
        {
            _ = Builders<NestedArrayHolder>.Update.PullFilter(t => t.Children, c => c.Data == 1);
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

        [BuildersMQL("{ \"Address.ZipCode\" : i.ToString() }")]
        public void Referencing_lambda_parameter()
        {
            _ = Enumerable.Range(0, 10).Select(i =>
            {
                var filter = Builders<Person>.Filter.Eq(o => o.Address.ZipCode, i.ToString());
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

        [BuildersMQL("{ \"$or\" : [{ \"Name\" : { \"$regularExpression\" : { \"pattern\" : \"Bob\", \"options\" : \"\" } } }, { \"Name\" : { \"$regularExpression\" : { \"pattern\" : localVariable, \"options\" : \"\" } } }] }")]
        public void Regex()
        {
            var localVariable = "Alice";

            _ = Builders<User>.Filter.Regex(u => u.Name, "Bob") |
                Builders<User>.Filter.Regex(u => u.Name, localVariable);
        }

        [BuildersMQL("{ \"Address\" : { \"$exists\" : false } }", 1)]
        [BuildersMQL("{ \"Address\" : { \"$exists\" : true } }", 2)]
        public void Single_expression_variable_reassignment()
        {
            var x = Builders<User>.Filter.Exists(u => u.Address, false);
            x = Builders<User>.Filter.Exists(u => u.Address, true);
        }

        [BuildersMQL("{ \"Age\" : -1, \"Name\" : 1, \"Address\" : 1 }")]
        public void Sort_chained()
        {
            _ = Builders<User>.Sort
                .Descending(u => u.Age)
                .Ascending(u => u.Name)
                .Ascending(u => u.Address);
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

        [BuildersMQL("{ \"Age\" : 1 }")]
        [BuildersMQL("{ \"Age\" : -1 }")]
        public void Sort_single_expression()
        {
            _ = Builders<User>.Sort.Ascending(u => u.Age);
            _ = Builders<User>.Sort.Descending(u => u.Age);
        }

        [BuildersMQL("{ \"Name\" : { \"$type\" : 2 } }")]
        public void Type()
        {
            _ = Builders<Person>.Filter.Type(c => c.Name, BsonType.String);
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
    }
}
