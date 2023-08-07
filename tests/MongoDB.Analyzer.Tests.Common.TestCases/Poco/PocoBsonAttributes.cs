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
using MongoDB.Analyzer.Tests.Common.DataModel;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MongoDB.Analyzer.Tests.Common.TestCases.Poco
{
    public sealed class PocoBsonAttributes : TestCasesBase
    {
        [PocoJson("{ \"Style\" : \"Style_val\", \"year_built\" : 9, \"_id\" : \"Identifier_val\" }")]
        public void House()
        {
        }

        [PocoJson("{ \"ExpiryDate\" : ISODate(\"1000-10-10T15:07:10Z\"), \"Name\" : \"Benjamin\", \"InStock\" : true, \"price\" : \"0\", \"Pair\" : { \"StringA\" : null, \"StringB\" : null }, \"Length\" : 6, \"Width\" : 5, \"SaleTime\" : \"08:08:08\" }")]
        public void Clothing()
        {
        }

        [PocoJson("{ \"VegetableCost\" : 3.0 }")]
        public void Vegetable()
        {
        }

        [PocoJson("{ \"ComputerCost\" : 2.0 }")]
        public void Computer()
        {
        }

        public class TestClasses
        {
            public class House
            {
                [BsonId]
                public string Identifier { get; set; }

                [BsonElement("year_built", Order = 2)]
                public int YearBuilt { get; set; }

                [BsonElement(Order = 1)]
                public string Style { get; set; }

                [BsonIgnore]
                public double Cost { get; set; }
            }

            public class Clothing
            {
                [BsonConstructor("Name", "InStock", "Price")]
                public Clothing(string Name, bool InStock, decimal Price)
                {
                    this.Name = Name;
                    this.InStock = InStock;
                    this.Price = Price;
                }

                [BsonFactoryMethod("Name", "InStock", "Price")]
                public void Factory_Method()
                {
                }

                [BsonIgnoreIfDefault]
                public string Name { get; set; }

                [BsonIgnoreIfNull]
                public bool InStock { get; set; }

                [BsonElement("price")]
                [BsonRepresentation(BsonType.Decimal128)]
                public decimal Price { get; set; }

                public Pair Pair { get; set; }

                [BsonDateTimeOptions(DateOnly = false, Kind = DateTimeKind.Unspecified, Representation = BsonType.DateTime)]
                public DateTime ExpiryDate;

                [BsonDefaultValue(defaultValue: 10)]
                public int Length { get; set; }

                [BsonRequired]
                public int Width { get; set; }

                [BsonTimeSpanOptions(representation: Bson.BsonType.String)]
                public TimeSpan SaleTime { get; set; }
            }

            [BsonIgnoreExtraElements]
            [BsonDiscriminator("Carrot")]
            public class Vegetable
            {
                public double VegetableCost { get; set; }
            }

            [BsonNoId]
            public class Computer
            {
                public double ComputerCost { get; set; }

                [BsonExtraElementsAttribute]
                public BsonDocument CatchAll { get; set; }
            }
        }
    }
}

