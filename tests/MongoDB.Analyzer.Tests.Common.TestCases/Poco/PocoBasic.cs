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

namespace MongoDB.Analyzer.Tests.Common.TestCases.Poco
{
    public sealed class PocoBasic : TestCasesBase
    {
        [PocoJson("{ \"City\" : \"Phoenix\", \"Province\" : \"Jalisco\", \"ZipCode\" : \"92101\" }")]
        public void Address()
        {
        }

        [PocoJson("{ \"AirportName\" : \"Jomo Kenyatta International Airport\", \"AirportCode\" : \"Jomo Kenyatta International Airport\" }")]
        public void Airport()
        {
        }

        [PocoJson("{ \"Make\" : \"Make_val\", \"Model\" : \"Model_val\", \"Year\" : 4 }")]
        public void Car()
        {
        }

        [PocoJson("{ \"CustomerID\" : 0, \"Name\" : \"Sophia\", \"Address\" : \"111 Sycamore Court, Woodland, WA 98674\" }")]
        public void Customer()
        {
        }

        [PocoJson("{ \"BooleanValue\" : true, \"ByteValue\" : 9, \"SByteValue\" : 0, \"ShortValue\" : 0, \"UShortValue\" : 1, \"IntValue\" : 8, \"UIntValue\" : 9, \"LongValue\" : NumberLong(9), \"ULongValue\" : NumberLong(0), \"CharValue\" : 9, \"DoubleValue\" : 1.0, \"StringValue\" : \"StringValue_val\", \"FloatValue\" : 0.0 }")]
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

