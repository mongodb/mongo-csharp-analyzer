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

using MongoDB.Analyzer.Tests.Common.DataModel;

namespace MongoDB.Analyzer.Tests.Common.TestCases.Poco
{
    public sealed class PocoNestedTypes : TestCasesBase
    {
        [PocoJson("{ \"Address\" : { \"City\" : \"City_val\", \"Province\" : \"Province_val\", \"ZipCode\" : \"ZipCode_val\" }, \"Person\" : { \"Name\" : \"Name_val\", \"LastName\" : \"LastName_val\", \"Address\" : { \"City\" : \"City_val\", \"Province\" : \"Province_val\", \"ZipCode\" : \"ZipCode_val\" }, \"Vehicle\" : { \"LicenceNumber\" : \"LicenceNumber_val\", \"VehicleType\" : { \"VehicleMake\" : null, \"Type\" : 0, \"Category\" : \"Category_val\", \"MPG\" : 3.0 } }, \"SiblingsCount\" : 3, \"TicksSinceBirth\" : NumberLong(5), \"IsRetired\" : true }, \"Tree\" : { \"Root\" : { \"Data\" : 4, \"Left\" : { \"Data\" : 4, \"Left\" : null, \"Right\" : null, \"Tree\" : null }, \"Right\" : { \"Data\" : 4, \"Left\" : null, \"Right\" : null, \"Tree\" : null }, \"Tree\" : { \"Root\" : null } } }, \"User\" : { \"Name\" : \"Name_val\", \"LastName\" : \"LastName_val\", \"Address\" : \"Address_val\", \"Age\" : 3, \"Height\" : 6, \"Scores\" : [] }, \"Vehicle\" : { \"LicenceNumber\" : \"LicenceNumber_val\", \"VehicleType\" : { \"VehicleMake\" : { \"Name\" : \"Name_val\" }, \"Type\" : 0, \"Category\" : \"Category_val\", \"MPG\" : 3.0 } }, \"EnumInt16\" : 0, \"EnumInt32\" : 0, \"EnumInt64\" : NumberLong(0), \"Pair\" : { \"StringA\" : null, \"StringB\" : null } }")]
        public void NestedTypeHolder()
        {
        }

        [PocoJson("{ \"NestedInt\" : 9, \"NestedDouble\" : 2.0, \"NestedString\" : \"NestedString_val\" }")]
        public void NestedType()
        {
        }

        [PocoJson("{ \"NestedNestedInt\" : 5, \"NestedNestedDouble\" : 8.0, \"NestedNestedString\" : \"NestedNestedString_val\" }")]
        public void NestedNestedType()
        {
        }

        public class TestClasses
        {
            public class NestedTypeHolder
            {
                public Address Address { get; set; }
                public Person Person { get; set; }
                public Tree Tree { get; set; }
                public User User { get; set; }
                public Vehicle Vehicle { get; set; }

                public EnumInt16 EnumInt16 { get; set; }
                public EnumInt32 EnumInt32 { get; set; }
                public EnumInt64 EnumInt64 { get; set; }

                public Pair Pair { get; set; }

                public class NestedType
                {
                    public int NestedInt { get; set; }
                    public double NestedDouble { get; set; }
                    public string NestedString { get; set; }

                    public class NestedNestedType
                    {
                        public int NestedNestedInt { get; set; }
                        public double NestedNestedDouble { get; set; }
                        public string NestedNestedString { get; set; }
                    }
                }
            }
        }
    }
}

