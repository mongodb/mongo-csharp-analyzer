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

    private IDictionary<string, string> _typesRemapping;

    public TypesMapper(string mqlGeneratorNamespace, TypesProcessor typesProcessor)
    {
        AddMappings(mqlGeneratorNamespace, typesProcessor);
    }

    public string RemapTypes(string expression)
    {
        if (_typesRemapping != null)
        {
            foreach (var pair in _typesRemapping)
            {
                expression = Regex.Replace(expression, pair.Key, pair.Value);
            }
        }

        return expression;
    }

    private void AddMappings(string sourceNamespace, TypesProcessor typesProcessor)
    {
        if (_typesRemapping == null)
        {
            _typesRemapping = new Dictionary<string, string>();
        }

        foreach (var pair in typesProcessor.GeneratedTypeToOriginalTypeMapping)
        {
            _typesRemapping[$"{RegexLookbehind}{sourceNamespace}.{pair.NewName}{RegexLookahead}"] = pair.PreviousName;
            _typesRemapping[$"{RegexLookbehind}{pair.NewName}{RegexLookahead}"] = pair.PreviousName;
        }
    }
}