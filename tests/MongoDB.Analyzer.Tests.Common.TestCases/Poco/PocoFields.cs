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

namespace MongoDB.Analyzer.Tests.Common.TestCases.Poco
{
    public sealed class PocoFields : TestCasesBase
    {
        [PocoJson("{ \"BooleanValue\" : true, \"ByteValue\" : 9, \"SByteValue\" : 0, \"ShortValue\" : 0, \"UShortValue\" : 1, \"IntValue\" : 8, \"UIntValue\" : 9, \"LongValue\" : NumberLong(9), \"ULongValue\" : NumberLong(0), \"CharValue\" : 9, \"DoubleValue\" : 1.0, \"StringValue\" : \"StringValue_val\", \"FloatValue\" : 0.0 }")]
        public void PrimitiveTypeHolderWithFields()
        {
        }

        [PocoJson("{ \"Address\" : { \"City\" : \"Phoenix\", \"Province\" : \"Jalisco\", \"ZipCode\" : \"92101\" }, \"Person\" : { \"Name\" : \"Sophia\", \"LastName\" : \"Jackson\", \"Address\" : { \"City\" : \"Phoenix\", \"Province\" : \"Jalisco\", \"ZipCode\" : \"92101\" }, \"Vehicle\" : { \"LicenceNumber\" : \"M47-NOP\", \"VehicleType\" : { \"VehicleMake\" : null, \"Type\" : 0, \"Category\" : \"Category_val\", \"MPG\" : 3.0 } }, \"SiblingsCount\" : 3, \"TicksSinceBirth\" : NumberLong(5), \"IsRetired\" : true }, \"Tree\" : { \"Root\" : { \"Data\" : 4, \"Left\" : { \"Data\" : 4, \"Left\" : null, \"Right\" : null, \"Tree\" : null }, \"Right\" : { \"Data\" : 4, \"Left\" : null, \"Right\" : null, \"Tree\" : null }, \"Tree\" : { \"Root\" : null } } }, \"User\" : { \"Name\" : \"Sophia\", \"LastName\" : \"Jackson\", \"Address\" : \"111 Sycamore Court, Woodland, WA 98674\", \"Age\" : 3, \"Height\" : 6, \"Scores\" : [] }, \"Vehicle\" : { \"LicenceNumber\" : \"M47-NOP\", \"VehicleType\" : { \"VehicleMake\" : { \"Name\" : \"Sophia\" }, \"Type\" : 0, \"Category\" : \"Category_val\", \"MPG\" : 3.0 } }, \"EnumInt16\" : 0, \"EnumInt32\" : 0, \"EnumInt64\" : NumberLong(0), \"Pair\" : { \"StringA\" : null, \"StringB\" : null } }")]
        public void NestedTypeHolderWithFields()
        {
        }

        [PocoJson("{ \"NestedInt\" : 9, \"NestedDouble\" : 2.0, \"NestedString\" : \"NestedString_val\" }")]
        public void NestedTypeWithFields()
        {
        }

        [PocoJson("{ \"NestedNestedInt\" : 5, \"NestedNestedDouble\" : 8.0, \"NestedNestedString\" : \"NestedNestedString_val\" }")]
        public void NestedNestedTypeWithFields()
        {
        }

        public class TestClasses
        {
            public class PrimitiveTypeHolderWithFields
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

            public class NestedTypeHolderWithFields
            {
                public Address Address;
                public Person Person;
                public Tree Tree;
                public User User;
                public Vehicle Vehicle;

                public EnumInt16 EnumInt16;
                public EnumInt32 EnumInt32;
                public EnumInt64 EnumInt64;

                public Pair Pair;

                public class NestedTypeWithFields
                {
                    public int NestedInt;
                    public double NestedDouble;
                    public string NestedString;

                    public class NestedNestedTypeWithFields
                    {
                        public int NestedNestedInt;
                        public double NestedNestedDouble;
                        public string NestedNestedString;
                    }
                }
            }
        }
    }
}

