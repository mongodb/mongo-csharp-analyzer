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
        [Json("{ \"Address\" : { \"City\" : \"City\", \"Province\" : \"Province\", \"ZipCode\" : \"ZipCode\" }, \"Person\" : { \"Name\" : \"Name\", \"LastName\" : \"LastName\", \"Address\" : { \"City\" : \"City\", \"Province\" : \"Province\", \"ZipCode\" : \"ZipCode\" }, \"Vehicle\" : { \"LicenceNumber\" : \"LicenceNumber\", \"VehicleType\" : { \"VehicleMake\" : null, \"Type\" : 0, \"Category\" : \"Category\", \"MPG\" : 3.0 } }, \"SiblingsCount\" : 3, \"TicksSinceBirth\" : NumberLong(5), \"IsRetired\" : true }, \"Tree\" : { \"Root\" : { \"Data\" : 4, \"Left\" : { \"Data\" : 4, \"Left\" : null, \"Right\" : null, \"Tree\" : null }, \"Right\" : { \"Data\" : 4, \"Left\" : null, \"Right\" : null, \"Tree\" : null }, \"Tree\" : { \"Root\" : null } } }, \"User\" : { \"Name\" : \"Name\", \"LastName\" : \"LastName\", \"Address\" : \"Address\", \"Age\" : 3, \"Height\" : 6, \"Scores\" : [] }, \"Vehicle\" : { \"LicenceNumber\" : \"LicenceNumber\", \"VehicleType\" : { \"VehicleMake\" : { \"Name\" : \"Name\" }, \"Type\" : 0, \"Category\" : \"Category\", \"MPG\" : 3.0 } }, \"EnumInt16\" : 0, \"EnumInt32\" : 0, \"EnumInt64\" : NumberLong(0), \"Pair\" : { \"StringA\" : null, \"StringB\" : null } }")]
        public void NestedTypeHolder()
        {
        }

        [Json("{ \"NestedInt\" : 9, \"NestedDouble\" : 2.0, \"NestedString\" : \"NestedString\" }")]
        public void NestedType()
        {
        }

        [Json("{ \"NestedNestedInt\" : 5, \"NestedNestedDouble\" : 8.0, \"NestedNestedString\" : \"NestedNestedString\" }")]
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

