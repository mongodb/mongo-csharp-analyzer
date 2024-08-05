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
        [NoDiagnostics]
        public void CustomDictionaries()
        {
            _ = GetMongoQueryable<CustomDictionariesHolder>().Where(t =>
                    t.IntDictionary["key"] == 1 &&
                    t.StringDictionary["key"] == "Value" &&
                    t.PesonsDictionary["string"].Name == "Bob" &&
                    t.NestedDictionariesHolderDictionary["key"].IntDictionary["key"] == 1 &&
                    t.IntIDictionary["key"] == 3 &&
                    t.NestedDictionariesHolderIDictionary["key"].IntDictionary["key"] == 3);
        }

        [NoDiagnostics]
        public void CustomEnumerables()
        {
            _ = GetMongoQueryable<CustomEnumerableHolder>().Where(t =>
                    t.Enumerable1.Count() == 121 &&
                    t.Enumerable1.ElementAt(12) == 1 &&
                    t.Enumerable2.Count() == 22 &&
                    t.Enumerable2.ElementAt(12).Enumerable2.ElementAt(21).Enumerable1.ElementAt(1) == 2);
        }

        [NoDiagnostics]
        public void CustomHashSets()
        {
            _ = GetMongoQueryable<CustomHashSetsHolder>().Where(t =>
                    t.IntHashSet.ElementAt(0) == 2 &&
                    t.PesonsHashSet.ElementAt(0).SiblingsCount == 2 &&
                    t.StringHashSet.ElementAt(0) == "Value" &&
                    t.NestedHashSetsHolderHashSet.ElementAt(0).IntHashSet.ElementAt(0) == 2);
        }

        [NoDiagnostics]
        public void CustomLinkedLists()
        {
            _ = GetMongoQueryable<CustomLinkedListHolder>().Where(t =>
                    t.IntLinkedList.ElementAt(0) == 1 &&
                    t.StringLinkedList.ElementAt(0) == "Value" &&
                    t.PesonsLinkedList.ElementAt(0).Name == "Bob" &&
                    t.NestedLinkedListsHolderLinkedList.ElementAt(0).IntLinkedList.ElementAt(0) == 1);
        }

        [NoDiagnostics]
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

        [NoDiagnostics]
        public void CustomQueues()
        {
            _ = GetMongoQueryable<CustomQueueHolder>()
                    .Where(t => t.IntQueue.ElementAt(0) == 1)
                    .Where(t => t.PesonsQueue.ElementAt(0).Name == "Name")
                    .Where(t => t.StringQueue.ElementAt(0) == "Value")
                    .Where(t => t.NestedQueuesHolderQueue.ElementAt(0).IntQueue.ElementAt(0) == 1);
        }

        [NoDiagnostics]
        public void CustomSortedDictionaries()
        {
            _ = GetMongoQueryable<CustomSortedDictionaryHolder>()
                    .Where(t => t.IntSortedDictionary["key"] == 1)
                    .Where(t => t.StringSortedDictionary["key"] == "Value")
                    .Where(t => t.PesonsSortedDictionary["string"].Name == "Bob")
                    .Where(t => t.NestedSortedDictionariesHolderSortedDictionary["key"].IntSortedDictionary["key"] == 1);
        }

        [NoDiagnostics]
        public void CustomStacks()
        {
            _ = GetMongoQueryable<CustomStackHolder>()
                    .Where(t => t.IntStack.ElementAt(0) == 1)
                    .Where(t => t.PesonsStack.ElementAt(0).Name == "Name")
                    .Where(t => t.StringStack.ElementAt(0) == "Value")
                    .Where(t => t.NestedStacksHolderStack.ElementAt(0).IntStack.ElementAt(0) == 1);
        }

        [MQL("aggregate([{ \"$match\" : { \"IntDictionary.key\" : 1, \"StringDictionary.key\" : \"Value\", \"PesonsDictionary.string.Name\" : \"Bob\", \"NestedDictionariesHolderDictionary.key.IntDictionary.key\" : 1, \"IntIDictionary.key\" : 3, \"NestedDictionariesHolderIDictionary.key.IntDictionary.key\" : 3 } }])")]
        public void Dictionaries()
        {
            _ = GetMongoQueryable<DictionariesHolder>().Where(t =>
                    t.IntDictionary["key"] == 1 &&
                    t.StringDictionary["key"] == "Value" &&
                    t.PesonsDictionary["string"].Name == "Bob" &&
                    t.NestedDictionariesHolderDictionary["key"].IntDictionary["key"] == 1 &&
                    t.IntIDictionary["key"] == 3 &&
                    t.NestedDictionariesHolderIDictionary["key"].IntDictionary["key"] == 3);
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

        [MQL("aggregate([{ \"$match\" : { \"IntHashSet.0\" : 2, \"PesonsHashSet.0.SiblingsCount\" : 2, \"StringHashSet.0\" : \"Value\", \"NestedHashSetsHolderHashSet.0.IntHashSet.0\" : 2 } }])")]
        public void HashSets()
        {
            _ = GetMongoQueryable<HashSetHolder>().Where(t =>
                    t.IntHashSet.ElementAt(0) == 2 &&
                    t.PesonsHashSet.ElementAt(0).SiblingsCount == 2 &&
                    t.StringHashSet.ElementAt(0) == "Value" &&
                    t.NestedHashSetsHolderHashSet.ElementAt(0).IntHashSet.ElementAt(0) == 2);
        }

        [MQL("aggregate([{ \"$match\" : { \"IntLinkedList.0\" : 1, \"StringLinkedList.0\" : \"Value\", \"PesonsLinkedList.0.Name\" : \"Bob\", \"NestedLinkedListsHolderLinkedList.0.IntLinkedList.0\" : 1 } }])")]
        public void LinkedLists()
        {
            _ = GetMongoQueryable<LinkedListHolder>().Where(t =>
                    t.IntLinkedList.ElementAt(0) == 1 &&
                    t.StringLinkedList.ElementAt(0) == "Value" &&
                    t.PesonsLinkedList.ElementAt(0).Name == "Bob" &&
                    t.NestedLinkedListsHolderLinkedList.ElementAt(0).IntLinkedList.ElementAt(0) == 1);
        }

        [MQL("aggregate([{ \"$match\" : { \"IntLinkedListNode.Value\" : 1, \"StringLinkedListNode.Value\" : \"Value\", \"PesonsLinkedListNode.Value.Name\" : \"Bob\", \"NestedLinkedListNodesHolderLinkedListNode.Value.IntLinkedListNode.Value\" : 1 } }])")]
        public void LinkedListNodes()
        {
            _ = GetMongoQueryable<LinkedListNodeHolder>().Where(t =>
                    t.IntLinkedListNode.Value == 1 &&
                    t.StringLinkedListNode.Value == "Value" &&
                    t.PesonsLinkedListNode.Value.Name == "Bob" &&
                    t.NestedLinkedListNodesHolderLinkedListNode.Value.IntLinkedListNode.Value == 1);
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

        [MQL("aggregate([{ \"$match\" : { \"IntDictionary.key\" : 1 } }, { \"$match\" : { \"StringDictionary.key\" : \"Value\" } }, { \"$match\" : { \"PesonsDictionary.string.Name\" : \"Bob\" } }, { \"$match\" : { \"NestedDictionariesHolderDictionary.key.IntDictionary.key\" : 1 } }, { \"$match\" : { \"IntIDictionary.key\" : 3 } }, { \"$match\" : { \"NestedDictionariesHolderIDictionary.key.IntDictionary.key\" : 3 } }])")]
        [MQL("aggregate([{ \"$match\" : { \"Enumerable1\" : { \"$size\" : 121 }, \"Enumerable1.12\" : 1, \"Enumerable2\" : { \"$size\" : 22 }, \"Enumerable2.12.Enumerable2.21.Enumerable1.1\" : 2 } }])")]
        [MQL("aggregate([{ \"$match\" : { \"IntHashSet.0\" : 2, \"PesonsHashSet.0.SiblingsCount\" : 2, \"StringHashSet.0\" : \"Value\", \"NestedHashSetsHolderHashSet.0.IntHashSet.0\" : 2 } }])")]
        [MQL("aggregate([{ \"$match\" : { \"IntLinkedList.0\" : 1, \"StringLinkedList.0\" : \"Value\", \"PesonsLinkedList.0.Name\" : \"Bob\", \"NestedLinkedListsHolderLinkedList.0.IntLinkedList.0\" : 1 } }])")]
        [MQL("aggregate([{ \"$match\" : { \"IntLinkedListNode.Value\" : 1, \"StringLinkedListNode.Value\" : \"Value\", \"PesonsLinkedListNode.Value.Name\" : \"Bob\", \"NestedLinkedListNodesHolderLinkedListNode.Value.IntLinkedListNode.Value\" : 1 } }])")]
        [MQL("aggregate([{ \"$match\" : { \"IntList.0\" : 2 } }, { \"$match\" : { \"StringList\" : { \"$size\" : 12 } } }, { \"$match\" : { \"PesonsList.2.Address.City\" : \"Hamburg\" } }, { \"$match\" : { \"NestedListsHolderList.2.StringList.4\" : \"Nested\" } }, { \"$match\" : { \"IntIList.1\" : 12 } }, { \"$match\" : { \"NestedListsHolderIList.12.IntIList.12\" : 2 } }])")]
        [MQL("aggregate([{ \"$match\" : { \"IntQueue.0\" : 1, \"PesonsQueue.0.Name\" : \"Name\", \"StringQueue.0\" : \"Value\", \"NestedQueuesHolderQueue.0.IntQueue.0\" : 1 } }])")]
        [MQL("aggregate([{ \"$match\" : { \"IntSortedDictionary.key\" : 1, \"StringSortedDictionary.key\" : \"Value\", \"PesonsSortedDictionary.string.Name\" : \"Bob\", \"NestedSortedDictionariesHolderSortedDictionary.key.IntSortedDictionary.key\" : 1 } }])")]
        [MQL("aggregate([{ \"$match\" : { \"IntStack.0\" : 1, \"PesonsStack.0.Name\" : \"Name\", \"StringStack.0\" : \"Value\", \"NestedStacksHolderStack.0.IntStack.0\" : 1 } }])")]
        public void Query_syntax()
        {
            _ = from dictionariesHolder in GetMongoQueryable<DictionariesHolder>()
                where dictionariesHolder.IntDictionary["key"] == 1
                where dictionariesHolder.StringDictionary["key"] == "Value"
                where dictionariesHolder.PesonsDictionary["string"].Name == "Bob"
                where dictionariesHolder.NestedDictionariesHolderDictionary["key"].IntDictionary["key"] == 1
                where dictionariesHolder.IntIDictionary["key"] == 3
                where dictionariesHolder.NestedDictionariesHolderIDictionary["key"].IntDictionary["key"] == 3
                select dictionariesHolder;

            _ = from enumerableHolder in GetMongoQueryable<EnumerableHolder>()
                where enumerableHolder.Enumerable1.Count() == 121 &&
                      enumerableHolder.Enumerable1.ElementAt(12) == 1 &&
                      enumerableHolder.Enumerable2.Count() == 22 &&
                      enumerableHolder.Enumerable2.ElementAt(12).Enumerable2.ElementAt(21).Enumerable1.ElementAt(1) == 2
                select enumerableHolder;

            _ = from hashSetHolder in GetMongoQueryable<HashSetHolder>()
                where hashSetHolder.IntHashSet.ElementAt(0) == 2 &&
                      hashSetHolder.PesonsHashSet.ElementAt(0).SiblingsCount == 2 &&
                      hashSetHolder.StringHashSet.ElementAt(0) == "Value" &&
                      hashSetHolder.NestedHashSetsHolderHashSet.ElementAt(0).IntHashSet.ElementAt(0) == 2
                select hashSetHolder;

            _ = from linkedListHolder in GetMongoQueryable<LinkedListHolder>()
                where linkedListHolder.IntLinkedList.ElementAt(0) == 1 &&
                      linkedListHolder.StringLinkedList.ElementAt(0) == "Value" &&
                      linkedListHolder.PesonsLinkedList.ElementAt(0).Name == "Bob" &&
                      linkedListHolder.NestedLinkedListsHolderLinkedList.ElementAt(0).IntLinkedList.ElementAt(0) == 1
                select linkedListHolder;

            _ = from linkedListNodeHolder in GetMongoQueryable<LinkedListNodeHolder>()
                where linkedListNodeHolder.IntLinkedListNode.Value == 1 &&
                      linkedListNodeHolder.StringLinkedListNode.Value == "Value" &&
                      linkedListNodeHolder.PesonsLinkedListNode.Value.Name == "Bob" &&
                      linkedListNodeHolder.NestedLinkedListNodesHolderLinkedListNode.Value.IntLinkedListNode.Value == 1
                select linkedListNodeHolder;

            _ = from listsHolder in GetMongoQueryable<ListsHolder>()
                where listsHolder.IntList[0] == 2
                where listsHolder.StringList.Count == 12
                where listsHolder.PesonsList[2].Address.City == "Hamburg"
                where listsHolder.NestedListsHolderList[2].StringList[4] == "Nested"
                where listsHolder.IntIList[1] == 12
                where listsHolder.NestedListsHolderIList[12].IntIList[12] == 2
                select listsHolder;


            _ = from queueHolder in GetMongoQueryable<QueueHolder>()
                where queueHolder.IntQueue.ElementAt(0) == 1 &&
                      queueHolder.PesonsQueue.ElementAt(0).Name == "Name" &&
                      queueHolder.StringQueue.ElementAt(0) == "Value" &&
                      queueHolder.NestedQueuesHolderQueue.ElementAt(0).IntQueue.ElementAt(0) == 1
                select queueHolder;

            _ = from sortedDictionaryHolder in GetMongoQueryable<SortedDictionaryHolder>()
                where sortedDictionaryHolder.IntSortedDictionary["key"] == 1 &&
                      sortedDictionaryHolder.StringSortedDictionary["key"] == "Value" &&
                      sortedDictionaryHolder.PesonsSortedDictionary["string"].Name == "Bob" &&
                      sortedDictionaryHolder.NestedSortedDictionariesHolderSortedDictionary["key"].IntSortedDictionary["key"] == 1
                select sortedDictionaryHolder;

            _ = from stackHolder in GetMongoQueryable<StackHolder>()
                where stackHolder.IntStack.ElementAt(0) == 1 &&
                      stackHolder.PesonsStack.ElementAt(0).Name == "Name" &&
                      stackHolder.StringStack.ElementAt(0) == "Value" &&
                      stackHolder.NestedStacksHolderStack.ElementAt(0).IntStack.ElementAt(0) == 1
                select stackHolder;

            _ = from customDictionariesHolder in GetMongoQueryable<CustomDictionariesHolder>()
                where customDictionariesHolder.IntDictionary["key"] == 1
                where customDictionariesHolder.StringDictionary["key"] == "Value"
                where customDictionariesHolder.PesonsDictionary["string"].Name == "Bob"
                where customDictionariesHolder.NestedDictionariesHolderDictionary["key"].IntDictionary["key"] == 1
                where customDictionariesHolder.IntIDictionary["key"] == 3
                where customDictionariesHolder.NestedDictionariesHolderIDictionary["key"].IntDictionary["key"] == 3
                select customDictionariesHolder;

            _ = from customEnumerableHolder in GetMongoQueryable<CustomEnumerableHolder>()
                where customEnumerableHolder.Enumerable1.Count() == 121 &&
                      customEnumerableHolder.Enumerable1.ElementAt(12) == 1 &&
                      customEnumerableHolder.Enumerable2.Count() == 22 &&
                      customEnumerableHolder.Enumerable2.ElementAt(12).Enumerable2.ElementAt(21).Enumerable1.ElementAt(1) == 2
                select customEnumerableHolder;

            _ = from customHashSetsHolder in GetMongoQueryable<CustomHashSetsHolder>()
                where customHashSetsHolder.IntHashSet.ElementAt(0) == 2 &&
                      customHashSetsHolder.PesonsHashSet.ElementAt(0).SiblingsCount == 2 &&
                      customHashSetsHolder.StringHashSet.ElementAt(0) == "Value" &&
                      customHashSetsHolder.NestedHashSetsHolderHashSet.ElementAt(0).IntHashSet.ElementAt(0) == 2
                select customHashSetsHolder;

            _ = from customLinkedListHolder in GetMongoQueryable<CustomLinkedListHolder>()
                where customLinkedListHolder.IntLinkedList.ElementAt(0) == 1 &&
                      customLinkedListHolder.StringLinkedList.ElementAt(0) == "Value" &&
                      customLinkedListHolder.PesonsLinkedList.ElementAt(0).Name == "Bob" &&
                      customLinkedListHolder.NestedLinkedListsHolderLinkedList.ElementAt(0).IntLinkedList.ElementAt(0) == 1
                select customLinkedListHolder;

            _ = from customListsHolder in GetMongoQueryable<CustomListsHolder>()
                where customListsHolder.IntList[0] == 2
                where customListsHolder.StringList.Count == 12
                where customListsHolder.PesonsList[2].Address.City == "Hamburg"
                where customListsHolder.NestedListsHolderList[2].StringList[4] == "Nested"
                where customListsHolder.IntIList[1] == 12
                where customListsHolder.NestedListsHolderIList[12].IntIList[12] == 2
                select customListsHolder;

            _ = from customQueueHolder in GetMongoQueryable<CustomQueueHolder>()
                where customQueueHolder.IntQueue.ElementAt(0) == 1 &&
                      customQueueHolder.PesonsQueue.ElementAt(0).Name == "Name" &&
                      customQueueHolder.StringQueue.ElementAt(0) == "Value" &&
                      customQueueHolder.NestedQueuesHolderQueue.ElementAt(0).IntQueue.ElementAt(0) == 1
                select customQueueHolder;

            _ = from customSortedDictionaryHolder in GetMongoQueryable<CustomSortedDictionaryHolder>()
                where customSortedDictionaryHolder.IntSortedDictionary["key"] == 1 &&
                      customSortedDictionaryHolder.StringSortedDictionary["key"] == "Value" &&
                      customSortedDictionaryHolder.PesonsSortedDictionary["string"].Name == "Bob" &&
                      customSortedDictionaryHolder.NestedSortedDictionariesHolderSortedDictionary["key"].IntSortedDictionary["key"] == 1
                select customSortedDictionaryHolder;

            _ = from customStackHolder in GetMongoQueryable<CustomStackHolder>()
                where customStackHolder.IntStack.ElementAt(0) == 1 &&
                      customStackHolder.PesonsStack.ElementAt(0).Name == "Name" &&
                      customStackHolder.StringStack.ElementAt(0) == "Value" &&
                      customStackHolder.NestedStacksHolderStack.ElementAt(0).IntStack.ElementAt(0) == 1
                select customStackHolder;
        }

        [MQL("aggregate([{ \"$match\" : { \"IntQueue.0\" : 1 } }, { \"$match\" : { \"PesonsQueue.0.Name\" : \"Name\" } }, { \"$match\" : { \"StringQueue.0\" : \"Value\" } }, { \"$match\" : { \"NestedQueuesHolderQueue.0.IntQueue.0\" : 1 } }])")]
        public void Queues()
        {
            _ = GetMongoQueryable<QueueHolder>()
                    .Where(t => t.IntQueue.ElementAt(0) == 1)
                    .Where(t => t.PesonsQueue.ElementAt(0).Name == "Name")
                    .Where(t => t.StringQueue.ElementAt(0) == "Value")
                    .Where(t => t.NestedQueuesHolderQueue.ElementAt(0).IntQueue.ElementAt(0) == 1);
        }

        [MQL("aggregate([{ \"$match\" : { \"IntSortedDictionary.key\" : 1 } }, { \"$match\" : { \"StringSortedDictionary.key\" : \"Value\" } }, { \"$match\" : { \"PesonsSortedDictionary.string.Name\" : \"Bob\" } }, { \"$match\" : { \"NestedSortedDictionariesHolderSortedDictionary.key.IntSortedDictionary.key\" : 1 } }])")]
        public void SortedDictionaries()
        {
            _ = GetMongoQueryable<SortedDictionaryHolder>()
                    .Where(t => t.IntSortedDictionary["key"] == 1)
                    .Where(t => t.StringSortedDictionary["key"] == "Value")
                    .Where(t => t.PesonsSortedDictionary["string"].Name == "Bob")
                    .Where(t => t.NestedSortedDictionariesHolderSortedDictionary["key"].IntSortedDictionary["key"] == 1);
        }

        [MQL("aggregate([{ \"$match\" : { \"IntStack.0\" : 1 } }, { \"$match\" : { \"PesonsStack.0.Name\" : \"Name\" } }, { \"$match\" : { \"StringStack.0\" : \"Value\" } }, { \"$match\" : { \"NestedStacksHolderStack.0.IntStack.0\" : 1 } }])")]
        public void Stacks()
        {
            _ = GetMongoQueryable<StackHolder>()
                    .Where(t => t.IntStack.ElementAt(0) == 1)
                    .Where(t => t.PesonsStack.ElementAt(0).Name == "Name")
                    .Where(t => t.StringStack.ElementAt(0) == "Value")
                    .Where(t => t.NestedStacksHolderStack.ElementAt(0).IntStack.ElementAt(0) == 1);
        }
    }
}
