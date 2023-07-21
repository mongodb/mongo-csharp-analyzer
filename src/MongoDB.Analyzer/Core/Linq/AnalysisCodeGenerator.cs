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

namespace MongoDB.Analyzer.Core.Linq;

internal static class AnalysisCodeGenerator
{
    private static readonly SyntaxTree[] s_helpersSyntaxTrees;
    private static readonly SyntaxTree s_linqProviderV2SyntaxTree;
    private static readonly SyntaxTree s_linqProviderV3SyntaxTree;
    private static readonly LinqMqlGeneratorTemplateBuilder.SyntaxElements s_mqlGeneratorSyntaxElementsLinq2;
    private static readonly LinqMqlGeneratorTemplateBuilder.SyntaxElements s_mqlGeneratorSyntaxElementsLinq3;
    private static readonly ParseOptions s_parseOptions;

    static AnalysisCodeGenerator()
    {
        s_linqProviderV2SyntaxTree = GetCodeResource(ResourceNames.Linq.IQueryableProviderV2);
        s_linqProviderV3SyntaxTree = GetCodeResource(ResourceNames.Linq.IQueryableProviderV3);
        s_helpersSyntaxTrees = GetCommonCodeResources(ResourceNames.Linq.IQueryableProvider);

        var mqlGeneratorSyntaxTree = GetCodeResource(ResourceNames.Linq.MqlGenerator);
        s_mqlGeneratorSyntaxElementsLinq2 = LinqMqlGeneratorTemplateBuilder.CreateSyntaxElements(mqlGeneratorSyntaxTree, false);
        s_mqlGeneratorSyntaxElementsLinq3 = LinqMqlGeneratorTemplateBuilder.CreateSyntaxElements(mqlGeneratorSyntaxTree, true);
        s_parseOptions = mqlGeneratorSyntaxTree.Options;
    }

    public static CompilationResult Compile(MongoAnalysisContext context, ExpressionsAnalysis linqExpressionAnalysis)
    {
        var semanticModel = context.SemanticModelAnalysisContext.SemanticModel;
        var referencesContainer = ReferencesProvider.GetReferences(semanticModel.Compilation.References, context.Logger);
        if (referencesContainer == null)
        {
            return CompilationResult.Failure;
        }

        var isLinq3 = referencesContainer.Version >= LinqAnalysisConstants.MinLinq3Version;
        var linqProviderSyntaxTree = isLinq3 ? s_linqProviderV3SyntaxTree : s_linqProviderV2SyntaxTree;

        var typesSyntaxTree = TypesGeneratorHelper.GenerateTypesSyntaxTree(AnalysisType.Linq, linqExpressionAnalysis.TypesDeclarations, s_parseOptions);
        var mqlGeneratorSyntaxTree = GenerateMqlGeneratorSyntaxTree(linqExpressionAnalysis, isLinq3);

        var syntaxTrees = new List<SyntaxTree>(s_helpersSyntaxTrees)
            {
                linqProviderSyntaxTree,
                typesSyntaxTree,
                mqlGeneratorSyntaxTree
            };

        var linqTestCodeExecutor = AnalysisCodeGeneratorUtilities.GetCodeExecutor<LinqMqlGeneratorExecutor>(context, AnalysisType.Linq, syntaxTrees.ToArray());

        var result = new CompilationResult(
            linqTestCodeExecutor != null,
            linqTestCodeExecutor,
            referencesContainer.Version);

        return result;
    }

    private static SyntaxTree GenerateMqlGeneratorSyntaxTree(ExpressionsAnalysis linqExpressionAnalysis, bool isLinq3)
    {
        var syntaxElements = isLinq3 ? s_mqlGeneratorSyntaxElementsLinq3 : s_mqlGeneratorSyntaxElementsLinq2;
        var testCodeBuilder = new LinqMqlGeneratorTemplateBuilder(syntaxElements);

        foreach (var linqContext in linqExpressionAnalysis.AnalysisNodeContexts)
        {
            var analysisNode = linqContext.Node;

            linqContext.EvaluationMethodName = testCodeBuilder.AddLinqExpression(
                analysisNode.ArgumentTypeName,
                analysisNode.RewrittenExpression);
        }

        return testCodeBuilder.GenerateSyntaxTree();
    }
}
