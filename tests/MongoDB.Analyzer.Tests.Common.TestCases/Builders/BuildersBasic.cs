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

        [BuildersMQL("{ \"Age\" : 1 }", 34, 34)]
        [BuildersMQL("{ \"Age\" : -1 }", 35, 35)]
        public void Sort_single_expression()
        {
            _ = Builders<User>.Sort.Ascending(u => u.Age);
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

        [BuildersMQL("{ \"Address\" : { \"$exists\" : false } }", 275)]
        [BuildersMQL("{ \"Address\" : { \"$exists\" : true } }", 276)]
        public void Single_expression_variable_reassignment()
        {
            var x = Builders<User>.Filter.Exists(u => u.Address, false);
            x = Builders<User>.Filter.Exists(u => u.Address, true);
        }

        [BuildersMQL("{ \"Age\" : 25 }", 291)]
        [BuildersMQL("{ \"Age\" : { \"$lt\" : 65 } }", 292)]
        [BuildersMQL("{ \"Age\" : { \"$gt\" : 11 } }", 294)]
        [BuildersMQL("{ \"Age\" : { \"$lt\" : 25 } }", 295)]
        [BuildersMQL("{ \"Age\" : 100 }", 297)]
        [BuildersMQL("{ \"Age\" : 11 }", 298)]
        [BuildersMQL("{ \"Age\" : 1 }", 300)]
        [BuildersMQL("{ \"Age\" : { \"$gt\" : 1 } }", 301)]
        [BuildersMQL("{ \"Age\" : 200 }", 303)]
        [BuildersMQL("{ \"Age\" : -1 }", 304)]
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

        [BuildersMQL("{ \"Address\" : \"1\" }", 315)]
        [BuildersMQL("{ \"Address\" : \"2\" }", 316)]
        [BuildersMQL("{ \"Address\" : \"3\" }", 317)]
        [BuildersMQL("{ \"Address\" : \"4\" }", 317)]
        [BuildersMQL("{ \"Age\" : { \"$lt\" : 15, \"$gt\" : 65 } }", 319)]
        [BuildersMQL("{ \"Age\" : { \"$lt\" : 17, \"$gt\" : 18 } }", 320)]
        public void Multiple_expression_variables_declaration_and_reassignment()
        {
            var x = Builders<User>.Filter.Eq(u => u.Address, "1");
            var y = Builders<User>.Filter.Eq(u => u.Address, "2");
            FilterDefinition<User> z = Builders<User>.Filter.Eq(u => u.Address, "3"), w = (Builders<User>.Filter.Eq(u => u.Address, "4"));

            x = y = Builders<User>.Filter.Lt(u => u.Age, 15) & Builders<User>.Filter.Gt(u => u.Age, 65);
            x = z = w = y = x = z = w = y = Builders<User>.Filter.Lt(u => u.Age, 17) & Builders<User>.Filter.Gt(u => u.Age, 18);
        }

        [BuildersMQL("{ \"Age\" : 1 }")]
        public void builders_projection_1()
        {
            _ = Builders<User>.Projection.Include(u => u.Age);
        }

        [BuildersMQL("{ \"Address\" : 1, \"LastName\" : 1 }")]
        public void builders_projection_2()
        {
            _ = Builders<Person>.Projection.Include(u => u.Address).Include(u => u.LastName);
        }

        [BuildersMQL("{ \"Name\" : 1, \"TicksSinceBirth\" : 1, \"Vehicle\" : 0 }")]
        public void builders_projection_3()
        {
            _ = Builders<Person>.Projection.Include(u => u.Name).Include(u => u.TicksSinceBirth)
                .Exclude(u => u.Vehicle);
        }

        [BuildersMQL("{ \"LicenceNumber\" : 0, \"VehicleType\" : 1 }")]
        public void builders_projection_4()
        {
            _ = Builders<Vehicle>.Projection.Exclude(v => v.LicenceNumber).Include(v => v.VehicleType);
        }

        [BuildersMQL("{ \"Name\" : 1, \"TicksSinceBirth\" : 0, \"Vehicle\" : 1, \"Address\" : 0 }")]
        public void builders_projection_5()
        {
            _ = Builders<Person>.Projection.Include(u => u.Name).Exclude(u => u.TicksSinceBirth)
                .Include(u => u.Vehicle).Exclude(u => u.Address);
        }

        [BuildersMQL("{ \"Address\" : 1, \"_id\" : 0 }")]
        public void builders_projection_expression_1()
        {
            _ = Builders<Person>.Projection.Expression(u => u.Address);
        }

        [BuildersMQL("{ \"Address\" : 1, \"LastName\" : 1, \"Name\" : 1, \"_id\" : 0 }")]
        public void builders_projection_expression_2()
        {
            _ = Builders<User>.Projection.Expression(u => u.LastName.Length + u.Address.Length + u.Name.Length);
        }

        [BuildersMQL("{ \"Address\" : 1, \"LastName\" : 1, \"Name\" : 1, \"_id\" : 0 }")]
        public void builders_projection_expression_with_anonymous_object_1()
        {
            _ = Builders<User>.Projection.Expression(u => new { X = u.LastName, Y = u.Name, Z = u.Address });
        }

        [BuildersMQL("{ \"Age\" : 1, \"Height\" : 1, \"_id\" : 0 }")]
        public void builders_projection_expression_with_anonymous_object_2()
        {
            _ = Builders<User>.Projection.Expression(u => new { Avg = (u.Age + u.Height + u.Age) / 3 });
        }

        [BuildersMQL("{ \"Address\" : 1, \"LastName\" : 1, \"Name\" : 1, \"SiblingsCount\" : 1, \"TicksSinceBirth\" : 1, \"_id\" : 0 }")]
        public void builders_projection_expression_with_anonymous_object_3()
        {
            _ = Builders<Person>.Projection.Expression(u => new { Address = u.Address, Name = u.Name, LastName = u.LastName, CustomField = (u.SiblingsCount + (int)(u.TicksSinceBirth)) / 2 });
        }

        [BuildersMQL("{ \"Address.City\" : 1, \"Address.Province\" : 1, \"Address.ZipCode\" : 1, \"Vehicle.LicenceNumber\" : 1, \"Vehicle.VehicleType.Category\" : 1, \"_id\" : 0 }")]
        public void builders_projection_expression_with_anonymous_object_4()
        {
            _ = Builders<Person>.Projection.Expression(u => new { City = u.Address.City, Province = u.Address.Province, ZipCode = u.Address.ZipCode, LicenseNumber = u.Vehicle.LicenceNumber, Category = u.Vehicle.VehicleType.Category });
        }

        [BuildersMQL("{ \"Age\" : 1 }")]
        [BuildersMQL("{ \"Name\" : 1 }")]
        [BuildersMQL("{ \"Age\" : 1, \"Name\" : 1 }")]
        public void combined_projections()
        {
            var projection1 = Builders<User>.Projection.Include(u => u.Age);
            var projection2 = Builders<User>.Projection.Include(u => u.Name);
            _ = Builders<User>.Projection.Combine(Builders<User>.Projection.Include(u => u.Age), Builders<User>.Projection.Include(u => u.Name));
        }

        [BuildersMQL("{ \"PesonsList\" : { \"$elemMatch\" : { \"SiblingsCount\" : { \"$lt\" : 11 } } } }")]
        public void projection_elem_match_1()
        {
            _ = Builders<ListsHolder>.Projection.ElemMatch(u => u.PesonsList, g => g.SiblingsCount < 11);
        }

        [BuildersMQL("{ \"PesonsList\" : { \"$elemMatch\" : { \"SiblingsCount\" : { \"$gt\" : 11, \"$lt\" : 15 } } } }")]
        public void projection_elem_match_2()
        {
            _ = Builders<ListsHolder>.Projection.ElemMatch(u => u.PesonsList, Builders<Person>.Filter.Gt(u => u.SiblingsCount, 11) & Builders<Person>.Filter.Lt(u => u.SiblingsCount, 15));
        }

        [BuildersMQL("{ \"PesonsList\" : { \"$elemMatch\" : { \"SiblingsCount\" : { \"$lt\" : 12, \"$gt\" : 3 } } } }")]
        public void projection_elem_match_3()
        {
            _ = Builders<ListsHolder>.Projection.ElemMatch(u => u.PesonsList, g => g.SiblingsCount < 12 && g.SiblingsCount > 3);
        }

        [BuildersMQL("{ \"PesonsList\" : { \"$elemMatch\" : { \"SiblingsCount\" : { \"$lt\" : 12, \"$gt\" : 3 } } }, \"NestedListsHolderIList\" : { \"$elemMatch\" : { \"PesonsList\" : { \"$size\" : 22 } } } }")]
        public void projection_elem_match_4()
        {
            _ = Builders<ListsHolder>.Projection.ElemMatch(u => u.PesonsList, g => g.SiblingsCount < 12 && g.SiblingsCount > 3).ElemMatch(u => u.NestedListsHolderIList, g => g.PesonsList.Count == 22);
        }

        [BuildersMQL("{ \"IntArray\" : { \"$slice\" : [10, 5] } }")]
        public void projection_slice_1()
        {
            _ = Builders<SimpleTypesArraysHolder>.Projection.Slice(u => u.IntArray, 10, 5);
        }

        [BuildersMQL("{ \"IntArray\" : { \"$slice\" : [10, 5] }, \"JaggedStringArray2\" : { \"$slice\" : [3, 9] } }")]
        public void projection_slice_2()
        {
            _ = Builders<SimpleTypesArraysHolder>.Projection.Slice(u => u.IntArray, 10, 5).Slice(u => u.JaggedStringArray2, 3, 9);
        }

        [NoDiagnostics]
        public void projection_as_expression()
        {
            _ = Builders<User>.Projection.As<Person>();
        }
    }
}
