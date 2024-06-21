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


using MongoDB.Analyzer.Tests.Common.DataModel;
using MongoDB.Driver;

#pragma warning disable IDE0005
using builder = MongoDB.Driver.Builders<MongoDB.Analyzer.Tests.Common.DataModel.User>;
#pragma warning restore IDE0005

#pragma warning disable IDE0001

namespace MongoDB.Analyzer.Tests.Common.TestCases.Builders
{
    public sealed class BuildersDefinitions : TestCasesBase
    {
        public FilterDefinitionBuilder<User> GetFilterDefinitionBuilder() => Builders<User>.Filter;
        public static FilterDefinitionBuilder<User> GetFilterDefinitionBuilderStatic() => Builders<User>.Filter;

        public BuildersDefinitions GetClass() => new();
        public static BuildersDefinitions GetClassStatic() => new();

        [BuildersMQL("{ \"Name\" : \"User Name1\" }")]
        [BuildersMQL("{ \"$or\" : [{ \"Age\" : { \"$lt\" : 10 } }, { \"Age\" : { \"$gt\" : 20 } }, { \"Name\" : { \"$ne\" : \"Bob\" }, \"LastName\" : { \"$exists\" : true } }] }")]
        [BuildersMQL("{ \"Scores.1\" : 100, \"$or\" : [{ \"Age\" : { \"$lt\" : 10 } }, { \"Age\" : { \"$gt\" : 20 } }, { \"Name\" : { \"$ne\" : \"Bob\" }, \"LastName\" : { \"$exists\" : true } }] }")]
        [BuildersMQL("{ \"$or\" : [{ \"Age\" : { \"$lt\" : 10 } }, { \"Age\" : { \"$gt\" : 20 } }, { \"Name\" : { \"$ne\" : \"Bob\" }, \"LastName\" : { \"$exists\" : true } }] }")]
        public void FilterDefinition()
        {
            var filterDefinition = Builders<User>.Filter;

            _ = filterDefinition.Eq(u => u.Name, "User Name1");

            _ = filterDefinition.Lt(u => u.Age, 10) |
                filterDefinition.Gt(u => u.Age, 20) |
                (filterDefinition.Ne(u => u.Name, "Bob") &
                 filterDefinition.Exists(u => u.LastName));

            _ = filterDefinition.Eq(u => u.Scores[1], 100) &
                ((Builders<User>.Filter.Lt(u => u.Age, 10)) |
                 Builders<User>.Filter.Gt(u => u.Age, 20) |
                (Builders<User>.Filter.Ne(u => u.Name, "Bob") &
                 Builders<User>.Filter.Exists(u => u.LastName)));

            _ = Builders<User>.Filter.Lt(u => u.Age, 10) |
                Builders<User>.Filter.Gt(u => u.Age, 20) |
                (filterDefinition.Ne(u => u.Name, "Bob") &
                 filterDefinition.Exists(u => u.LastName));
        }

