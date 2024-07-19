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
        [NoDiagnostics]
        public void CustomDictionaries()
        {
            _ = Builders<CustomDictionariesHolder>.Filter.Eq(t => t.IntDictionary["key"], 1) |
                Builders<CustomDictionariesHolder>.Filter.Eq(t => t.StringDictionary["key"], "Value") |
                Builders<CustomDictionariesHolder>.Filter.Eq(t => t.PesonsDictionary["string"].Name, "Bob") |
                Builders<CustomDictionariesHolder>.Filter.Eq(t => t.NestedDictionariesHolderDictionary["key"].IntDictionary["key"], 1) |
                Builders<CustomDictionariesHolder>.Filter.Eq(t => t.IntIDictionary["key"], 3) |
                Builders<CustomDictionariesHolder>.Filter.Eq(t => t.NestedDictionariesHolderIDictionary["key"].IntDictionary["key"], 3);
        }

        [NoDiagnostics]
        public void CustomEnumerables()
        {
            _ = Builders<CustomEnumerableHolder>.Filter.Eq(t => t.Enumerable1.ElementAt(0), 2) |
                Builders<CustomEnumerableHolder>.Filter.Eq(t => t.Enumerable2.ElementAt(1).Enumerable1.ElementAt(0), 2);
        }

        [NoDiagnostics]
        public void CustomHashSets()
        {
            _ = Builders<CustomHashSetsHolder>.Filter.Eq(t => t.IntHashSet.ElementAt(0), 2) |
                Builders<CustomHashSetsHolder>.Filter.Eq(t => t.PesonsHashSet.ElementAt(0).SiblingsCount, 2) |
                Builders<CustomHashSetsHolder>.Filter.Eq(t => t.StringHashSet.ElementAt(0), "Value") |
                Builders<CustomHashSetsHolder>.Filter.Eq(t => t.NestedHashSetsHolderHashSet.ElementAt(0).IntHashSet.ElementAt(0), 2);
        }

        [NoDiagnostics]
        public void CustomLinkedLists()
        {
            _ = Builders<CustomLinkedListHolder>.Filter.Eq(t => t.IntLinkedList.ElementAt(0), 1) |
                Builders<CustomLinkedListHolder>.Filter.Eq(t => t.StringLinkedList.ElementAt(0), "Value") |
                Builders<CustomLinkedListHolder>.Filter.Eq(t => t.PesonsLinkedList.ElementAt(0).Name, "Bob") |
                Builders<CustomLinkedListHolder>.Filter.Eq(t => t.NestedLinkedListsHolderLinkedList.ElementAt(0).IntLinkedList.ElementAt(0), 1);
        }

        [NoDiagnostics]
        public void CustomLists()
        {
            _ = Builders<CustomListsHolder>.Filter.Eq(t => t.IntList[2], 1) |
                Builders<CustomListsHolder>.Filter.Eq(t => t.StringList[3], "Value") |
                Builders<CustomListsHolder>.Filter.Eq(t => t.PesonsList[4].Name, "Bob") |
                Builders<CustomListsHolder>.Filter.Eq(t => t.NestedListsHolderList[5].IntList[1], 1) |
                Builders<CustomListsHolder>.Filter.Eq(t => t.IntIList[4], 3) |
                Builders<CustomListsHolder>.Filter.Eq(t => t.NestedListsHolderIList[15].IntList[3], 3);
        }

        [NoDiagnostics]
        public void CustomQueues()
        {
            _ = Builders<CustomQueueHolder>.Filter.Eq(t => t.IntQueue.Dequeue(), 1) |
                Builders<CustomQueueHolder>.Filter.Eq(t => t.PesonsQueue.Dequeue().Name, "Name") |
                Builders<CustomQueueHolder>.Filter.Eq(t => t.StringQueue.Dequeue(), "Value") |
                Builders<CustomQueueHolder>.Filter.Eq(t => t.NestedQueuesHolderQueue.Dequeue().IntQueue.Dequeue(), 1);
        }

        [NoDiagnostics]
        public void CustomSortedDictionaries()
        {
            _ = Builders<CustomSortedDictionaryHolder>.Filter.Eq(t => t.IntSortedDictionary["key"], 1) |
                Builders<CustomSortedDictionaryHolder>.Filter.Eq(t => t.StringSortedDictionary["key"], "Value") |
                Builders<CustomSortedDictionaryHolder>.Filter.Eq(t => t.PesonsSortedDictionary["string"].Name, "Bob") |
                Builders<CustomSortedDictionaryHolder>.Filter.Eq(t => t.NestedSortedDictionariesHolderSortedDictionary["key"].IntSortedDictionary["key"], 1);
        }

        [NoDiagnostics]
        public void CustomStacks()
        {
            _ = Builders<CustomStackHolder>.Filter.Eq(t => t.IntStack.Pop(), 1) |
                Builders<CustomStackHolder>.Filter.Eq(t => t.PesonsStack.Pop().Name, "Name") |
                Builders<CustomStackHolder>.Filter.Eq(t => t.StringStack.Pop(), "Value") |
                Builders<CustomStackHolder>.Filter.Eq(t => t.NestedStacksHolderStack.Pop().IntStack.Pop(), 1);
        }

        [BuildersMQL("{ \"$or\" : [{ \"IntDictionary.key\" : 1 }, { \"StringDictionary.key\" : \"Value\" }, { \"PesonsDictionary.string.Name\" : \"Bob\" }, { \"NestedDictionariesHolderDictionary.key.IntDictionary.key\" : 1 }, { \"IntIDictionary.key\" : 3 }, { \"NestedDictionariesHolderIDictionary.key.IntDictionary.key\" : 3 }] }")]
        public void Dictionaries()
        {
            _ = Builders<DictionariesHolder>.Filter.Eq(t => t.IntDictionary["key"], 1) |
                Builders<DictionariesHolder>.Filter.Eq(t => t.StringDictionary["key"], "Value") |
                Builders<DictionariesHolder>.Filter.Eq(t => t.PesonsDictionary["string"].Name, "Bob") |
                Builders<DictionariesHolder>.Filter.Eq(t => t.NestedDictionariesHolderDictionary["key"].IntDictionary["key"], 1) |
                Builders<DictionariesHolder>.Filter.Eq(t => t.IntIDictionary["key"], 3) |
                Builders<DictionariesHolder>.Filter.Eq(t => t.NestedDictionariesHolderIDictionary["key"].IntDictionary["key"], 3);
        }

        [BuildersMQL("{ \"$or\" : [{ \"Enumerable1.0\" : 2 }, { \"Enumerable2.1.Enumerable1.0\" : 2 }] }")]
        public void Enumerables()
        {
            _ = Builders<EnumerableHolder>.Filter.Eq(t => t.Enumerable1.ElementAt(0), 2) |
                Builders<EnumerableHolder>.Filter.Eq(t => t.Enumerable2.ElementAt(1).Enumerable1.ElementAt(0), 2);
        }

        [BuildersMQL("{ \"$or\" : [{ \"IntHashSet.0\" : 2 }, { \"PesonsHashSet.0.SiblingsCount\" : 2 }, { \"StringHashSet.0\" : \"Value\" }, { \"NestedHashSetsHolderHashSet.0.IntHashSet.0\" : 2 }] }")]
        public void HashSets()
        {
            _ = Builders<HashSetHolder>.Filter.Eq(t => t.IntHashSet.ElementAt(0), 2) |
                Builders<HashSetHolder>.Filter.Eq(t => t.PesonsHashSet.ElementAt(0).SiblingsCount, 2) |
                Builders<HashSetHolder>.Filter.Eq(t => t.StringHashSet.ElementAt(0), "Value") |
                Builders<HashSetHolder>.Filter.Eq(t => t.NestedHashSetsHolderHashSet.ElementAt(0).IntHashSet.ElementAt(0), 2);
        }

        [BuildersMQL("{ \"$or\" : [{ \"IntLinkedList.0\" : 1 }, { \"StringLinkedList.0\" : \"Value\" }, { \"PesonsLinkedList.0.Name\" : \"Bob\" }, { \"NestedLinkedListsHolderLinkedList.0.IntLinkedList.0\" : 1 }] }")]
        public void LinkedLists()
        {
            _ = Builders<LinkedListHolder>.Filter.Eq(t => t.IntLinkedList.ElementAt(0), 1) |
                Builders<LinkedListHolder>.Filter.Eq(t => t.StringLinkedList.ElementAt(0), "Value") |
                Builders<LinkedListHolder>.Filter.Eq(t => t.PesonsLinkedList.ElementAt(0).Name, "Bob") |
                Builders<LinkedListHolder>.Filter.Eq(t => t.NestedLinkedListsHolderLinkedList.ElementAt(0).IntLinkedList.ElementAt(0), 1);
        }

        [BuildersMQL("{ \"$or\" : [{ \"IntLinkedListNode.Value\" : 1 }, { \"StringLinkedListNode.Value\" : \"Value\" }, { \"PesonsLinkedListNode.Value.Name\" : \"Bob\" }, { \"NestedLinkedListNodesHolderLinkedListNode.Value.IntLinkedListNode.Value\" : 1 }] }")]
        public void LinkedListNodes()
        {
            _ = Builders<LinkedListNodeHolder>.Filter.Eq(t => t.IntLinkedListNode.Value, 1) |
                Builders<LinkedListNodeHolder>.Filter.Eq(t => t.StringLinkedListNode.Value, "Value") |
                Builders<LinkedListNodeHolder>.Filter.Eq(t => t.PesonsLinkedListNode.Value.Name, "Bob") |
                Builders<LinkedListNodeHolder>.Filter.Eq(t => t.NestedLinkedListNodesHolderLinkedListNode.Value.IntLinkedListNode.Value, 1);
        }

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

        [BuildersMQL("{ \"$or\" : [{ \"IntQueue.0\" : 1 }, { \"PesonsQueue.0.Name\" : \"Name\" }, { \"StringQueue.0\" : \"Value\" }, { \"NestedQueuesHolderQueue.0.IntQueue.0\" : 1 }] }")]
        public void Queues()
        {
            _ = Builders<QueueHolder>.Filter.Eq(t => t.IntQueue.ElementAt(0), 1) |
                Builders<QueueHolder>.Filter.Eq(t => t.PesonsQueue.ElementAt(0).Name, "Name") |
                Builders<QueueHolder>.Filter.Eq(t => t.StringQueue.ElementAt(0), "Value") |
                Builders<QueueHolder>.Filter.Eq(t => t.NestedQueuesHolderQueue.ElementAt(0).IntQueue.ElementAt(0), 1);
        }

        [BuildersMQL("{ \"$or\" : [{ \"IntSortedDictionary.key\" : 1 }, { \"StringSortedDictionary.key\" : \"Value\" }, { \"PesonsSortedDictionary.string.Name\" : \"Bob\" }, { \"NestedSortedDictionariesHolderSortedDictionary.key.IntSortedDictionary.key\" : 1 }] }")]
        public void SortedDictionaries()
        {
            _ = Builders<SortedDictionaryHolder>.Filter.Eq(t => t.IntSortedDictionary["key"], 1) |
                Builders<SortedDictionaryHolder>.Filter.Eq(t => t.StringSortedDictionary["key"], "Value") |
                Builders<SortedDictionaryHolder>.Filter.Eq(t => t.PesonsSortedDictionary["string"].Name, "Bob") |
                Builders<SortedDictionaryHolder>.Filter.Eq(t => t.NestedSortedDictionariesHolderSortedDictionary["key"].IntSortedDictionary["key"], 1);
        }

        [BuildersMQL("{ \"$or\" : [{ \"IntStack.0\" : 1 }, { \"PesonsStack.0.Name\" : \"Name\" }, { \"StringStack.0\" : \"Value\" }, { \"NestedStacksHolderStack.0.IntStack.0\" : 1 }] }")]
        public void Stack()
        {
            _ = Builders<StackHolder>.Filter.Eq(t => t.IntStack.ElementAt(0), 1) |
                Builders<StackHolder>.Filter.Eq(t => t.PesonsStack.ElementAt(0).Name, "Name") |
                Builders<StackHolder>.Filter.Eq(t => t.StringStack.ElementAt(0), "Value") |
                Builders<StackHolder>.Filter.Eq(t => t.NestedStacksHolderStack.ElementAt(0).IntStack.ElementAt(0), 1);
        }
    }
}
