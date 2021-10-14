using System.IO;

namespace MongoDB.Analyzer.Tests.Helpers
{
    internal static class PathUtils
    {
        public static string ProjectParentFolderPrefix { get; } = @"..\..\..\..\";
        public static string TestCasesPath { get; } = GetFullPathRelativeToParent("MongoDB.Analyzer.Tests.Common.TestCases");
        public static string TestDataModelAssemblyPathCasesPath { get; } = GetFullPathRelativeToParent(@"MongoDB.Analyzer.Tests.Common.ClassLibrary\bin\Debug\netstandard2.0\MongoDB.Analyzer.Tests.Common.ClassLibrary");

        public static string GetTestCaseFileFullPath(string testCaseFileName) =>
            Path.Combine(TestCasesPath, testCaseFileName);

        public static string GetFullPathRelativeToParent(string partialPath) =>
            Path.GetFullPath(ProjectParentFolderPrefix + partialPath);
    }
}
