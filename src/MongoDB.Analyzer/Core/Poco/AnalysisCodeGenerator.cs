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

namespace MongoDB.Analyzer.Core.Poco;

internal static class AnalysisCodeGenerator
{
    private static readonly PocoJsonGeneratorTemplateBuilder.SyntaxElements s_jsonGeneratorSyntaxElements;
    private static readonly ParseOptions s_parseOptions;

    static AnalysisCodeGenerator()
    {
        var jsonGeneratorSyntaxTree = GetCodeResource(ResourceNames.Poco.JsonGenerator);
        s_jsonGeneratorSyntaxElements = PocoJsonGeneratorTemplateBuilder.CreateSyntaxElements(jsonGeneratorSyntaxTree);
        s_parseOptions = jsonGeneratorSyntaxTree.Options;
    }

    public static CompilationResult Compile(MongoAnalysisContext context, ExpressionsAnalysis pocoExpressionAnalysis)
    {
        var semanticModel = context.SemanticModelAnalysisContext.SemanticModel;
        var referencesContainer = ReferencesProvider.GetReferences(semanticModel.Compilation.References, context.Logger);
        if (referencesContainer == null)
        {
            return CompilationResult.Failure;
        }

        var typesSyntaxTree = TypesGeneratorHelper.GenerateTypesSyntaxTree(AnalysisType.Poco, pocoExpressionAnalysis.TypesDeclarations, s_parseOptions);
        var jsonGeneratorSyntaxTree = GenerateJsonGeneratorSyntaxTree(pocoExpressionAnalysis);

        var syntaxTrees = new[] { typesSyntaxTree, jsonGeneratorSyntaxTree };

        var generatorType = AnalysisCodeGeneratorUtilities.CompileAndGetGeneratorType(AnalysisType.Poco, context, referencesContainer, syntaxTrees);
        if (generatorType == null)
        {
            return CompilationResult.Failure;
        }

        var result = new CompilationResult(
            true,
            new PocoJsonGeneratorExecutor(generatorType),
            referencesContainer.Version);

        return result;
    }

    public static SyntaxTree GenerateJsonGeneratorSyntaxTree(ExpressionsAnalysis pocoExpressionAnalysis)
    {
        var testCodeBuilder = new PocoJsonGeneratorTemplateBuilder(s_jsonGeneratorSyntaxElements);

        foreach (var pocoContext in pocoExpressionAnalysis.AnalysisNodeContexts)
        {
            pocoContext.EvaluationMethodName = testCodeBuilder.AddPoco(pocoContext.Node.RewrittenExpression as ClassDeclarationSyntax);
        }

        return testCodeBuilder.GenerateSyntaxTree();
    }
}
