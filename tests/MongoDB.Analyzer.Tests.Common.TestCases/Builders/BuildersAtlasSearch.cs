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
using System.Collections.Generic;
using MongoDB.Analyzer.Tests.Common.DataModel;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GeoJsonObjectModel;
using MongoDB.Driver.Search;

namespace MongoDB.Analyzer.Tests.Common.TestCases.Builders
{
    public sealed class BuildersAtlasSearch : TestCasesBase
    {
        private static readonly GeoJsonPolygon<GeoJson2DGeographicCoordinates> s_testPolygon =
            new GeoJsonPolygon<GeoJson2DGeographicCoordinates>(
                new GeoJsonPolygonCoordinates<GeoJson2DGeographicCoordinates>(
                    new GeoJsonLinearRingCoordinates<GeoJson2DGeographicCoordinates>(
                        new List<GeoJson2DGeographicCoordinates>()
                        {
                                new GeoJson2DGeographicCoordinates(-161.323242, 22.512557),
                                new GeoJson2DGeographicCoordinates(-152.446289, 22.065278),
                                new GeoJson2DGeographicCoordinates(-156.09375, 17.811456),
                                new GeoJson2DGeographicCoordinates(-161.323242, 22.512557)
                        })));

        [BuildersMQL("{ \"autocomplete\" : { \"query\" : \"My address\", \"path\" : \"Address\" } }", DriverVersions.V2_19_AndHigher, 1, 2, 3)]
        public void Autocomplete()
        {
            _ = Builders<User>.Search.Autocomplete(m => m.Address, "My address");
            _ = Builders<User>.Search.Autocomplete("Address", "My address");
            _ = Builders<BsonDocument>.Search.Autocomplete("Address", "My address");
        }

        [BuildersMQL("{ \"equals\" : { \"value\" : true, \"path\" : \"IsRetired\" } }", DriverVersions.V2_19_AndHigher, 1, 2, 3)]
        public void Equals()
        {
            _ = Builders<Person>.Search.Equals(m => m.IsRetired, true);
            _ = Builders<Person>.Search.Equals("IsRetired", true);
            _ = Builders<BsonDocument>.Search.Equals("IsRetired", true);
        }

        [BuildersMQL("{ \"near\" : { \"origin\" : 1, \"pivot\" : 2, \"path\" : \"SiblingsCount\" } }", DriverVersions.V2_19_AndHigher)]
        [BuildersMQL("{ \"near\" : { \"origin\" : NumberLong(5), \"pivot\" : NumberLong(2), \"path\" : \"SiblingsCount\" } }", DriverVersions.V2_19_AndHigher, 2, 3)]
        public void Near()
        {
            Builders<Person>.Search.Near(p => p.SiblingsCount, 1, 2);
            Builders<Person>.Search.Near("SiblingsCount", 5L, 2L);
            Builders<BsonDocument>.Search.Near("SiblingsCount", 5L, 2L);
        }

        [BuildersMQL("{ \"phrase\" : { \"query\" : \"Columbia\", \"path\" : \"Address.Province\" } }", DriverVersions.V2_19_AndHigher, 1, 2, 3)]
        public void Phrase()
        {
            _ = Builders<Person>.Search.Phrase(m => m.Address.Province, "Columbia");
            _ = Builders<Person>.Search.Phrase("Address.Province", "Columbia");
            _ = Builders<BsonDocument>.Search.Phrase("Address.Province", "Columbia");
        }

        [BuildersMQL("{ \"queryString\" : { \"defaultPath\" : \"Name\", \"query\" : \"constant string\" } }", DriverVersions.V2_19_AndHigher, 1, 2, 3)]
        public void QueryString()
        {
            _ = Builders<Person>.Search.QueryString(m => m.Name, "constant string");
            _ = Builders<Person>.Search.QueryString("Name", "constant string");
            _ = Builders<BsonDocument>.Search.QueryString("Name", "constant string");
        }