        [BuildersMQL("{ \"Address\" : -1 }")]
        [BuildersMQL("{ new { Field = \"Name\", IntValue = 10, BoolValue = false, DoubleValue = 2.5 }.Field : \"2d\", new { Field = \"LastName\", IntValue = 10, BoolValue = false, DoubleValue = 2.5 }.Field : \"2d\", new { Field = \"Address\", IntValue = 10, BoolValue = false, DoubleValue = 2.5 }.Field : \"text\", new { Field = \"Vehicle\", IntValue = 10, BoolValue = false, DoubleValue = 2.5 }.Field : \"text\" }")]
        [BuildersMQL("{ \"Name\" : \"2d\", \"LastName\" : \"2d\", \"Address\" : \"text\", \"Vehicle\" : \"text\" }")]
        [BuildersMQL("{ new { Field = \"Name\", IntValue = 10, BoolValue = false, DoubleValue = 2.5 }.Field : \"2d\", new { Field = \"LastName\", IntValue = 10, BoolValue = false, DoubleValue = 2.5 }.Field : \"2d\", new { Field = \"Address\", IntValue = 10, BoolValue = false, DoubleValue = 2.5 }.Field : \"text\", new { Field = \"Vehicle\", IntValue = 10, BoolValue = false, DoubleValue = 2.5 }.Field : \"text\" }")]
        public void IndexKeysDefinition()
        {
            var indexDefinition = Builders<Person>.IndexKeys;

            _ = indexDefinition.Descending(x => x.Address);

            _ = indexDefinition.Combine(
                    indexDefinition.Geo2D(new { Field = "Name", IntValue = 10, BoolValue = false, DoubleValue = 2.5 }.Field)
                        .Geo2D(new { Field = "LastName", IntValue = 10, BoolValue = false, DoubleValue = 2.5 }.Field),
                    indexDefinition.Text(new { Field = "Address", IntValue = 10, BoolValue = false, DoubleValue = 2.5 }.Field)
                        .Text(new { Field = "Vehicle", IntValue = 10, BoolValue = false, DoubleValue = 2.5 }.Field));

            _ = indexDefinition.Combine(Builders<Person>.IndexKeys.Geo2D(u => u.Name).Geo2D("LastName"), Builders<Person>.IndexKeys.Text(u => u.Address).Text("Vehicle"));

            _ = Builders<Person>.IndexKeys.Combine(
                    indexDefinition.Geo2D(new { Field = "Name", IntValue = 10, BoolValue = false, DoubleValue = 2.5 }.Field)
                        .Geo2D(new { Field = "LastName", IntValue = 10, BoolValue = false, DoubleValue = 2.5 }.Field),
                    indexDefinition.Text(new { Field = "Address", IntValue = 10, BoolValue = false, DoubleValue = 2.5 }.Field)
                        .Text(new { Field = "Vehicle", IntValue = 10, BoolValue = false, DoubleValue = 2.5 }.Field));
        }

        [BuildersMQL("{ \"Name\" : \"User Name1\" }")]
        [BuildersMQL("{ \"Name\" : \"User Name2\" }")]
        public void MethodTest()
        {
            _ = GetFilterDefinitionBuilder().Eq(u => u.Name, "User Name1");
            _ = GetClass().GetFilterDefinitionBuilder().Eq(u => u.Name, "User Name2");
        }

        [BuildersMQL("{ \"Age\" : 1 }")]
        [BuildersMQL("{ new { Field = \"Age\" }.Field : 1, new { Field = \"LastName\" }.Field : 1 }")]
        [BuildersMQL("{ \"Age\" : 1, \"Name\" : 1 }")]
        [BuildersMQL("{ new { Field = \"Age\" }.Field : 1, new { Field = \"LastName\" }.Field : 1 }")]
        public void ProjectionDefinition()
        {
            var projectionDefinition = Builders<User>.Projection;

            _ = projectionDefinition.Include(u => u.Age);

            _ = projectionDefinition.Combine(
                    projectionDefinition.Include(new { Field = "Age" }.Field),
                    projectionDefinition.Include(new { Field = "LastName" }.Field));

            _ = projectionDefinition.Combine(Builders<User>.Projection.Include(u => u.Age), Builders<User>.Projection.Include(u => u.Name));

            _ = Builders<User>.Projection.Combine(
                    projectionDefinition.Include(new { Field = "Age" }.Field),
                    projectionDefinition.Include(new { Field = "LastName" }.Field));
        }

