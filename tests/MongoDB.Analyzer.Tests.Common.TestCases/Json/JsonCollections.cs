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

using System.Collections.Generic;
using MongoDB.Analyzer.Tests.Common.DataModel;

namespace MongoDB.Analyzer.Tests.Common.TestCases.Json
{
    public sealed class JsonCollections : TestCasesBase
    {
        [Json("{ \"IntList\" : [0], \"PesonsList\" : [{ \"Name\" : \"Name\", \"LastName\" : \"LastName\", \"Address\" : { \"City\" : \"City\", \"Province\" : \"Province\", \"ZipCode\" : \"ZipCode\" }, \"Vehicle\" : { \"LicenceNumber\" : \"LicenceNumber\", \"VehicleType\" : { \"VehicleMake\" : { \"Name\" : \"Name\" }, \"Type\" : 0, \"Category\" : \"Category\", \"MPG\" : 0.0 } }, \"SiblingsCount\" : 0, \"TicksSinceBirth\" : NumberLong(0), \"IsRetired\" : false }], \"StringList\" : [\"StringList\"], \"NestedListsHolderList\" : [{ \"IntList\" : null, \"PesonsList\" : null, \"StringList\" : null, \"NestedListsHolderList\" : null, \"IntIList\" : null, \"NestedListsHolderIList\" : null }], \"IntIList\" : [0], \"NestedListsHolderIList\" : [{ \"IntList\" : null, \"PesonsList\" : null, \"StringList\" : null, \"NestedListsHolderList\" : null, \"IntIList\" : null, \"NestedListsHolderIList\" : null }] }")]
        public void ListsHolder()
        {
        }

        [Json("{ \"Enumerable1\" : [0], \"Enumerable2\" : [{ \"Enumerable1\" : null, \"Enumerable2\" : null }] }")]
        public void EnumerableHolder()
        {
        }

        [Json("{ \"NestedIntList\" : [[0]], \"NestedStringList\" : [[\"NestedStringList\"]], \"ListOfIntArray\" : [[0, 0]], \"ListOfStringArray\" : [[\"ListOfStringArray\", \"ListOfStringArray\"]], \"NestedNestedIntList\" : [[[0]]], \"NestedIntIList\" : [[0]], \"NestedStringIList\" : [[\"NestedStringIList\"]], \"NestedIListIntArray\" : [[[0, 0]]], \"NestedIntIEnumerable\" : [[0]], \"NestedStringIEnumerable\" : [[\"NestedStringIEnumerable\"]], \"NestedIntArrayIEnumerable\" : [[[0, 0]]] }")]
        public void NestedCollectionHolder()
        {
        }

        public class TestClasses
        {
            public class ListsHolder
            {
                public List<int> IntList { get; set; }
                public List<Person> PesonsList { get; set; }
                public System.Collections.Generic.List<string> StringList { get; set; }
                public System.Collections.Generic.List<ListsHolder> NestedListsHolderList { get; set; }

                public IList<int> IntIList { get; set; }
                public System.Collections.Generic.IList<ListsHolder> NestedListsHolderIList { get; set; }
            }

            public class EnumerableHolder
            {
                public IEnumerable<int> Enumerable1 { get; set; }
                public System.Collections.Generic.IEnumerable<EnumerableHolder> Enumerable2 { get; set; }
            }

            public class NestedCollectionHolder
            {
                public List<List<int>> NestedIntList { get; set; }
                public List<List<string>> NestedStringList { get; set; }
                public List<int[]> ListOfIntArray { get; set; }
                public List<string[]> ListOfStringArray { get; set; }
                public List<List<List<int>>> NestedNestedIntList { get; set; }

                public IList<IList<int>> NestedIntIList { get; set; }
                public IList<IList<string>> NestedStringIList { get; set; }
                public IList<IList<int[]>> NestedIListIntArray { get; set; }

                public IEnumerable<IEnumerable<int>> NestedIntIEnumerable { get; set; }
                public IEnumerable<IEnumerable<string>> NestedStringIEnumerable { get; set; }
                public IEnumerable<IEnumerable<int[]>> NestedIntArrayIEnumerable { get; set; }
            }
        }
    }
}

