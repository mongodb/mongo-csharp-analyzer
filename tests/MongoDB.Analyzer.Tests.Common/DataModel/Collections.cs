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
    public class CustomEnumerableHolder
    {
        public CustomIEnumerable<int> Enumerable1 { get; set; }
        public CustomIEnumerable<EnumerableHolder> Enumerable2 { get; set; }
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

    public class EnumerableHolder
    {
        public IEnumerable<int> Enumerable1 { get; set; }
        public System.Collections.Generic.IEnumerable<EnumerableHolder> Enumerable2 { get; set; }
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

    public class DictionariesHolder
    {
        public Dictionary<string, int> IntDictionary { get; set; }
        public Dictionary<string, Person> PesonsDictionary { get; set; }
        public System.Collections.Generic.Dictionary<string, string> StringDictionary { get; set; }
        public System.Collections.Generic.Dictionary<string, DictionariesHolder> NestedDictionariesHolderDictionary { get; set; }

        public IDictionary<string, int> IntIDictionary { get; set; }
        public System.Collections.Generic.IDictionary<string, DictionariesHolder> NestedDictionariesHolderIDictionary { get; set; }
    }
}
