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
using MongoDB.Driver.Linq;

namespace MongoDB.Analyzer.Tests.Common.TestCases.Linq
{
    public sealed class LinqCollections : TestCasesBase
    {
        [MQL("aggregate([{ \"$match\" : { \"Enumerable1\" : { \"$size\" : 121 }, \"Enumerable1.12\" : 1, \"Enumerable2\" : { \"$size\" : 22 }, \"Enumerable2.12.Enumerable2.21.Enumerable1.1\" : 2 } }])")]
        public void CustomEnumerables()
        {
            _ = GetMongoQueryable<CustomEnumerableHolder>().Where(t =>
                    t.Enumerable1.Count() == 121 &&
                    t.Enumerable1.ElementAt(12) == 1 &&
                    t.Enumerable2.Count() == 22 &&
                    t.Enumerable2.ElementAt(12).Enumerable2.ElementAt(21).Enumerable1.ElementAt(1) == 2);
        }

        [MQL("aggregate([{ \"$match\" : { \"IntList.0\" : 2 } }, { \"$match\" : { \"StringList\" : { \"$size\" : 12 } } }, { \"$match\" : { \"PesonsList.2.Address.City\" : \"Hamburg\" } }, { \"$match\" : { \"NestedListsHolderList.2.StringList.4\" : \"Nested\" } }, { \"$match\" : { \"IntIList.1\" : 12 } }, { \"$match\" : { \"NestedListsHolderIList.12.IntIList.12\" : 2 } }])")]
        public void CustomLists()
        {
            _ = GetMongoQueryable<CustomListsHolder>()
                .Where(t => t.IntList[0] == 2)
                .Where(t => t.StringList.Count == 12)
                .Where(t => t.PesonsList[2].Address.City == "Hamburg")
                .Where(t => t.NestedListsHolderList[2].StringList[4] == "Nested")
                .Where(t => t.IntIList[1] == 12)
                .Where(t => t.NestedListsHolderIList[12].IntIList[12] == 2);
        }

        [MQL("aggregate([{ \"$match\" : { \"Enumerable1\" : { \"$size\" : 121 }, \"Enumerable1.12\" : 1, \"Enumerable2\" : { \"$size\" : 22 }, \"Enumerable2.12.Enumerable2.21.Enumerable1.1\" : 2 } }])")]
        public void Enumerables()
        {
            _ = GetMongoQueryable<EnumerableHolder>().Where(t =>
                    t.Enumerable1.Count() == 121 &&
                    t.Enumerable1.ElementAt(12) == 1 &&
                    t.Enumerable2.Count() == 22 &&
                    t.Enumerable2.ElementAt(12).Enumerable2.ElementAt(21).Enumerable1.ElementAt(1) == 2);
        }

        [MQL("aggregate([{ \"$match\" : { \"IntList.0\" : 2 } }, { \"$match\" : { \"StringList\" : { \"$size\" : 12 } } }, { \"$match\" : { \"PesonsList.2.Address.City\" : \"Hamburg\" } }, { \"$match\" : { \"NestedListsHolderList.2.StringList.4\" : \"Nested\" } }, { \"$match\" : { \"IntIList.1\" : 12 } }, { \"$match\" : { \"NestedListsHolderIList.12.IntIList.12\" : 2 } }])")]
        public void Lists()
        {
            _ = GetMongoQueryable<ListsHolder>()
                .Where(t => t.IntList[0] == 2)
                .Where(t => t.StringList.Count == 12)
                .Where(t => t.PesonsList[2].Address.City == "Hamburg")
                .Where(t => t.NestedListsHolderList[2].StringList[4] == "Nested")
                .Where(t => t.IntIList[1] == 12)
                .Where(t => t.NestedListsHolderIList[12].IntIList[12] == 2);
        }


        [MQL("aggregate([{ \"$match\" : { \"IntList.0\" : 2 } }, { \"$match\" : { \"StringList\" : { \"$size\" : 12 } } }, { \"$match\" : { \"PesonsList.2.Address.City\" : \"Hamburg\" } }, { \"$match\" : { \"NestedListsHolderList.2.StringList.4\" : \"Nested\" } }, { \"$match\" : { \"IntIList.1\" : 12 } }, { \"$match\" : { \"NestedListsHolderIList.12.IntIList.12\" : 2 } }])")]
        [MQL("aggregate([{ \"$match\" : { \"Enumerable1\" : { \"$size\" : 121 }, \"Enumerable1.12\" : 1, \"Enumerable2\" : { \"$size\" : 22 }, \"Enumerable2.12.Enumerable2.21.Enumerable1.1\" : 2 } }])")]
        [MQL("aggregate([{ \"$match\" : { \"IntList.0\" : 2 } }, { \"$match\" : { \"StringList\" : { \"$size\" : 12 } } }, { \"$match\" : { \"PesonsList.2.Address.City\" : \"Hamburg\" } }, { \"$match\" : { \"NestedListsHolderList.2.StringList.4\" : \"Nested\" } }, { \"$match\" : { \"IntIList.1\" : 12 } }, { \"$match\" : { \"NestedListsHolderIList.12.IntIList.12\" : 2 } }])")]
        [MQL("aggregate([{ \"$match\" : { \"Enumerable1\" : { \"$size\" : 121 }, \"Enumerable1.12\" : 1, \"Enumerable2\" : { \"$size\" : 22 }, \"Enumerable2.12.Enumerable2.21.Enumerable1.1\" : 2 } }])")]
        public void Query_syntax()
        {
            _ = from listsHolder in GetMongoQueryable<ListsHolder>()
                where listsHolder.IntList[0] == 2
                where listsHolder.StringList.Count == 12
                where listsHolder.PesonsList[2].Address.City == "Hamburg"
                where listsHolder.NestedListsHolderList[2].StringList[4] == "Nested"
                where listsHolder.IntIList[1] == 12
                where listsHolder.NestedListsHolderIList[12].IntIList[12] == 2
                select listsHolder;

            _ = from enumerableHolder in GetMongoQueryable<EnumerableHolder>()
                where enumerableHolder.Enumerable1.Count() == 121 &&
                      enumerableHolder.Enumerable1.ElementAt(12) == 1 &&
                      enumerableHolder.Enumerable2.Count() == 22 &&
                      enumerableHolder.Enumerable2.ElementAt(12).Enumerable2.ElementAt(21).Enumerable1.ElementAt(1) == 2
                select enumerableHolder;

            _ = from customListsHolder in GetMongoQueryable<CustomListsHolder>()
                where customListsHolder.IntList[0] == 2
                where customListsHolder.StringList.Count == 12
                where customListsHolder.PesonsList[2].Address.City == "Hamburg"
                where customListsHolder.NestedListsHolderList[2].StringList[4] == "Nested"
                where customListsHolder.IntIList[1] == 12
                where customListsHolder.NestedListsHolderIList[12].IntIList[12] == 2
                select customListsHolder;

            _ = from customEnumerableHolder in GetMongoQueryable<CustomEnumerableHolder>()
                where customEnumerableHolder.Enumerable1.Count() == 121 &&
                      customEnumerableHolder.Enumerable1.ElementAt(12) == 1 &&
                      customEnumerableHolder.Enumerable2.Count() == 22 &&
                      customEnumerableHolder.Enumerable2.ElementAt(12).Enumerable2.ElementAt(21).Enumerable1.ElementAt(1) == 2
                select customEnumerableHolder;
        }
    }
}
