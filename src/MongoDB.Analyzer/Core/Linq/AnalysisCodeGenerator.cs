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
    private static readonly SyntaxTree[] s_commonResources;
    private static readonly MqlGeneratorTestMethodTemplate s_testMethodTemplate;
    private static readonly CSharpParseOptions s_parseOptions;

    static AnalysisCodeGenerator()
    {
        var mqlGeneratorSyntaxTree = GetCodeResource(ResourceNames.Linq.MqlGenerator);
        s_testMethodTemplate = LinqMqlGeneratorTemplateBuilder.CreateTestMethodTemplate(mqlGeneratorSyntaxTree);

        s_parseOptions = (CSharpParseOptions)mqlGeneratorSyntaxTree.Options;
        s_commonResources = GetCommonCodeResources();
    }

    public static CompilationResult Compile(MongoAnalysisContext context, ExpressionsAnalysis linqExpressionAnalysis)
    {
        var semanticModel = context.SemanticModelAnalysisContext.SemanticModel;
        var referencesContainer = ReferencesProvider.GetReferences(semanticModel.Compilation.References, context.Logger);
        if (referencesContainer == null)
        {
            return CompilationResult.Failure;
        }

        var typesSyntaxTree = TypesGeneratorHelper.GenerateTypesSyntaxTree(AnalysisType.Linq, linqExpressionAnalysis.TypesDeclarations, s_parseOptions);
        var mqlGeneratorSyntaxTree = GenerateMqlGeneratorSyntaxTree(linqExpressionAnalysis);

        var syntaxTrees = new List<SyntaxTree>(s_commonResources)
            {
                typesSyntaxTree,
                mqlGeneratorSyntaxTree
            };

        var generatorType = AnalysisCodeGeneratorUtilities.CompileAndGetGeneratorType(AnalysisType.Linq, context, referencesContainer, syntaxTrees);
        if (generatorType == null)
        {
            return CompilationResult.Failure;
        }

        var result = new CompilationResult(
            true,
            new LinqMqlGeneratorExecutor(generatorType),
            referencesContainer.Version);

        return result;
    }

    private static SyntaxTree GenerateMqlGeneratorSyntaxTree(ExpressionsAnalysis linqExpressionAnalysis)
    {
        var testCodeBuilder = new LinqMqlGeneratorTemplateBuilder(s_testMethodTemplate);
        var generatedMqlMethodDeclarations = new List<MethodDeclarationSyntax>(linqExpressionAnalysis.AnalysisNodeContexts.Length);

        foreach (var linqContext in linqExpressionAnalysis.AnalysisNodeContexts)
        {
            var analysisNode = linqContext.Node;
            var (generatedMqlMethodName, generatedMqlMethodDeclaration) = testCodeBuilder.GenerateMqlGeneratorMethod(
                analysisNode.ArgumentTypeName,
                analysisNode.RewrittenExpression);

            linqContext.EvaluationMethodName = generatedMqlMethodName;
            generatedMqlMethodDeclarations.Add(generatedMqlMethodDeclaration);
        }

        testCodeBuilder.AddMqlGeneratorMethods(generatedMqlMethodDeclarations.ToArray());
        return testCodeBuilder.GenerateSyntaxTree();
    }
}