        [BuildersMQL("{ \"wildcard\" : { \"query\" : [\"foo\", \"bar\"], \"path\" : \"Name\" } }", DriverVersions.V2_19_AndHigher)]
        [BuildersMQL("{ \"wildcard\" : { \"query\" : \"A\", \"path\" : \"Name\" } }", DriverVersions.V2_19_AndHigher, 2, 3)]
        public void Wildcard()
        {
            _ = Builders<Person>.Search.Wildcard(m => m.Name, new[] { "foo", "bar" });
            _ = Builders<Person>.Search.Wildcard("Name", "A");
            _ = Builders<BsonDocument>.Search.Wildcard("Name", "A");
        }

        [BuildersMQL("{ \"regex\" : { \"query\" : [\"Donald\", \"Mike\"], \"path\" : \"Name\" } }", DriverVersions.V2_19_AndHigher)]
        [BuildersMQL("{ \"regex\" : { \"query\" : \"Alice\", \"path\" : \"Name\" } }", DriverVersions.V2_19_AndHigher, 2, 3)]
        public void Regex()
        {
            _ = Builders<Person>.Search.Regex(m => m.Name, new[] { "Donald", "Mike" });
            _ = Builders<Person>.Search.Regex("Name", "Alice");
            _ = Builders<BsonDocument>.Search.Regex("Name", "Alice");
        }

        [BuildersMQL("{ \"span\" : { \"first\" : { \"operator\" : { \"term\" : { \"query\" : \"foo\", \"path\" : \"Name\" } }, \"endPositionLte\" : 5 } } }", DriverVersions.V2_19_AndHigher)]
        [BuildersMQL("{ \"span\" : { \"or\" : { \"clauses\" : [{ \"term\" : { \"query\" : \"a\", \"path\" : \"Name\" } }, { \"term\" : { \"query\" : \"b\", \"path\" : \"Name\" } }, { \"term\" : { \"query\" : \"c\", \"path\" : \"Name\" } }] } } }", DriverVersions.V2_19_AndHigher)]
        public void Span()
        {
            _ = Builders<Person>.Search.Span(Builders<Person>.SearchSpan
                .First(Builders<Person>.SearchSpan.Term(p => p.Name, "foo"), 5));

            _ = Builders<Person>.Search.Span(Builders<Person>.SearchSpan.Or(
                    Builders<Person>.SearchSpan.Term(p => p.Name, "a"),
                    Builders<Person>.SearchSpan.Term(p => p.Name, "b"),
                    Builders<Person>.SearchSpan.Term(p => p.Name, "c")));
        }

        [BuildersMQL("{ \"text\" : { \"query\" : \"My address\", \"path\" : \"Address\" } }", DriverVersions.V2_19_AndHigher, 1, 2, 3)]
        public void Text()
        {
            _ = Builders<User>.Search.Text(m => m.Address, "My address");
            _ = Builders<User>.Search.Text("Address", "My address");
            _ = Builders<BsonDocument>.Search.Text("Address", "My address");
        }

        [NoDiagnostics(DriverVersions.V2_19_AndHigher)]
        public void Valid_but_ignored()
        {
            _ = Builders<Person>.Search.GeoShape(
                "location",
                GeoShapeRelation.Disjoint,
                s_testPolygon);

            _ = Builders<Person>.Search.GeoWithin(
                "location",
                s_testPolygon);

            _ = Builders<Person>.Search.Range(m => m.SiblingsCount, SearchRangeBuilder.Gt(1).Lt(10));
            _ = Builders<Person>.Search.Near("Date", DateTime.Now, 1);

            _ = Builders<Person>.Search.Wildcard(new FieldDefinition<Person>[]
                    {
                        new ExpressionFieldDefinition<Person, string>(x => x.Name),
                        new ExpressionFieldDefinition<Person, string>(x => x.LastName)
                    }, "A");
        }

        [NoDiagnostics(DriverVersions.V2_18_AndLower)]
        public void Search_should_be_ignored_in_older_drivers()
        {
            // Technically Atlas Search code can't appear with older drivers, via normal C# usage,
            // so this test covers isoteric edge cases.
            _ = Builders<User>.Search.Text(m => m.Address, "My address");
            _ = Builders<Person>.Search.Phrase(m => m.Address.Province, "Columbia");
        }
    }
}
