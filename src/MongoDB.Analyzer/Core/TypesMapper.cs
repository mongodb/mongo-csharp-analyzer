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

    private Lazy<(Regex NewNameMatcher, string PreviousName)[]> _typesRemapping;

    public TypesMapper(string @namespace, IEnumerable<(string NewName, string PreviousName)> mapping)
    {
        _typesRemapping = new(() => mapping
            .Select(pair => (GetRegex(pair.NewName), pair.PreviousName))
            .ToArray());

        Regex GetRegex(string name) => new($"{RegexLookbehind}{@namespace}.{name}{RegexLookahead}", RegexOptions.Compiled);
    }

    public string RemapTypes(string expression)
    {
        foreach (var (newNameMatcher, previousName) in _typesRemapping.Value)
        {
            expression = newNameMatcher.Replace(expression, previousName);
        }

        return expression;
    }
}
