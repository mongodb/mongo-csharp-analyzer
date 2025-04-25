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

using System.Collections.Immutable;
using System.IO;
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
        Common.PocoAnalysisVerbosity jsonAnalyzerVerbosity)
    {
        var testDataModelAssembly = PathUtilities.GetTestDataModelAssemblyPath();

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

        var compilationOptions = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);
        var compilation = CSharpCompilation.Create(
            "TestAssembly",
            [testCodeSyntaxTree],
            metadataReferences,
            compilationOptions);

        var mongodbAnalyzer = new MongoDBDiagnosticAnalyzer();
        jsonAnalyzerVerbosity = jsonAnalyzerVerbosity == Common.PocoAnalysisVerbosity.Undefined ? Common.PocoAnalysisVerbosity.None : jsonAnalyzerVerbosity;

        var settings = new MongoDBAnalyzerSettings(
            OutputDriverVersion: true,
            SendTelemetry: false,
            PocoAnalysisVerbosity: (PocoAnalysisVerbosity)jsonAnalyzerVerbosity);
        var analyzerOptions = new AnalyzerOptions(ImmutableArray.Create<AdditionalText>(new AdditionalTextAnalyzerSettings(settings)));

        var compilationWithAnalyzers = compilation.WithAnalyzers(ImmutableArray.Create<DiagnosticAnalyzer>(mongodbAnalyzer), analyzerOptions);
        var diagnostics = await compilationWithAnalyzers.GetAllDiagnosticsAsync();

        return diagnostics;
    }
}
