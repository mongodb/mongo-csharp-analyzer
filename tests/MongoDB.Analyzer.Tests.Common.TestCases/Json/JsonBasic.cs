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

namespace MongoDB.Analyzer.Tests.Common.TestCases.Json
{
    public sealed class JsonBasic : TestCasesBase
    {
        [Json("{ \"City\" : \"City\", \"Province\" : \"Province\", \"ZipCode\" : \"ZipCode\" }")]
        public void Address()
        {
        }

        [Json("{ \"AirportName\" : \"AirportName\", \"AirportCode\" : \"AirportCode\" }")]
        public void Airport()
        {
        }

        [Json("{ \"Make\" : \"Make\", \"Model\" : \"Model\", \"Year\" : 4 }")]
        public void Car()
        {
        }

        [Json("{ \"CustomerID\" : 0, \"Name\" : \"Name\", \"Address\" : \"Address\" }")]
        public void Customer()
        {
        }

        [Json("{ \"BooleanValue\" : true, \"ByteValue\" : 9, \"SByteValue\" : 0, \"ShortValue\" : 0, \"UShortValue\" : 1, \"IntValue\" : 8, \"UIntValue\" : 9, \"LongValue\" : NumberLong(9), \"ULongValue\" : NumberLong(0), \"CharValue\" : 9, \"DoubleValue\" : 1.0, \"StringValue\" : \"StringValue\", \"FloatValue\" : 0.0 }")]
        public void PrimitiveTypeHolder()
        {
        }

        public class TestClasses
        {
            public class Address
            {
                public string City { get; set; }
                public string Province { get; set; }
                public string ZipCode { get; set; }
            }

            public class Airport
            {
                public string AirportName { get; set; }
                public string AirportCode { get; set; }
            }

            public class Car
            {
                public string Make { get; set; }
                public string Model { get; set; }
                public int Year { get; set; }
            }

            public class Customer
            {
                public int CustomerID { get; set; }
                public string Name { get; set; }
                public string Address { get; set; }
            }

            public class PrimitiveTypeHolder
            {
                public bool BooleanValue { get; set; }
                public byte ByteValue { get; set; }
                public sbyte SByteValue { get; set; }

                public short ShortValue { get; set; }
                public ushort UShortValue { get; set; }

                public int IntValue { get; set; }
                public uint UIntValue { get; set; }

                public long LongValue { get; set; }
                public ulong ULongValue { get; set; }

                public char CharValue { get; set; }
                public double DoubleValue { get; set; }

                public string StringValue { get; set; }
                public float FloatValue { get; set; }
            }
        }
    }
}

