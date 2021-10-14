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

namespace MongoDB.Analyzer.Tests.Infrastructure;

internal static class PathUtilities
{
    private static readonly string s_testCasesBaseNamespace = "MongoDB.Analyzer.Tests.Common.TestCases";
    private static readonly string s_testCasesBaseFolder = s_testCasesBaseNamespace;
    private static readonly string s_projectParentFolderPrefix = @"..\..\..\..\";
    private static readonly string s_testCasesPath = GetFullPathRelativeToParent(s_testCasesBaseFolder);

    public static string TestDataModelAssemblyPath { get; } = GetFullPathRelativeToParent(@"MongoDB.Analyzer.Tests.Common.ClassLibrary\bin\Debug\netstandard2.0\MongoDB.Analyzer.Tests.Common.ClassLibrary");

    public static string GetTestCaseFileFullPathFromName(string testCaseFullyQualifiedName)
    {
        var pathNameFromTypeName = testCaseFullyQualifiedName
            .Replace(s_testCasesBaseNamespace, string.Empty)
            .Trim('.')
            .Replace('.', '\\');

        var result = Path.Combine(s_testCasesPath, pathNameFromTypeName + ".cs");
        return result;
    }

    public static void VerifyTestDataModelAssembly()
    {
        var fileName = $"{TestDataModelAssemblyPath}.dll";
        if (!File.Exists(fileName))
        {
            throw new FileNotFoundException($"DataModel assembly {fileName} not found", fileName);
        }
    }


    private static string GetFullPathRelativeToParent(string partialPath) =>
        Path.GetFullPath(s_projectParentFolderPrefix + partialPath);
}
