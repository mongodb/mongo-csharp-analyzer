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

namespace MongoDB.Analyzer.Tests.Common.TestCases.Jira
{
    internal sealed class VS140 : TestCasesBase
    {
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
                (((Builders<User>.Filter.Lt(u => u.Age, 10)) |
                 Builders<User>.Filter.Gt(u => u.Age, 20)) |
                (Builders<User>.Filter.Ne(u => u.Name, "Bob") &
                 Builders<User>.Filter.Exists(u => u.LastName)));
        }

        [BuildersMQL("{ \"Address\" : \"text\", \"Vehicle\" : \"text\" }")]
        [BuildersMQL("{ \"Name\" : \"2d\", \"LastName\" : \"2d\" }")]
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
        }

        [BuildersMQL("{ \"Age\" : 1 }")]
        [BuildersMQL("{ \"Name\" : 1 }")]
        public void ProjectionDefinition()
        {
            var projectionDefinition = Builders<User>.Projection;

            _ = projectionDefinition.Include(u => u.Age);

            _ = projectionDefinition.Combine(
                    projectionDefinition.Include(new { Field = "Age" }.Field),
                    projectionDefinition.Include(new { Field = "LastName" }.Field));

            _ = projectionDefinition.Combine(Builders<User>.Projection.Include(u => u.Age), Builders<User>.Projection.Include(u => u.Name));
        }

        [NoDiagnostics]
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
        }

        [BuildersMQL("{ \"Age\" : -1 }")]
        [BuildersMQL("{ \"Name\" : 1 }")]
        [BuildersMQL("{ \"Address\" : 1 }")]
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
        }

        [NoDiagnostics]
        public void UpdateDefinition()
        {
            var updateDefinition = Builders<User>.Update;
            _ = updateDefinition.Set(u => u.Age, 22);
        }
    }
}

