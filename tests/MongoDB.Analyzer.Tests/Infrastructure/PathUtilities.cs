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

using System.IO;
using MongoDB.Analyzer.Tests.Common;
using NuGet.Versioning;

namespace MongoDB.Analyzer.Tests.Infrastructure;

internal static class PathUtilities
{
    private static readonly string s_testCasesBaseNamespace = "MongoDB.Analyzer.Tests.Common.TestCases";
    private static readonly string s_testCasesBaseFolder = s_testCasesBaseNamespace;
    private static readonly string s_projectParentFolderPrefix = Path.Combine("..", "..", "..", "..");
    private static readonly string s_testCasesPath = GetFullPathRelativeToParent(s_testCasesBaseFolder);

    public static string TestDataModelAssemblyPathDriver_2_27_OrLower { get; } = GetFullPathRelativeToParent("MongoDB.Analyzer.Tests.Common.ClassLibrary", "bin", "DRIVER_2_27_OR_LOWER", "netstandard2.0", "MongoDB.Analyzer.Tests.Common.ClassLibrary");
    public static string TestDataModelAssemblyPathDriver_2_28_OrGreater { get; } = GetFullPathRelativeToParent("MongoDB.Analyzer.Tests.Common.ClassLibrary", "bin", "DRIVER_2_28_OR_GREATER", "netstandard2.0", "MongoDB.Analyzer.Tests.Common.ClassLibrary");
    public static string NugetConfigPath { get; } = GetFullPathRelativeToParent("..", "nuget.config");

    public static string GetTestCaseFileFullPathFromName(string testCaseFullyQualifiedName)
    {
        var pathNameFromTypeName = testCaseFullyQualifiedName
            .Replace(s_testCasesBaseNamespace, string.Empty)
            .Trim('.')
            .Replace('.', Path.DirectorySeparatorChar);

        var result = Path.Combine(s_testCasesPath, pathNameFromTypeName + ".cs");
        return result;
    }

    public static bool IsDriverVersion_2_28_OrGreater(string driverVersion) => VersionRange.Parse(DriverVersions.V2_28_OrGreater).Satisfies(NuGetVersion.Parse(driverVersion));

    public static void VerifyTestDataModelAssembly(string testDataModelAssembly)
    {
        if (!File.Exists($"{testDataModelAssembly}.dll"))
        {
            throw new FileNotFoundException($"DataModel assembly {testDataModelAssembly} not found", testDataModelAssembly);
        }
    }

    private static string GetFullPathRelativeToParent(params string[] pathComponents) =>
        Path.GetFullPath(Path.Combine(s_projectParentFolderPrefix, pathComponents.Length == 1 ? pathComponents[0] : Path.Combine(pathComponents)));
}
