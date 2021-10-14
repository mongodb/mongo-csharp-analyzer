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

using System.Linq;
using MongoDB.Analyzer.Tests.Common.DataModel;
using MongoDB.Driver;

namespace MongoDB.Analyzer.Tests.Common.TestCases.Builders
{
    public sealed class BuildersCollections : TestCasesBase
    {
        [BuildersMQL("{ \"$or\" : [{ \"IntList.2\" : 1 }, { \"StringList.3\" : \"Value\" }, { \"PesonsList.4.Name\" : \"Bob\" }, { \"NestedListsHolderList.5.IntList.1\" : 1 }, { \"IntIList.4\" : 3 }, { \"NestedListsHolderIList.15.IntList.3\" : 3 }] }")]
        public void Lists()
        {
            _ = Builders<ListsHolder>.Filter.Eq(t => t.IntList[2], 1) |
                Builders<ListsHolder>.Filter.Eq(t => t.StringList[3], "Value") |
                Builders<ListsHolder>.Filter.Eq(t => t.PesonsList[4].Name, "Bob") |
                Builders<ListsHolder>.Filter.Eq(t => t.NestedListsHolderList[5].IntList[1], 1) |
                Builders<ListsHolder>.Filter.Eq(t => t.IntIList[4], 3) |
                Builders<ListsHolder>.Filter.Eq(t => t.NestedListsHolderIList[15].IntList[3], 3);
        }

        [BuildersMQL("{ \"$or\" : [{ \"Enumerable1.0\" : 2 }, { \"Enumerable2.1.Enumerable1.0\" : 2 }] }")]
        public void Enumerables()
        {
            _ = Builders<EnumerableHolder>.Filter.Eq(t => t.Enumerable1.ElementAt(0), 2) |
                Builders<EnumerableHolder>.Filter.Eq(t => t.Enumerable2.ElementAt(1).Enumerable1.ElementAt(0), 2);
        }
    }
}
