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

namespace MongoDB.Analyzer.Tests.Common.TestCases.Json
{
    public sealed class JsonNestedTypes : TestCasesBase
    {
        [JsonAttribute("{ \"Address\" : { \"City\" : \"City\", \"Province\" : \"Province\", \"ZipCode\" : \"ZipCode\" }, \"Person\" : { \"Name\" : \"Name\", \"LastName\" : \"LastName\", \"Address\" : { \"City\" : null, \"Province\" : null, \"ZipCode\" : null }, \"Vehicle\" : { \"LicenceNumber\" : \"LicenceNumber\", \"VehicleType\" : { \"VehicleMake\" : { \"Name\" : \"Name\" }, \"Type\" : 0, \"Category\" : \"Category\", \"MPG\" : 0.0 } }, \"SiblingsCount\" : 0, \"TicksSinceBirth\" : NumberLong(0), \"IsRetired\" : false }, \"Tree\" : { \"Root\" : { \"Data\" : 0, \"Left\" : { \"Data\" : 0, \"Left\" : null, \"Right\" : null, \"Tree\" : null }, \"Right\" : { \"Data\" : 0, \"Left\" : null, \"Right\" : null, \"Tree\" : null }, \"Tree\" : { \"Root\" : null } } }, \"User\" : { \"Name\" : \"Name\", \"LastName\" : \"LastName\", \"Address\" : \"Address\", \"Age\" : 0, \"Height\" : 0, \"Scores\" : [0, 0] }, \"Vehicle\" : { \"LicenceNumber\" : null, \"VehicleType\" : null }, \"EnumInt16\" : 0, \"EnumInt32\" : 0, \"EnumInt64\" : NumberLong(0), \"Pair\" : { \"StringA\" : null, \"StringB\" : null } }")]
        public void NestedTypeHolder()
        {
        }

        [JsonAttribute("{ \"NestedInt\" : 0, \"NestedDouble\" : 0.0, \"NestedString\" : \"NestedString\" }")]
        public void NestedType()
        {
        }

        [JsonAttribute("{ \"NestedNestedInt\" : 0, \"NestedNestedDouble\" : 0.0, \"NestedNestedString\" : \"NestedNestedString\" }")]
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

