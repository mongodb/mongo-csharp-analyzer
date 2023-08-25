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

namespace MongoDB.Analyzer.Core.HelperResources;
using static MongoDB.Analyzer.Core.HelperResources.ResourcesUtilities;

internal sealed class SyntaxTreesCache
{
    private readonly ConcurrentDictionary<Version, SyntaxTree[]> _cache;
    private readonly SyntaxTree[] _commonSyntaxTrees;
    private readonly CSharpParseOptions _parseOptions;
    private readonly string[] _versionedResourcesNames;

    private static readonly Version s_minVersion = Version.Parse("2.12");

    public SyntaxTreesCache(
        CSharpParseOptions parseOptions,
        params string[] versionedResourcesNames)
    {
        _parseOptions = parseOptions;
        _versionedResourcesNames = versionedResourcesNames;

        _cache = new ConcurrentDictionary<Version, SyntaxTree[]>();
        _commonSyntaxTrees = GetCommonCodeResources();
    }

    public SyntaxTree[] GetSyntaxTrees(Version driverVersion)
    {
        if (!_cache.TryGetValue(driverVersion, out var result))
        {
            var driverVersionsOrGreater = Enumerable
                  .Range(s_minVersion.Minor, driverVersion.Minor - s_minVersion.Minor + 1)
                  .Select(minor => $"DRIVER_{driverVersion.Major}_{minor}_OR_GREATER").ToArray();

            var parseOptions = _parseOptions.WithPreprocessorSymbols(driverVersionsOrGreater);
            var versionedSyntaxTrees = GetVersionedCodeResources(parseOptions, _versionedResourcesNames);

            var commonSyntaxTrees = _commonSyntaxTrees.Concat(versionedSyntaxTrees).ToArray();
            result = _cache.GetOrAdd(driverVersion, commonSyntaxTrees);
        }

        return result;
    }
}
