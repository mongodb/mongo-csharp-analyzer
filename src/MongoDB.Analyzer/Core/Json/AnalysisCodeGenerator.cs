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

using MongoDB.Analyzer.Core.Utilities;
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

    public static CompilationResult Compile(MongoAnalysisContext context, ExpressionsAnalysis jsonExpressionAnalysis)
    {
        var semanticModel = context.SemanticModelAnalysisContext.SemanticModel;
        var referencesContainer = ReferencesProvider.GetReferences(semanticModel.Compilation.References, context.Logger, AnalysisType.Poco);
        if (referencesContainer == null)
        {
            return CompilationResult.Failure;
        }

        var typesSyntaxTree = TypesGeneratorHelper.GenerateTypesSyntaxTree(AnalysisType.Poco, jsonExpressionAnalysis.TypesDeclarations, s_parseOptions);
        var jsonGeneratorSyntaxTree = GenerateJsonGeneratorSyntaxTree(jsonExpressionAnalysis);

        var syntaxTrees = new List<SyntaxTree>(s_helpersSyntaxTrees)
            {
                typesSyntaxTree,
                jsonGeneratorSyntaxTree
            };

        var jsonCodeExecutor = AnalysisCodeGeneratorUtilities.GetCodeExecutor<JsonGeneratorExecutor>(context, referencesContainer, syntaxTrees.ToArray(), AnalysisType.Poco);

        var result = new CompilationResult(
            jsonCodeExecutor != null,
            jsonCodeExecutor,
            referencesContainer.Version);

        return result;
    }

    public static SyntaxTree GenerateJsonGeneratorSyntaxTree(ExpressionsAnalysis jsonExpressionAnalysis)
    {
        var testCodeBuilder = new JsonGeneratorTemplateBuilder(s_jsonGeneratorSyntaxElements);

        foreach (var jsonContext in jsonExpressionAnalysis.AnalysisNodeContexts)
        {
            jsonContext.EvaluationMethodName = testCodeBuilder.AddPoco(jsonContext.Node.RewrittenExpression as ClassDeclarationSyntax);
        }

        return testCodeBuilder.GenerateSyntaxTree();
    }
}