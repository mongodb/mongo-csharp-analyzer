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
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BasicSample
{
    public class PocoSample
    {
        // This POCO will be analyzed with PocoAnalysisVerbosity set to "All"
        // or if it's used in Builders/LINQ expression
        public class Address
        {
            public string StreetName { get; set; }
            public string City { get; set; }
            public string Province { get; set; }
            public string ZipCode { get; set; }
        }

        // This POCO will be analyzed with PocoAnalysisVerbosity set to "All"
        // or if it's used in Builders/LINQ expression
        public class AirlineTicket
        {
            public string CustomerId { get; set; }

            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            public string Address { get; set; }

            public string DepartureCountry { get; set; }
            public string ArrivalCountry { get; set; }

            public string DepartureAirportName { get; set; }
            public string ArrivalAirportName { get; set; }
        }

        public class House
        {
            [BsonId]
            public string Identifier { get; set; }

            [BsonElement("year_built")]
            public int YearBuilt { get; set; }

            [BsonElement("occupied_on")]
            public DateTime DateOccupiedOn { get; set; }

            public string Style { get; set; }

            [BsonIgnore]
            public double Cost { get; set; }
        }

        // Warning is displayed due to invalid BsonTimeSpanOptions usage for a boolean property
        public class NotSupportedPoco
        {
            [BsonTimeSpanOptions(representation: BsonType.Array, Units = MongoDB.Bson.Serialization.Options.TimeSpanUnits.Hours)]
            public bool InStock { get; set; }

            public int Width { get; set; }
        }
    }
}

