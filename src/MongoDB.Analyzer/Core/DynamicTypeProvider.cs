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

internal static class DynamicTypeProvider
{
    private static readonly ConcurrentDictionary<string, string> s_pathsMappings = new();

    static DynamicTypeProvider()
    {
        AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolve;
    }

    private static Assembly AssemblyResolve(object sender, ResolveEventArgs args)
    {
        var assemblyName = new AssemblyName(args.Name);
        if (s_pathsMappings.TryGetValue($"{assemblyName.Name}_{assemblyName.Version}", out var assemblyPath))
        {
            return Assembly.LoadFile(assemblyPath);
        }
        return null;
    }

    public static Type GetType(
        ReferencesContainer referencesContainer,
        MemoryStream assemblyMemoryStream,
        string typeName)
    {
        Type result;
        try
        {
            foreach (var driverPath in referencesContainer.DriverPaths)
            {
                s_pathsMappings.GetOrAdd($"{driverPath.Key}_{referencesContainer.Version}", driverPath.Value);
            }

            var assembly = Assembly.Load(assemblyMemoryStream.ToArray());
            result = assembly.GetType(typeName);
        }
        catch
        {
            result = null;
        }

        return result;
    }
}
