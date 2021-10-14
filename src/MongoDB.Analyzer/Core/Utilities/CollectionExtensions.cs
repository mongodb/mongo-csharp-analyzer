using System.Collections.Generic;
using System.Linq;

namespace MongoDB.Analyzer.Core
{
    internal static class CollectionExtensions
    {
        public static bool AnySafe<T>(this IEnumerable<T> enumerable) =>
            enumerable?.Any() == true;

        public static bool EmptyOrNull<T>(this IEnumerable<T> enumerable) =>
            enumerable?.Any() != true;
    }
}
