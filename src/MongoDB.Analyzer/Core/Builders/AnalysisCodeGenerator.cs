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
using MongoDB.Analyzer.Core.Utilities;
using static MongoDB.Analyzer.Core.HelperResources.ResourcesUtilities;

namespace MongoDB.Analyzer.Core.Builders;

internal static class AnalysisCodeGenerator
{
    private static readonly BuildersMqlGeneratorTemplateBuilder.SyntaxElements s_mqlGeneratorSyntaxElements;
    private static readonly CSharpParseOptions s_parseOptions;
    private static readonly SyntaxTreesCache s_syntaxTreesCache;

    static AnalysisCodeGenerator()
    {
        var mqlGeneratorSyntaxTree = GetCodeResource(ResourceNames.Builders.MqlGenerator);
        s_mqlGeneratorSyntaxElements = BuildersMqlGeneratorTemplateBuilder.CreateSyntaxElements(mqlGeneratorSyntaxTree);

        s_parseOptions = (CSharpParseOptions)mqlGeneratorSyntaxTree.Options;
        s_syntaxTreesCache = new SyntaxTreesCache(s_parseOptions, ResourceNames.Builders.Renderer);
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

        var helperSyntaxTrees = s_syntaxTreesCache.GetSyntaxTrees(referencesContainer.Version);
        var syntaxTrees = new List<SyntaxTree>(helperSyntaxTrees)
            {
                typesSyntaxTree,
                mqlGeneratorSyntaxTree
            };

        var generatorType = AnalysisCodeGeneratorUtilities.CompileAndGetGeneratorType(AnalysisType.Builders, context, referencesContainer, syntaxTrees);
        if (generatorType == null)
        {
            return CompilationResult.Failure;
        }

        var result = new CompilationResult(
            true,
            new BuildersMqlGeneratorExecutor(generatorType),
            referencesContainer.Version);

        return result;
    }

    private static SyntaxTree GenerateMqlGeneratorSyntaxTree(ExpressionsAnalysis builderExpressionAnalysis)
    {
        var testCodeBuilder = new BuildersMqlGeneratorTemplateBuilder(s_mqlGeneratorSyntaxElements);
        var generatedMqlMethodDeclarations = new List<MethodDeclarationSyntax>();

        foreach (var builderContext in builderExpressionAnalysis.AnalysisNodeContexts)
        {
            var (generatedMqlMethodName, generatedMqlMethodDeclaration) = testCodeBuilder.GenerateMqlGeneratorMethod(builderContext);
            builderContext.EvaluationMethodName = generatedMqlMethodName;
            generatedMqlMethodDeclarations.Add(generatedMqlMethodDeclaration);
        }

        testCodeBuilder.AddMqlGeneratorMethods(generatedMqlMethodDeclarations.ToArray());
        return testCodeBuilder.GenerateSyntaxTree();
    } 
}
