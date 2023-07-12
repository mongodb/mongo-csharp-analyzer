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

using static MongoDB.Analyzer.Core.HelperResources.JsonSyntaxElements.Json;
using static MongoDB.Analyzer.Core.HelperResources.ResourcesUtilities;

namespace MongoDB.Analyzer.Core.Json;

internal static class AnalysisCodeGenerator
{
    private static readonly SyntaxTree[] s_helpersSyntaxTrees;
    private static readonly JsonGeneratorTemplateBuilder.SyntaxElements s_jsonGeneratorSyntaxElements;
    private static readonly ParseOptions s_parseOptions;


    static AnalysisCodeGenerator()
    {
        s_helpersSyntaxTrees = GetCommonCodeResources();
        var jsonGeneratorSyntaxTree = GetCodeResource(ResourceNames.Json.JsonGenerator);
        s_jsonGeneratorSyntaxElements = JsonGeneratorTemplateBuilder.CreateSyntaxElements(jsonGeneratorSyntaxTree);
        s_parseOptions = jsonGeneratorSyntaxTree.Options;
    }

    public static CompilationResult Compile(MongoAnalysisContext context, JsonExpressionAnalysis jsonExpressionAnalysis)
    {
        var semanticModel = context.SemanticModelAnalysisContext.SemanticModel;
        var referencesContainer = ReferencesProvider.GetReferences(semanticModel.Compilation.References, context.Logger);
        if (referencesContainer == null)
        {
            return CompilationResult.Failure;
        }

        var typesSyntaxTree = TypesGeneratorHelper.GenerateTypesSyntaxTree(AnalysisType.Json, jsonExpressionAnalysis.TypesDeclarations, s_parseOptions);
        var jsonGeneratorSyntaxTree = GenerateJsonGeneratorSyntaxTree(jsonExpressionAnalysis);

        var syntaxTrees = new List<SyntaxTree>(s_helpersSyntaxTrees)
            {
                typesSyntaxTree,
                jsonGeneratorSyntaxTree
            };

        var compilation = CSharpCompilation.Create(
            JsonAnalysisConstants.AnalysisAssemblyName,
            syntaxTrees,
            referencesContainer.References,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        using var memoryStream = new MemoryStream();
        var emitResult = compilation.Emit(memoryStream);

        JsonGeneratorExecutor jsonCodeExecutor = null;

        if (emitResult.Success)
        {
            context.Logger.Log("Compilation successful");

            memoryStream.Seek(0, SeekOrigin.Begin);

            var jsonGeneratorType = DynamicTypeProvider.GetType(referencesContainer, memoryStream, JsonGeneratorFullName);

            jsonCodeExecutor = jsonGeneratorType != null ? new JsonGeneratorExecutor(jsonGeneratorType) : null;
        }
        else
        {
            context.Logger.Log($"Compilation failed with: {string.Join(Environment.NewLine, emitResult.Diagnostics)}");
        }

        var result = new CompilationResult(
            jsonCodeExecutor != null,
            jsonCodeExecutor,
            referencesContainer.Version);

        return result;
    }

    public static SyntaxTree GenerateJsonGeneratorSyntaxTree(JsonExpressionAnalysis jsonExpressionAnalysis)
    {
        var testCodeBuilder = new JsonGeneratorTemplateBuilder(s_jsonGeneratorSyntaxElements);

        foreach (var jsonContext in jsonExpressionAnalysis.AnalysisNodeContexts)
        {
            var analysisNode = jsonContext.Node;
            jsonContext.EvaluationMethodName = testCodeBuilder.AddPOCO(analysisNode.RewrittenPOCO);
        }

        return testCodeBuilder.GenerateSyntaxTree();
    }
}