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

namespace MongoDB.Analyzer.Core;

internal static class CollectionExtensions
{
    public static bool AnySafe<T>(this IEnumerable<T> enumerable) =>
        enumerable?.Any() ?? false;

    public static bool AnySafe<T>(this IEnumerable<T> enumerable, Func<T, bool> predicate) =>
        enumerable?.Any(predicate) ?? false;

    public static bool EmptyOrNull<T>(this IEnumerable<T> enumerable) =>
        enumerable?.Any() != true;

    public static ImmutableArray<T> CreateImmutableArray<T>(params IEnumerable<T>[] enumerables) =>
        ImmutableArray.Create(enumerables.Aggregate((prev, curr) => prev.Concat(curr)).ToArray());

    public static bool TryGetValueSafe<K, V>(this IDictionary<K, V> dictionary, K key, out V value)
    {
        value = default;

        return dictionary?.TryGetValue(key, out value) ?? false;
    }
}
