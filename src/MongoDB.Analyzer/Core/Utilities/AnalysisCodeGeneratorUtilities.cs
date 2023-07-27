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

using MongoDB.Analyzer.Core.HelperResources;

namespace MongoDB.Analyzer.Core.Utilities;

internal static class AnalysisCodeGeneratorUtilities
{
    private const string AnalysisAssemblyName = "DynamicProxyGenAssembly2";

    public static Type CompileAndGetGeneratorType(MongoAnalysisContext context, ReferencesContainer referencesContainer, SyntaxTree[] syntaxTrees, AnalysisType analysisType)
    {
        var compilation = CSharpCompilation.Create(
            AnalysisAssemblyName,
            syntaxTrees,
            referencesContainer.References,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        using var memoryStream = new MemoryStream();
        var emitResult = compilation.Emit(memoryStream);
        Type generatorType = null;

        if (emitResult.Success)
        {
            context.Logger.Log("Compilation successful");

            memoryStream.Seek(0, SeekOrigin.Begin);

            generatorType = DynamicTypeProvider.GetType(referencesContainer, memoryStream, GetGeneratorFullName(analysisType));
        }
        else
        {
            context.Logger.Log($"Compilation failed with: {string.Join(Environment.NewLine, emitResult.Diagnostics)}");
        }

        return generatorType;
    }

    private static string GetGeneratorFullName(AnalysisType analysisType) =>
        analysisType switch
        {
            AnalysisType.Builders => MqlGeneratorSyntaxElements.Builders.MqlGeneratorFullName,
            AnalysisType.Linq => MqlGeneratorSyntaxElements.Linq.MqlGeneratorFullName,
            AnalysisType.Poco => JsonSyntaxElements.Poco.JsonGeneratorFullName,
            _ => throw new ArgumentOutOfRangeException(nameof(analysisType), "Unsupported analysis type")
        };
}

