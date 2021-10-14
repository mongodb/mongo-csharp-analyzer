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

namespace MongoDB.Analyzer.Core.Linq;

internal static class LinqMqlGeneratorProvider
{
    private static readonly ConcurrentDictionary<string, string> s_pathsMappings = new();

    static LinqMqlGeneratorProvider()
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

    public static LinqMqlGeneratorExecutor GetLinqMqlGeneratorExecutor(
        ReferencesContainer referencesContainer,
        MemoryStream assemblyMemoryStream,
        LinqVersion driverLinqVersion,
        LinqVersion defaultLinqVersion)
    {
        LinqMqlGeneratorExecutor result;
        try
        {
            foreach (var driverPath in referencesContainer.DriverPaths)
            {
                s_pathsMappings.GetOrAdd($"{driverPath.Key}_{referencesContainer.Version}", driverPath.Value);
            }

            var assembly = Assembly.Load(assemblyMemoryStream.ToArray());
            var testClassType = assembly.GetType(MqlGeneratorSyntaxElements.MqlGeneratorFullName);
            result = testClassType != null ? new LinqMqlGeneratorExecutor(testClassType, driverLinqVersion, defaultLinqVersion) : null;
        }
        catch
        {
            result = null;
        }

        return result;
    }
}
