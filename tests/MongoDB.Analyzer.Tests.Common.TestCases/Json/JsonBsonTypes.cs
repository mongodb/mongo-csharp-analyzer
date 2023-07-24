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

using MongoDB.Bson;

namespace MongoDB.Analyzer.Tests.Common.TestCases.Json
{
    public class JsonBsonTypes : TestCasesBase
    {
        [Json("{ \"BsonDocumentField\" : { }, \"BsonObjectIdField\" : { \"_csharpnull\" : true }, \"BsonTypeField\" : 0, \"BsonValueField\" : { \"_csharpnull\" : true }, \"TimeSpanUnitsField\" : 0 }")]
        public void BsonTypeContainer()
        {
        }

        public class TestClasses
        {
            public class BsonTypeContainer
            {
                public BsonDocument BsonDocumentField { get; set; }
                public BsonObjectId BsonObjectIdField { get; set; }
                public BsonType BsonTypeField { get; set; }
                public BsonValue BsonValueField { get; set; }
                public Bson.Serialization.Options.TimeSpanUnits TimeSpanUnitsField { get; set; }
            }
        }
    }
}

