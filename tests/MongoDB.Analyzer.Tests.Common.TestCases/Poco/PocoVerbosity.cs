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

using MongoDB.Bson.Serialization.Attributes;

namespace MongoDB.Analyzer.Tests.Common.TestCases.Poco
{
    public sealed class PocoVerbosity : TestCasesBase
    {
        [NoDiagnosticsJson(jsonAnalyzerVerbosity: JsonAnalyzerVerbosity.Medium)]
        public void Airline()
        {
        }

        [Json("{ \"StringProperty\" : \"StringProperty_val\" }", jsonAnalyzerVerbosity: JsonAnalyzerVerbosity.Medium)]
        public void ClassWithBsonAttributes()
        {
        }

        [Json("{ \"string_property\" : \"StringProperty_val\" }", jsonAnalyzerVerbosity: JsonAnalyzerVerbosity.Medium)]
        public void ClassWithPropertyBsonAttributes()
        {
        }

        [Json("{ \"string_field\" : \"StringField_val\" }", jsonAnalyzerVerbosity: JsonAnalyzerVerbosity.Medium)]
        public void ClassWithFieldBsonAttributes()
        {
        }

        [Json("{ \"string_field\" : \"StringField_val\", \"string_property\" : \"StringProperty_val\" }", jsonAnalyzerVerbosity: JsonAnalyzerVerbosity.Medium)]
        public void ClassWithPropertyAndFieldAttributes()
        {
        }

        [NoDiagnosticsJson(jsonAnalyzerVerbosity: JsonAnalyzerVerbosity.None)]
        public void ClassWithBsonAttributes2()
        {
        }

        [NoDiagnosticsJson(jsonAnalyzerVerbosity: JsonAnalyzerVerbosity.None)]
        public void ClassWithPropertyBsonAttributes2()
        {
        }

        [NoDiagnosticsJson(jsonAnalyzerVerbosity: JsonAnalyzerVerbosity.None)]
        public void ClassWithFieldBsonAttributes2()
        {
        }

        [NoDiagnosticsJson(jsonAnalyzerVerbosity: JsonAnalyzerVerbosity.None)]
        public void ClassWithPropertyAndFieldAttributes2()
        {
        }

        public class TestClasses
        {
            public class Airline
            {
                public string AirlineName { get; set; }
            }

            [BsonIgnoreExtraElements]
            public class ClassWithBsonAttributes
            {
                public string StringProperty { get; set; }
            }

            public class ClassWithPropertyBsonAttributes
            {
                [BsonElement("string_property", Order = 2)]
                public string StringProperty { get; set; }
            }

            public class ClassWithFieldBsonAttributes
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

            [BsonIgnoreExtraElements]
            public class ClassWithBsonAttributes2
            {
                public string StringProperty { get; set; }
            }

            public class ClassWithPropertyBsonAttributes2
            {
                [BsonElement("string_property", Order = 2)]
                public string StringProperty { get; set; }
            }

            public class ClassWithFieldBsonAttributes2
            {
                [BsonElement("string_field", Order = 2)]
                public string StringField;
            }

            public class ClassWithPropertyAndFieldAttributes2
            {
                [BsonElement("string_property", Order = 2)]
                public string StringProperty { get; set; }

                [BsonElement("string_field", Order = 1)]
                public string StringField;
            }
        }
    }
}

