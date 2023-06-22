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
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MongoDB.Analyzer.Tests.Common.DataModel
{
    [BsonDiscriminatorAttribute("Apple")]
    public class Fruit
    {
        public Fruit() { }

        [BsonConstructorAttribute("Name", "Weight", "Color")]
        public Fruit(string Name, double Weight, string Color, int Quantity, double TotalCost)
        {
            this.Name = Name;
            this.Weight = Weight;
            this.Color = Color;
            this.Quantity = Quantity;
            this.TotalCost = TotalCost;
        }

        [BsonFactoryMethodAttribute("Name", "Weight", "Color")]
        public void Factory_Method()
        {
        }

        [BsonIdAttribute(Order = int.MaxValue)]
        public string Name;

        [BsonDefaultValueAttribute(5.5)]
        public double Weight { get; set; }

        [BsonRequiredAttribute]
        public string Color { get; set; }

        [BsonIgnoreAttribute]
        public int Quantity { get; set; }

        [BsonElementAttribute(elementName: "Cost")]
        public double TotalCost;

        [BsonIgnoreIfDefaultAttribute(true)]
        public double Volume { get; set; }

        [BsonIgnoreIfNullAttribute(true)]
        public object Width { get; set; }

        [BsonDateTimeOptionsAttribute(DateOnly = false, Kind = DateTimeKind.Unspecified, Representation = BsonType.DateTime)]
        public DateTime ExpiryDate;

        [BsonDictionaryOptionsAttribute(Bson.Serialization.Options.DictionaryRepresentation.Document)]
        public Dictionary<string, string> DictionaryField;

        [BsonTimeSpanOptionsAttribute(representation : Bson.BsonType.String)]
        public TimeSpan TimeSpanField { get; set; }

        [BsonExtraElementsAttribute]
        public BsonDocument CatchAll { get; set; }
    }

    [BsonKnownTypesAttribute(new Type[] {typeof(GreenApple) })]
    public class Apple : Fruit
    {
        public string AppleType { get; set; }

        [BsonIgnoreAttribute]
        public string _AppleID;
    }

    [BsonDiscriminatorAttribute("GreenAppleDiscriminator")]
    public class GreenApple : Apple
    {
        public double GreenAppleCost { get; set; }
    }

    [BsonSerializerAttribute(typeof(CustomBsonSerializer))]
    public class RedApple : Apple
    {
        [BsonRepresentationAttribute(BsonType.Double)]
        public double RedAppleCost { get; set; }

        public string RedAppleType { get; set; }
    }

    [BsonSerializerAttribute(typeof(Person))]
    public class GrannyApple : Apple
    {
        public double GrannyAppleCost { get; set; }
    }

    [BsonKnownTypes(typeof(int))]
    public class GoldenApple : Apple
    {
        public double GoldenAppleCost { get; set; }
    }

    [BsonKnownTypes(typeof(System.Type))]
    public class FujiApple : Apple
    {
        public double FujiAppleCost { get; set; }
    }

    [BsonKnownTypes(typeof(System.TimeSpan))]
    public class YellowApple : Apple
    {
        public double YellowAppleCost { get; set; }
    }

    public class Banana
    {
        [BsonIdAttribute(IdGenerator = typeof(System.Type))]
        public double BananaCost { get; set; }

        [BsonElementAttribute(elementName: "Weight")]
        public double BananaWeight;
    }

    public class Pear
    {
        [BsonTimeSpanOptionsAttribute(units: Bson.Serialization.Options.TimeSpanUnits.Days, representation: Bson.BsonType.String)]
        public TimeSpan TimeSpanField { get; set; }

        public DateTimeOffset DateTimeOffset { get; set; }
        public int Quantity { get; set; }
    }
}

