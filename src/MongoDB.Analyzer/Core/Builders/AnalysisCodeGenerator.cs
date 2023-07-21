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

namespace MongoDB.Analyzer.Core.Builders;

internal static class AnalysisCodeGenerator
{
    private static readonly SyntaxTree[] s_helpersSyntaxTrees;
    private static readonly SyntaxTree s_renderer_2_19_and_higher;
    private static readonly BuildersMqlGeneratorTemplateBuilder.SyntaxElements s_mqlGeneratorSyntaxElements;
    private static readonly ParseOptions s_parseOptions;

    static AnalysisCodeGenerator()
    {
        s_helpersSyntaxTrees = GetCommonCodeResources(ResourceNames.Builders.Renderer);
        s_renderer_2_19_and_higher = GetCodeResource(ResourceNames.Builders.Renderer_2_19_and_higher);

        var mqlGeneratorSyntaxTree = GetCodeResource(ResourceNames.Builders.MqlGenerator);
        s_mqlGeneratorSyntaxElements = BuildersMqlGeneratorTemplateBuilder.CreateSyntaxElements(mqlGeneratorSyntaxTree);
        s_parseOptions = mqlGeneratorSyntaxTree.Options;
    }

    public static CompilationResult Compile(MongoAnalysisContext context, ExpressionsAnalysis buildersExpressionAnalysis)
    {
        var semanticModel = context.SemanticModelAnalysisContext.SemanticModel;
        var referencesContainer = ReferencesProvider.GetReferences(semanticModel.Compilation.References, context.Logger);
        if (referencesContainer == null)
        {
            return CompilationResult.Failure;
        }

        var typesSyntaxTree = TypesGeneratorHelper.GenerateTypesSyntaxTree(AnalysisType.Builders, buildersExpressionAnalysis.TypesDeclarations, s_parseOptions);
        var mqlGeneratorSyntaxTree = GenerateMqlGeneratorSyntaxTree(buildersExpressionAnalysis);

        var syntaxTrees = new List<SyntaxTree>(s_helpersSyntaxTrees)
            {
                typesSyntaxTree,
                mqlGeneratorSyntaxTree
            };

        if (referencesContainer.Version >= BuildersAnalysisConstants.Version_2_19_and_higher)
        {
            syntaxTrees.Add(s_renderer_2_19_and_higher);
        }

        var buildersMqlCodeExecutor = AnalysisCodeGeneratorUtilities.GetCodeExecutor<BuildersMqlGeneratorExecutor>(context, AnalysisType.Builders, syntaxTrees.ToArray());

        var result = new CompilationResult(
            buildersMqlCodeExecutor != null,
            buildersMqlCodeExecutor,
            referencesContainer.Version);

        return result;
    }

    private static SyntaxTree GenerateMqlGeneratorSyntaxTree(ExpressionsAnalysis builderExpressionAnalysis)
    {
        var testCodeBuilder = new BuildersMqlGeneratorTemplateBuilder(s_mqlGeneratorSyntaxElements);

        foreach (var builderContext in builderExpressionAnalysis.AnalysisNodeContexts)
        {
            var analysisNode = builderContext.Node;

            builderContext.EvaluationMethodName = testCodeBuilder.AddBuildersExpression(
                analysisNode.ArgumentTypeName,
                analysisNode.RewrittenExpression);
        }

        return testCodeBuilder.GenerateSyntaxTree();
    }
}
