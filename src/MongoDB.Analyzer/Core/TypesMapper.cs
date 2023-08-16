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

using System.Text.RegularExpressions;

namespace MongoDB.Analyzer.Core;

internal sealed class TypesMapper
{
    private const string RegexLookahead = "(?![\\w])";
    private const string RegexLookbehind = "(?<![\\w\\.])";

    private Lazy<IDictionary<string, string>> _typesRemapping;

    public TypesMapper(string @namespace, IEnumerable<(string NewName, string PreviousName)> mapping)
    {
        _typesRemapping = new Lazy<IDictionary<string, string>>(() =>
            {
                var result = new Dictionary<string, string>();
                foreach (var pair in mapping)
                {
                    result[$"{RegexLookbehind}{@namespace}.{pair.NewName}{RegexLookahead}"] = pair.PreviousName;
                    result[$"{RegexLookbehind}{pair.NewName}{RegexLookahead}"] = pair.PreviousName;
                }
                return result;
            });
    }

    public string RemapTypes(string expression)
    {
        foreach (var pair in _typesRemapping.Value)
        {
            expression = Regex.Replace(expression, pair.Key, pair.Value);
        }

        return expression;
    }
}
