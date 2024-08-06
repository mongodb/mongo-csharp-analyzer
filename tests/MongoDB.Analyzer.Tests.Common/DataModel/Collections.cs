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

using System.Collections;
using System.Collections.Generic;

namespace MongoDB.Analyzer.Tests.Common.DataModel
{
    public class CustomDictionariesHolder
    {
        public CustomDictionary<string, int> IntDictionary { get; set; }
        public CustomDictionary<string, Person> PesonsDictionary { get; set; }
        public CustomDictionary<string, string> StringDictionary { get; set; }
        public CustomDictionary<string, CustomDictionariesHolder> NestedDictionariesHolderDictionary { get; set; }

        public CustomIDictionary<string, int> IntIDictionary { get; set; }
        public CustomIDictionary<string, CustomDictionariesHolder> NestedDictionariesHolderIDictionary { get; set; }
    }

    public class CustomEnumerableHolder
    {
        public CustomIEnumerable<int> Enumerable1 { get; set; }
        public CustomIEnumerable<EnumerableHolder> Enumerable2 { get; set; }
    }

    public class CustomIDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        public TValue this[TKey key] { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        public ICollection<TKey> Keys => throw new System.NotImplementedException();

        public ICollection<TValue> Values => throw new System.NotImplementedException();

        public int Count => throw new System.NotImplementedException();

        public bool IsReadOnly => throw new System.NotImplementedException();

