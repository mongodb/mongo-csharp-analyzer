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

internal static class ReferencesProvider
{
    private const string Netstandard20 = "netstandard2.0";
    private const string NetstandardDll = "netstandard.dll";

    private static readonly string[] s_additionalDependencies = new[] { "System.Runtime.dll" };

    private static readonly HashSet<string> s_mongodbDriverAssemblies = new(new[]
    {
        "MongoDB.Bson.dll",
        "MongoDB.Driver.Core.dll",
        "MongoDB.Driver.dll"
    });

    private static readonly ConcurrentDictionary<Version, IDictionary<string, string>> s_mongodbAssemblyPaths = new();

    public static Version GetMongoDBDriverVersion(IEnumerable<MetadataReference> metadataReferences)
    {
        var mongoDBreference = GetMongoDBDriverReferences(metadataReferences).FirstOrDefault();

        if (mongoDBreference != null)
        {
            var assemblyName = AssemblyName.GetAssemblyName(mongoDBreference.FilePath);
            return assemblyName?.Version;
        }

        return null;
    }

    public static ReferencesContainer GetReferences(IEnumerable<MetadataReference> metadataReferences, Logger logger)
    {
        var resultReferences = new List<MetadataReference>();

        var netStandardPath = GetNetStandardPath(metadataReferences);
        if (string.IsNullOrWhiteSpace(netStandardPath))
        {
            logger.Log("Failed to find netStandard assembly");
            return null;
        }
        resultReferences.Add(MetadataReference.CreateFromFile(netStandardPath));

        var mongoDBReferences = GetMongoDBDriverReferences(metadataReferences);
        if (!mongoDBReferences.Any())
        {
            logger.Log("Failed to find mongodb driver references");
            return null;
        }

        var (version, nameToPathMappings, missingAssemblyPath) = GetNetstandard20DriverAssemblyLocation(mongoDBReferences);
        if (version == null)
        {
            var referencesFilenames = string.Join(",", mongoDBReferences.Select(r => r?.FilePath));

            logger.Log($"Failed to find: {missingAssemblyPath}");
            logger.Log($"MongoDB driver references: {referencesFilenames}");
            return null;
        }
        else
        {
            logger.Log($"Found Netstandard20 driver location for {version}");
        }

        foreach (var mapping in nameToPathMappings)
        {
            resultReferences.Add(MetadataReference.CreateFromFile(mapping.Value));
        }

        resultReferences.Add(MetadataReference.CreateFromFile(typeof(object).Assembly.Location));
        resultReferences.Add(MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location));
        resultReferences.Add(MetadataReference.CreateFromFile(typeof(IEnumerator<int>).Assembly.Location));
        resultReferences.Add(MetadataReference.CreateFromFile(typeof(Queryable).Assembly.Location));
        resultReferences.Add(MetadataReference.CreateFromFile(typeof(System.Dynamic.DynamicObject).Assembly.Location));
        resultReferences.Add(MetadataReference.CreateFromFile(typeof(System.Runtime.AssemblyTargetedPatchBandAttribute).Assembly.Location));
        resultReferences.Add(MetadataReference.CreateFromFile(typeof(Task).Assembly.Location));
        resultReferences.Add(MetadataReference.CreateFromFile(typeof(Unsafe).Assembly.Location));

        // Try adding additional assemblies for Rider support
        var baseDir = Path.GetDirectoryName(typeof(object).Assembly.Location);

        foreach (var dependency in s_additionalDependencies)
        {
            TryAddingAssembly(dependency);
        }

        void TryAddingAssembly(string assemblyName)
        {
            var assemblyPath = Path.Combine(baseDir, assemblyName);
            if (File.Exists(Path.Combine(assemblyPath)))
            {
                resultReferences.Add(MetadataReference.CreateFromFile(assemblyPath));
            }
        }

        logger.Log($"Found references for {version} driver version");
        return new ReferencesContainer(resultReferences.ToArray(), nameToPathMappings, version);
    }

    private static PortableExecutableReference[] GetMongoDBDriverReferences(IEnumerable<MetadataReference> metadataReferences) =>
        metadataReferences
        .OfType<PortableExecutableReference>()
        .Where(r => s_mongodbDriverAssemblies.Contains(Path.GetFileName(r.FilePath)))
        .ToArray();

    private static string GetNetStandardPath(IEnumerable<MetadataReference> metadataReferences)
    {
        var netStandardDllPath = Path.Combine(Path.GetFullPath(Path.Combine(typeof(object).Assembly.Location, "..\\")), NetstandardDll);
        if (File.Exists(netStandardDllPath))
        {
            return netStandardDllPath;
        }
        else
        {
            return metadataReferences
                .OfType<PortableExecutableReference>()
                .FirstOrDefault(r => r.FilePath.Contains(NetstandardDll))?
                .FilePath;
        }
    }

    private static (Version, IDictionary<string, string>, string MissingAssembly) GetNetstandard20DriverAssemblyLocation(PortableExecutableReference[] driverReferences)
    {
        IDictionary<string, string> nameToPathMapping = null;
        Version version = null;

        foreach (var reference in driverReferences)
        {
            var assemblyName = Path.GetFileName(reference.FilePath);
            var libPath = Path.GetFullPath(Path.GetDirectoryName(Path.GetDirectoryName(reference.FilePath)));

            var assemblyPath = Path.Combine(libPath, Netstandard20, assemblyName);

            if (File.Exists(assemblyPath))
            {
                var assemblyFullName = AssemblyName.GetAssemblyName(assemblyPath);
                version = assemblyFullName.Version;

                if (nameToPathMapping == null &&
                    s_mongodbAssemblyPaths.TryGetValue(assemblyFullName.Version, out nameToPathMapping))
                {
                    return (version, nameToPathMapping, null);
                }
                else
                {
                    nameToPathMapping ??= new Dictionary<string, string>();
                    nameToPathMapping.Add(assemblyFullName.Name, assemblyPath);
                }
            }
            else
            {
                return (null, null, assemblyPath);
            }
        }

        s_mongodbAssemblyPaths.AddOrUpdate(version, nameToPathMapping, (_, _) => nameToPathMapping);

        return (version, nameToPathMapping, null);
    }

    public static string GetDriverVersion(Assembly assembly) =>
        assembly.GetReferencedAssemblies().FirstOrDefault(a => a.Name.Contains("MongoDB"))?.Version?.ToString();
}
