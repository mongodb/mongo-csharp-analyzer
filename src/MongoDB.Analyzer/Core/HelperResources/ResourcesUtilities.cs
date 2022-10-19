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

internal static class ResourcesUtilities
{
    private static readonly string[] s_commonResources = new[]
    {
        ResourceNames.EmptyCursor,
        ResourceNames.MongoCollectionMock,
        ResourceNames.MongoClientMock,
        ResourceNames.MongoDatabaseMock
    };

    private static readonly CSharpParseOptions s_parseOptions = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.CSharp8);

    public static SyntaxTree GetCodeResource(string csharpCodeResourceName) =>
        CSharpSyntaxTree.ParseText(GetStringResource(csharpCodeResourceName), s_parseOptions);

    public static SyntaxTree[] GetCommonCodeResources(params string[] additionalResourceNames) =>
        s_commonResources
            .Concat(additionalResourceNames)
            .Select(GetCodeResource)
            .ToArray();

    private static string GetStringResource(string resourceName)
    {
        var fullResourceName = $"{ResourceNames.HelperResourcesBasePath}.{resourceName}.cs";
        using var resourceStream = typeof(ResourcesUtilities).Assembly.GetManifestResourceStream(fullResourceName);
        using var streamReader = new StreamReader(resourceStream);

        return streamReader.ReadToEnd();
    }
}
