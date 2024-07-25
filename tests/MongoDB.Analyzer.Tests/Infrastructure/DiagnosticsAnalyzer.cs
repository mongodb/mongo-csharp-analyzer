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

using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;
using MongoDB.Analyzer.Core;

namespace MongoDB.Analyzer.Tests.Infrastructure;

internal static class DiagnosticsAnalyzer
{
    public static async Task<ImmutableArray<Diagnostic>> Analyze(
        string testCodeFilename,
        string driverVersion,
        Common.LinqVersion linqVersion,
        Common.PocoAnalysisVerbosity jsonAnalyzerVerbosity)
    {
        var isDriverVersion_2_28_Or_Greater = PathUtilities.IsDriverVersion_2_28_Or_Greater(driverVersion);
        var testDataModelAssembly = isDriverVersion_2_28_Or_Greater ? PathUtilities.TestDataModelAssemblyPathDRIVER_2_28_OR_Greater : PathUtilities.TestDataModelAssemblyPathDRIVER_2_27_OR_Lower;
        PathUtilities.VerifyTestDataModelAssembly(testDataModelAssembly);

#if NET472
        var netReferences = ReferenceAssemblies.NetFramework.Net472.Default.AddAssemblies(ImmutableArray.Create("System.Drawing"));
#elif NETCOREAPP2_1
            var netReferences = ReferenceAssemblies.NetCore.NetCoreApp21;
#elif NETCOREAPP3_1
        var netReferences = ReferenceAssemblies.NetCore.NetCoreApp31;
#endif

        var packages = ImmutableArray.Create(new PackageIdentity("MongoDB.Driver", driverVersion),
            new PackageIdentity("Microsoft.EntityFrameworkCore", "3.1.0"));

        var allReferences = netReferences
            .AddPackages(packages)
            .AddAssemblies(ImmutableArray.Create(testDataModelAssembly))
            .WithNuGetConfigFilePath(PathUtilities.NugetConfigPath);

        var metadataReferences = await allReferences.ResolveAsync(LanguageNames.CSharp, default);

        var testCodeText = File.ReadAllText(testCodeFilename);
        var testCodeSyntaxTree = CSharpSyntaxTree.ParseText(testCodeText);

        var publicKey = new byte[160] { 0, 36, 0, 0, 4, 128, 0, 0, 148, 0, 0, 0, 6, 2, 0, 0, 0, 36, 0, 0, 82, 83, 65, 49, 0, 4, 0, 0, 1, 0, 1, 0, 53, 40, 127, 13, 56, 131, 192, 160, 117, 200, 142, 12, 218, 60, 233, 59, 98, 16, 3, 236, 189, 94, 146, 13, 74, 140, 114, 56, 86, 79, 77, 47, 79, 104, 17, 106, 202, 40, 201, 178, 19, 65, 220, 58, 135, 118, 121, 193, 69, 86, 25, 43, 43, 47, 95, 226, 193, 29, 98, 78, 8, 148, 211, 8, 255, 123, 148, 191, 111, 215, 42, 239, 27, 65, 1, 127, 254, 37, 114, 233, 144, 25, 209, 198, 25, 99, 230, 140, 208, 237, 103, 115, 74, 66, 203, 51, 59, 128, 142, 56, 103, 203, 230, 49, 147, 114, 20, 227, 46, 64, 159, 177, 250, 98, 253, 182, 157, 73, 76, 37, 48, 230, 74, 64, 228, 23, 214, 238 };
        var compilationOptions = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, publicSign: true, cryptoPublicKey: publicKey.ToImmutableArray());
        var assemblyName = isDriverVersion_2_28_Or_Greater ? "DynamicProxyGenAssembly2" : "TestAssembly";
        var compilation = CSharpCompilation.Create(assemblyName, new[] { testCodeSyntaxTree }, metadataReferences, compilationOptions);

        var mongodbAnalyzer = new MongoDBDiagnosticAnalyzer();
        var linqDefaultVersion = linqVersion == Common.LinqVersion.Undefined ? null : (LinqVersion?)linqVersion;
        jsonAnalyzerVerbosity = jsonAnalyzerVerbosity == Common.PocoAnalysisVerbosity.Undefined ? Common.PocoAnalysisVerbosity.None : jsonAnalyzerVerbosity;

        var settings = new MongoDBAnalyzerSettings(
            OutputDriverVersion: true,
            DefaultLinqVersion: linqDefaultVersion,
            SendTelemetry: false,
            PocoAnalysisVerbosity: (PocoAnalysisVerbosity)jsonAnalyzerVerbosity);
        var analyzerOptions = new AnalyzerOptions(ImmutableArray.Create<AdditionalText>(new AdditionalTextAnalyzerSettings(settings)));

        var compilationWithAnalyzers = compilation.WithAnalyzers(ImmutableArray.Create<DiagnosticAnalyzer>(mongodbAnalyzer), analyzerOptions);

        var test = PrintAssemblyVersion(compilation, "MongoDB.Bson");
        var test2 = PrintSpecificReferencedAssemblyVersion($"{testDataModelAssembly}.dll", "MongoDB.Bson");
        var test3 = PrintSpecificReferencedAssemblyVersion($"{testDataModelAssembly}.dll", "MongoDB.Driver");

        var diagnostics = await compilationWithAnalyzers.GetAllDiagnosticsAsync();

        return diagnostics;
    }

    public static string PrintAssemblyVersion(CSharpCompilation compilation, string assemblyName)
    {
        var assemblyReference = compilation.References
        .OfType<PortableExecutableReference>()
        .FirstOrDefault(r => Path.GetFileNameWithoutExtension(r.FilePath).Equals(assemblyName, StringComparison.OrdinalIgnoreCase));

        if (assemblyReference != null)
        {
            // Read the assembly from the file path
            using var stream = new FileStream(assemblyReference.FilePath, FileMode.Open, FileAccess.Read);
            var reader = new PEReader(stream);
            var metadataReader = reader.GetMetadataReader();
            var assemblyDefinition = metadataReader.GetAssemblyDefinition();
            var version = assemblyDefinition.Version;

            return $"Assembly: {assemblyName}, Version: {version}";
        }
        else
        {
            return $"Assembly: {assemblyName} not found in the compilation references.";
        }
    }

    public static string PrintAssemblyVersion2(string dllPath)
    {
        try
        {
            var assembly = Assembly.LoadFrom(dllPath);
            var version = assembly.GetName().Version;
            return $"Assembly version: {version}";
        }
        catch (Exception ex)
        {
            return $"Failed to load assembly: {ex.Message}";
        }
    }

    public static string PrintSpecificReferencedAssemblyVersion(string dllPath, string targetAssemblyName)
    {
        try
        {
            using var stream = new FileStream(dllPath, FileMode.Open, FileAccess.Read);
            var peReader = new PEReader(stream);
            var metadataReader = peReader.GetMetadataReader();

            foreach (var reference in metadataReader.AssemblyReferences)
            {
                var assemblyReference = metadataReader.GetAssemblyReference(reference);
                var name = metadataReader.GetString(assemblyReference.Name);

                // Check if the name matches the target assembly
                if (name.Equals(targetAssemblyName, StringComparison.OrdinalIgnoreCase))
                {
                    var version = assemblyReference.Version;
                    return $"Assembly: {name}, Version: {version}";
                }
            }

            return $"Assembly: {targetAssemblyName} not found in the references.";
        }
        catch (Exception ex)
        {
            return $"Failed to read assembly: {ex.Message}";
        }
    }
}
