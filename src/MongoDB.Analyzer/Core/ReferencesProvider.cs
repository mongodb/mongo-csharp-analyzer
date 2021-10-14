using Microsoft.CodeAnalysis;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MongoDB.Analyzer.Core
{
    internal static class ReferencesProvider
    {
        private const string Netstandard20 = "netstandard2.0";
        private const string NetstandardDll = "netstandard.dll";

        private static HashSet<string> s_mongodbDriverAssemblies = new HashSet<string>(new[]
            {
                "MongoDB.Bson.dll",
                "MongoDB.Driver.Core.dll",
                "MongoDB.Driver.dll"
            });

        private static readonly ConcurrentDictionary<string, string> s_mongodbPackagePaths = new ConcurrentDictionary<string, string>();

        static ReferencesProvider()
        {
            AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolve;
        }

        private static Assembly AssemblyResolve(object sender, ResolveEventArgs args)
        {
            if (args.Name.Contains("MongoDB"))
            {
                var assemblyName = new AssemblyName(args.Name);
                var pathKey = $"{assemblyName.Name}_{assemblyName.Version}";

                if (s_mongodbPackagePaths.TryGetValue(pathKey, out var path))
                {
                    return Assembly.LoadFrom(path);
                }
            }
            return null;
        }

        public static MetadataReference[] GetMetadataReferences(IEnumerable<MetadataReference> metadataReferencesSource)
        {
            var result = new List<MetadataReference>();

            var netStandardPath = GetNetStandardPath();
            if (string.IsNullOrWhiteSpace(netStandardPath))
            {
                return null;
            }
            result.Add(MetadataReference.CreateFromFile(netStandardPath));

            var mongoDBreferences = metadataReferencesSource.
                 OfType<PortableExecutableReference>().
                 Where(r => s_mongodbDriverAssemblies.Contains(Path.GetFileName(r.FilePath))).
                 ToArray();
            if (!mongoDBreferences.Any())
            {
                return null;
            }

            foreach (var assemblyName in s_mongodbDriverAssemblies)
            {
                var assemblyPath = GetMongoDBDriverNetFrameworFullPath(assemblyName);

                if (string.IsNullOrWhiteSpace(assemblyPath))
                {
                    return null;
                }

                result.Add(MetadataReference.CreateFromFile(assemblyPath, MetadataReferenceProperties.Assembly));
            }

            result.Add(MetadataReference.CreateFromFile(typeof(object).Assembly.Location));
            result.Add(MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location));
            result.Add(MetadataReference.CreateFromFile(typeof(IEnumerator<int>).Assembly.Location));
            result.Add(MetadataReference.CreateFromFile(typeof(Queryable).Assembly.Location));
            result.Add(MetadataReference.CreateFromFile(typeof(System.Dynamic.DynamicObject).Assembly.Location));
            result.Add(MetadataReference.CreateFromFile(typeof(System.Runtime.AssemblyTargetedPatchBandAttribute).Assembly.Location));            
            result.Add(MetadataReference.CreateFromFile(typeof(Task).Assembly.Location));

            string GetNetStandardPath()
            {
                var netStandardDllPath = Path.Combine(Path.GetFullPath(Path.Combine(typeof(object).Assembly.Location, "..\\")), NetstandardDll);
                if (File.Exists(netStandardDllPath))
                {
                    return netStandardDllPath;
                }
                else
                {
                    return metadataReferencesSource.
                        OfType<PortableExecutableReference>().
                        Where(r => r.FilePath.Contains(NetstandardDll)).
                        FirstOrDefault().FilePath;
                }
            }

            string GetMongoDBDriverNetFrameworFullPath(string assemblyName)
            {
                var reference = mongoDBreferences.FirstOrDefault(r => r.FilePath.Contains(assemblyName));
                var libPath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(reference.FilePath), "..\\"));

                var assemblyPath = Path.Combine(libPath, Netstandard20, assemblyName);
                if (File.Exists(assemblyPath))
                {
                    var assemblyNameFull = AssemblyName.GetAssemblyName(assemblyPath);
                    var pathKey = $"{assemblyNameFull.Name}_{assemblyNameFull.Version}";
                    s_mongodbPackagePaths.AddOrUpdate(pathKey, assemblyPath, (_, _) => assemblyPath);

                    return assemblyPath;
                }

                return null;
            }

            return result.ToArray();
        }

        public static string GetDriverVersion(Assembly assembly) =>
            assembly.GetReferencedAssemblies().FirstOrDefault(a => a.Name.Contains("MongoDB"))?.Version?.ToString();
    }
}