        [BuildersMQL("{ \"equals\" : { \"value\" : true, \"path\" : \"IsRetired\" } }", DriverVersions.V2_19_OrGreater)]
        [BuildersMQL("{ \"span\" : { \"first\" : { \"operator\" : { \"term\" : { \"query\" : \"foo\", \"path\" : \"Name\" } }, \"endPositionLte\" : 5 } } }", DriverVersions.V2_19_OrGreater)]
        [BuildersMQL("{ \"span\" : { \"or\" : { \"clauses\" : [{ \"term\" : { \"query\" : \"a\", \"path\" : \"Name\" } }, { \"term\" : { \"query\" : \"b\", \"path\" : \"Name\" } }, { \"term\" : { \"query\" : \"c\", \"path\" : \"Name\" } }] } } }", DriverVersions.V2_19_OrGreater)]
        [BuildersMQL("{ \"span\" : { \"or\" : { \"clauses\" : [{ \"term\" : { \"query\" : \"a\", \"path\" : \"Name\" } }, { \"term\" : { \"query\" : \"b\", \"path\" : \"Name\" } }, { \"term\" : { \"query\" : \"c\", \"path\" : \"Name\" } }] } } }", DriverVersions.V2_19_OrGreater)]
        public void SearchDefinition()
        {
            var searchDefinition = Builders<Person>.Search;
            var searchSpanDefinition = Builders<Person>.SearchSpan;

            _ = searchDefinition.Equals(m => m.IsRetired, true);

            _ = searchDefinition.Span(Builders<Person>.SearchSpan
                .First(Builders<Person>.SearchSpan.Term(p => p.Name, "foo"), 5));

            _ = searchDefinition.Span(searchSpanDefinition.Or(
                    searchSpanDefinition.Term(p => p.Name, "a"),
                    searchSpanDefinition.Term(p => p.Name, "b"),
                    searchSpanDefinition.Term(p => p.Name, "c")));

            _ = Builders<Person>.Search.Span(searchSpanDefinition.Or(
                    searchSpanDefinition.Term(p => p.Name, "a"),
                    searchSpanDefinition.Term(p => p.Name, "b"),
                    searchSpanDefinition.Term(p => p.Name, "c")));
        }

        [BuildersMQL("{ new { Address = \"Address\" }.Address : 1, new { Name = \"Name\" }.Name : -1 }")]
        [BuildersMQL("{ \"Age\" : -1, \"Name\" : 1, \"Address\" : 1 }")]
        [BuildersMQL("{ \"Age\" : -1, \"Name\" : 1, \"Address\" : 1 }")]
        public void SortDefinition()
        {
            var sortDefinition = Builders<User>.Sort;

            _ = sortDefinition.Combine(
                    sortDefinition.Ascending(new { Address = "Address" }.Address),
                    sortDefinition.Descending(new { Name = "Name" }.Name));

            _ = sortDefinition.Combine(
                    Builders<User>.Sort.Descending(u => u.Age),
                    Builders<User>.Sort.Ascending(u => u.Name),
                    Builders<User>.Sort.Ascending(u => u.Address));

            _ = Builders<User>.Sort.Combine(
                    sortDefinition.Descending(u => u.Age),
                    sortDefinition.Ascending(u => u.Name),
                    sortDefinition.Ascending(u => u.Address));
        }

        [BuildersMQL("{ \"Name\" : \"User Name1\" }")]
        [BuildersMQL("{ \"Name\" : \"User Name2\" }")]
        public void StaticMethodTest()
        {
            _ = GetFilterDefinitionBuilderStatic().Eq(u => u.Name, "User Name1");
            _ = GetClassStatic().GetFilterDefinitionBuilder().Eq(u => u.Name, "User Name2");
        }

        [BuildersMQL("{ \"$set\" : { \"Age\" : 22 } }")]
        public void UpdateDefinition()
        {
            var updateDefinition = Builders<User>.Update;
            _ = updateDefinition.Set(u => u.Age, 22);
        }

        [BuildersMQL("{ \"Name\" : \"User Name1\" }")]
        [BuildersMQL("{ \"$set\" : { \"Age\" : 22 } }")]
        public void UsingAliasTest()
        {
            _ = builder.Filter.Eq(u => u.Name, "User Name1");
            _ = builder.Update.Set(u => u.Age, 22);
        }
    }
}

#pragma warning disable IDE0001