        public void Add(TKey key, TValue value) => throw new System.NotImplementedException();
        public void Add(KeyValuePair<TKey, TValue> item) => throw new System.NotImplementedException();
        public void Clear() => throw new System.NotImplementedException();
        public bool Contains(KeyValuePair<TKey, TValue> item) => throw new System.NotImplementedException();
        public bool ContainsKey(TKey key) => throw new System.NotImplementedException();
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) => throw new System.NotImplementedException();
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => throw new System.NotImplementedException();
        public bool Remove(TKey key) => throw new System.NotImplementedException();
        public bool Remove(KeyValuePair<TKey, TValue> item) => throw new System.NotImplementedException();
        public bool TryGetValue(TKey key, out TValue value) => throw new System.NotImplementedException();
        IEnumerator IEnumerable.GetEnumerator() => throw new System.NotImplementedException();
    }

    public class CustomIEnumerable<T> : IEnumerable<T>
    {
        public IEnumerator<T> GetEnumerator() => throw new System.NotImplementedException();
        IEnumerator IEnumerable.GetEnumerator() => throw new System.NotImplementedException();
    }

    public class CustomIList<T> : IList<T>
    {
        public T this[int index] { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public int Count => throw new System.NotImplementedException();
        public bool IsReadOnly => throw new System.NotImplementedException();
        public void Add(T item) => throw new System.NotImplementedException();
        public void Clear() => throw new System.NotImplementedException();
        public bool Contains(T item) => throw new System.NotImplementedException();
        public void CopyTo(T[] array, int arrayIndex) => throw new System.NotImplementedException();
        public IEnumerator<T> GetEnumerator() => throw new System.NotImplementedException();
        public int IndexOf(T item) => throw new System.NotImplementedException();
        public void Insert(int index, T item) => throw new System.NotImplementedException();
        public bool Remove(T item) => throw new System.NotImplementedException();
        public void RemoveAt(int index) => throw new System.NotImplementedException();
        IEnumerator IEnumerable.GetEnumerator() => throw new System.NotImplementedException();
    }

    public class CustomDictionary<TKey, TValue> : Dictionary<TKey, TValue> { }

    public class CustomHashSet<T> : HashSet<T> { }

    public class CustomHashSetsHolder
    {
        public CustomHashSet<int> IntHashSet { get; set; }
        public CustomHashSet<Person> PesonsHashSet { get; set; }
        public CustomHashSet<string> StringHashSet { get; set; }
        public CustomHashSet<CustomHashSetsHolder> NestedHashSetsHolderHashSet { get; set; }
    }

    public class CustomLinkedList<T> : LinkedList<T> { }

    public class CustomLinkedListHolder
    {
        public CustomLinkedList<int> IntLinkedList { get; set; }
        public CustomLinkedList<Person> PesonsLinkedList { get; set; }
        public CustomLinkedList<string> StringLinkedList { get; set; }
        public CustomLinkedList<CustomLinkedListHolder> NestedLinkedListsHolderLinkedList { get; set; }
    }


    public class CustomList<T> : List<T> { }

    public class CustomListsHolder
    {
        public CustomList<int> IntList { get; set; }
        public CustomList<Person> PesonsList { get; set; }
        public CustomList<string> StringList { get; set; }
        public CustomList<CustomListsHolder> NestedListsHolderList { get; set; }

        public CustomIList<int> IntIList { get; set; }
        public CustomIList<CustomListsHolder> NestedListsHolderIList { get; set; }
    }

    public class CustomQueue<T>: Queue<T> { }

    public class CustomQueueHolder
    {
        public CustomQueue<int> IntQueue { get; set; }
        public CustomQueue<Person> PesonsQueue { get; set; }
        public CustomQueue<string> StringQueue { get; set; }
        public CustomQueue<QueueHolder> NestedQueuesHolderQueue { get; set; }
    }

    public class CustomSortedDictionary<TKey, TValue> : SortedDictionary<TKey, TValue> { }

    public class CustomSortedDictionaryHolder
    {
        public CustomSortedDictionary<string, int> IntSortedDictionary { get; set; }
        public CustomSortedDictionary<string, Person> PesonsSortedDictionary { get; set; }
        public CustomSortedDictionary<string, string> StringSortedDictionary { get; set; }
        public CustomSortedDictionary<string, CustomSortedDictionaryHolder> NestedSortedDictionariesHolderSortedDictionary { get; set; }
    }

    public class CustomStack<T> : Stack<T> { }

    public class CustomStackHolder
    {
        public CustomStack<int> IntStack { get; set; }
        public CustomStack<Person> PesonsStack { get; set; }
        public CustomStack<string> StringStack { get; set; }
        public CustomStack<StackHolder> NestedStacksHolderStack { get; set; }
    }

    public class DictionariesHolder
    {
        public Dictionary<string, int> IntDictionary { get; set; }
        public Dictionary<string, Person> PesonsDictionary { get; set; }
        public System.Collections.Generic.Dictionary<string, string> StringDictionary { get; set; }
        public System.Collections.Generic.Dictionary<string, DictionariesHolder> NestedDictionariesHolderDictionary { get; set; }

        public IDictionary<string, int> IntIDictionary { get; set; }
        public System.Collections.Generic.IDictionary<string, DictionariesHolder> NestedDictionariesHolderIDictionary { get; set; }
    }

    public class EnumerableHolder
    {
        public IEnumerable<int> Enumerable1 { get; set; }
        public System.Collections.Generic.IEnumerable<EnumerableHolder> Enumerable2 { get; set; }
    }

    public class HashSetHolder
    {
        public HashSet<int> IntHashSet { get; set; }
        public HashSet<Person> PesonsHashSet { get; set; }
        public System.Collections.Generic.HashSet<string> StringHashSet { get; set; }
        public System.Collections.Generic.HashSet<HashSetHolder> NestedHashSetsHolderHashSet { get; set; }
    }

    public class LinkedListHolder
    {
        public LinkedList<int> IntLinkedList { get; set; }
        public LinkedList<Person> PesonsLinkedList { get; set; }
        public System.Collections.Generic.LinkedList<string> StringLinkedList { get; set; }
        public System.Collections.Generic.LinkedList<LinkedListHolder> NestedLinkedListsHolderLinkedList { get; set; }
    }

    public class LinkedListNodeHolder
    {
        public LinkedListNode<int> IntLinkedListNode { get; set; }
        public LinkedListNode<Person> PesonsLinkedListNode { get; set; }
        public System.Collections.Generic.LinkedListNode<string> StringLinkedListNode { get; set; }
        public System.Collections.Generic.LinkedListNode<LinkedListNodeHolder> NestedLinkedListNodesHolderLinkedListNode { get; set; }
    }

    public class ListsHolder
    {
        public List<int> IntList { get; set; }
        public List<Person> PesonsList { get; set; }
        public System.Collections.Generic.List<string> StringList { get; set; }
        public System.Collections.Generic.List<ListsHolder> NestedListsHolderList { get; set; }

        public IList<int> IntIList { get; set; }
        public System.Collections.Generic.IList<ListsHolder> NestedListsHolderIList { get; set; }
    }

    public class QueueHolder
    {
        public Queue<int> IntQueue { get; set; }
        public Queue<Person> PesonsQueue { get; set; }
        public System.Collections.Generic.Queue<string> StringQueue { get; set; }
        public System.Collections.Generic.Queue<QueueHolder> NestedQueuesHolderQueue { get; set; }
    }

    public class SortedDictionaryHolder
    {
        public SortedDictionary<string, int> IntSortedDictionary { get; set; }
        public SortedDictionary<string, Person> PesonsSortedDictionary { get; set; }
        public System.Collections.Generic.SortedDictionary<string, string> StringSortedDictionary { get; set; }
        public System.Collections.Generic.SortedDictionary<string, SortedDictionaryHolder> NestedSortedDictionariesHolderSortedDictionary { get; set; }
    }

    public class StackHolder
    {
        public Stack<int> IntStack { get; set; }
        public Stack<Person> PesonsStack { get; set; }
        public System.Collections.Generic.Stack<string> StringStack { get; set; }
        public System.Collections.Generic.Stack<StackHolder> NestedStacksHolderStack { get; set; }
    }
}
