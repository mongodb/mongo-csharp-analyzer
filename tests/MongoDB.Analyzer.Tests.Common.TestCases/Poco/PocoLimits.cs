﻿// Copyright 2021-present MongoDB Inc.
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

using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace MongoDB.Analyzer.Tests.Common.TestCases.Poco
{
    public sealed class PocoLimits : TestCasesBase
    {
        [PocoJson("{ \"AirlineName\" : \"Radiant Skies Aviation\" }", pocoLimit: 10, pocoAnalysisVerbosity: PocoAnalysisVerbosity.All)]
        public void Airline_not_used_in_expression1()
        {
        }

        [PocoJson("{ \"AirlineName\" : \"Radiant Skies Aviation\" }", pocoLimit: 10, pocoAnalysisVerbosity: PocoAnalysisVerbosity.All)]
        public void Airline_not_used_in_expression2()
        {
        }

        [PocoJson("{ \"AirlineName\" : \"Radiant Skies Aviation\" }", pocoLimit: 10, pocoAnalysisVerbosity: PocoAnalysisVerbosity.All)]
        public void Airline_not_used_in_expression3()
        {
        }

        [BuildersMQL("{ \"AirlineName\" : \"Lufthansa\" }", pocoLimit: 10, pocoAnalysisVerbosity: PocoAnalysisVerbosity.All)]
        [PocoJson("{ \"AirlineName\" : \"Radiant Skies Aviation\" }", pocoLimit: 10, pocoAnalysisVerbosity: PocoAnalysisVerbosity.All)]
        public void Airline_used_in_expression1()
        {
            _ = Builders<TestClasses.Airline_used_in_expression1>.Filter.Eq(u => u.AirlineName, "Lufthansa");
        }

        [BuildersMQL("{ \"AirlineName\" : \"Lufthansa\" }", pocoLimit: 10, pocoAnalysisVerbosity: PocoAnalysisVerbosity.All)]
        [PocoJson("{ \"AirlineName\" : \"Radiant Skies Aviation\" }", pocoLimit: 10, pocoAnalysisVerbosity: PocoAnalysisVerbosity.All)]
        public void Airline_used_in_expression2()
        {
            _ = Builders<TestClasses.Airline_used_in_expression2>.Filter.Eq(u => u.AirlineName, "Lufthansa");
        }

        [BuildersMQL("{ \"AirlineName\" : \"Lufthansa\" }", pocoLimit: 10, pocoAnalysisVerbosity: PocoAnalysisVerbosity.All)]
        [PocoJson("{ \"AirlineName\" : \"Radiant Skies Aviation\" }", pocoLimit: 10, pocoAnalysisVerbosity: PocoAnalysisVerbosity.All)]
        public void Airline_used_in_expression3()
        {
            _ = Builders<TestClasses.Airline_used_in_expression3>.Filter.Eq(u => u.AirlineName, "Lufthansa");
        }

        [PocoJson("{ \"StringProperty\" : \"StringProperty_val\" }", pocoLimit: 10, pocoAnalysisVerbosity: PocoAnalysisVerbosity.All)]
        public void ClassWithBsonAttributes()
        {
        }

        [PocoJson("{ \"StringProperty\" : \"StringProperty_val\" }", pocoLimit: 10, pocoAnalysisVerbosity: PocoAnalysisVerbosity.All)]
        public void ClassWithBsonAttributes2()
        {
        }

        [NoDiagnostics(pocoLimit: 10, pocoAnalysisVerbosity: PocoAnalysisVerbosity.All)]
        public void ClassWithFieldBsonAttributes()
        {
        }

        [NoDiagnostics(pocoLimit: 10, pocoAnalysisVerbosity: PocoAnalysisVerbosity.All)]
        public void ClassWithFieldBsonAttributes2()
        {
        }

        [NoDiagnostics(pocoLimit: 10, pocoAnalysisVerbosity: PocoAnalysisVerbosity.All)]
        public void ClassWithPropertyAndFieldAttributes()
        {
        }

        [NoDiagnostics(pocoLimit: 10, pocoAnalysisVerbosity: PocoAnalysisVerbosity.All)]
        public void ClassWithPropertyAndFieldAttributes2()
        {
        }

        [NoDiagnostics(pocoLimit: 10, pocoAnalysisVerbosity: PocoAnalysisVerbosity.All)]
        public void ClassWithPropertyBsonAttributes()
        {
        }

        [NoDiagnostics(pocoLimit: 10, pocoAnalysisVerbosity: PocoAnalysisVerbosity.All)]
        public void ClassWithPropertyBsonAttributes2()
        {
        }

        public class TestClasses
        {
            public class Airline_not_used_in_expression1
            {
                public string AirlineName { get; set; }
            }

            public class Airline_not_used_in_expression2
            {
                public string AirlineName { get; set; }
            }

            public class Airline_not_used_in_expression3
            {
                public string AirlineName { get; set; }
            }

            public class Airline_used_in_expression1
            {
                public string AirlineName { get; set; }
            }

            public class Airline_used_in_expression2
            {
                public string AirlineName { get; set; }
            }

            public class Airline_used_in_expression3
            {
                public string AirlineName { get; set; }
            }

            [BsonIgnoreExtraElements]
            public class ClassWithBsonAttributes
            {
                public string StringProperty { get; set; }
            }

            [BsonIgnoreExtraElements]
            public class ClassWithBsonAttributes2
            {
                public string StringProperty { get; set; }
            }

            public class ClassWithFieldBsonAttributes
            {
                [BsonElement("string_field", Order = 2)]
                public string StringField;
            }

            public class ClassWithFieldBsonAttributes2
            {
                [BsonElement("string_field", Order = 2)]
                public string StringField;
            }

            public class ClassWithPropertyAndFieldAttributes
            {
                [BsonElement("string_property", Order = 2)]
                public string StringProperty { get; set; }

                [BsonElement("string_field", Order = 1)]
                public string StringField;
            }

            public class ClassWithPropertyAndFieldAttributes2
            {
                [BsonElement("string_property", Order = 2)]
                public string StringProperty { get; set; }

                [BsonElement("string_field", Order = 1)]
                public string StringField;
            }

            public class ClassWithPropertyBsonAttributes
            {
                [BsonElement("string_property", Order = 2)]
                public string StringProperty { get; set; }
            }

            public class ClassWithPropertyBsonAttributes2
            {
                [BsonElement("string_property", Order = 2)]
                public string StringProperty { get; set; }
            }
        }
    }
}

