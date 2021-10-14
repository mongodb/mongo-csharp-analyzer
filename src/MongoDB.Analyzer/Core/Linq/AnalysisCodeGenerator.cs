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

namespace MongoDB.Analyzer.Core.Linq;

internal static class AnalysisCodeGenerator
{
    private static readonly SyntaxTree s_mqlGeneratorSyntaxTree;
    private static readonly SyntaxTree s_linqProviderV2SyntaxTree;
    private static readonly SyntaxTree s_linqProviderV3SyntaxTree;
    private static readonly SyntaxTree[] s_helpersSyntaxTrees;

    static AnalysisCodeGenerator()
    {
        s_linqProviderV2SyntaxTree = ResourcesUtilities.GetCodeResource(ResourceNames.IQueryableProviderV2);
        s_linqProviderV3SyntaxTree = ResourcesUtilities.GetCodeResource(ResourceNames.IQueryableProviderV3);
        s_mqlGeneratorSyntaxTree = ResourcesUtilities.GetCodeResource(ResourceNames.MqlGenerator);

        s_helpersSyntaxTrees = new SyntaxTree[]
        {
            ResourcesUtilities.GetCodeResource(ResourceNames.IQueryableProvider),
            ResourcesUtilities.GetCodeResource(ResourceNames.EmptyCursor),
            ResourcesUtilities.GetCodeResource(ResourceNames.MongoCollectionMock)
        };
    }

    public static CompilationResult Compile(MongoAnalyzerContext context, ExpressionsAnalysis linqExpressionAnalysis)
    {
        var semanticModel = context.SemanticModelAnalysisContext.SemanticModel;
        var referencesContainer = ReferencesProvider.GetReferences(semanticModel.Compilation.References, context.Logger);
        if (referencesContainer == null)
        {
            return CompilationResult.Failure;
        }

        var isLinq3 = referencesContainer.Version >= LinqAnalysisConstants.MinLinq3Version;
        var linqProviderSyntaxTree = isLinq3 ? s_linqProviderV3SyntaxTree : s_linqProviderV2SyntaxTree;

        var typesSyntaxTree = GenerateTypesSyntaxTree(linqExpressionAnalysis);
        var mqlGeneratorSyntaxTree = GenerateMqlGeneratorSyntaxTree(linqExpressionAnalysis, isLinq3);

        var syntaxTrees = new List<SyntaxTree>(s_helpersSyntaxTrees)
            {
                linqProviderSyntaxTree,
                typesSyntaxTree,
                mqlGeneratorSyntaxTree
            };

        var compilation = CSharpCompilation.Create(
            LinqAnalysisConstants.AnalysisAssemblyName,
            syntaxTrees,
            referencesContainer.References,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        using var memoryStream = new MemoryStream();
        var emitResult = compilation.Emit(memoryStream);

        LinqMqlGeneratorExecutor linqTestCodeExecutor = null;

        if (emitResult.Success)
        {
            context.Logger.Log("Compilation successful");

            memoryStream.Seek(0, SeekOrigin.Begin);

            var mqlGeneratorType = DynamicTypeProvider.GetType(referencesContainer, memoryStream, MqlGeneratorSyntaxElements.MqlGeneratorFullName);

            linqTestCodeExecutor = mqlGeneratorType != null ?
                new LinqMqlGeneratorExecutor(mqlGeneratorType, isLinq3 ? LinqVersion.V3 : LinqVersion.V2, context.Settings.DefaultLinqVersion) : null;
        }
        else
        {
            context.Logger.Log($"Compilation failed with: {string.Join(Environment.NewLine, emitResult.Diagnostics)}");
        }

        var result = new CompilationResult(
            linqTestCodeExecutor != null,
            linqTestCodeExecutor,
            referencesContainer.Version);

        return result;
    }

    private static SyntaxTree GenerateMqlGeneratorSyntaxTree(ExpressionsAnalysis linqExpressionAnalysis, bool isLinq3)
    {
        var testCodeBuilder = new LinqMqlGeneratorTemplateBuilder(s_mqlGeneratorSyntaxTree, isLinq3);

        foreach (var linqContext in linqExpressionAnalysis.AnalysisNodeContexts)
        {
            var analysisNode = linqContext.Node;

            linqContext.EvaluationMethodName = testCodeBuilder.AddLinqExpression(
                analysisNode.ArgumentTypeName,
                analysisNode.RewrittenExpression);
        }

        return testCodeBuilder.GenerateSyntaxTree();
    }

    private static SyntaxTree GenerateTypesSyntaxTree(ExpressionsAnalysis linqExpressionAnalysis)
    {
        var namespaceDeclarationSyntax = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.ParseName(MqlGeneratorSyntaxElements.MqlGeneratorNamespace))
            .AddMembers(linqExpressionAnalysis.TypesDeclarations);

        var generatedTypesCompilationUnit = SyntaxFactory.CompilationUnit()
            .AddUsings(SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System")),
                SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("MongoDB.Bson")),
                SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("MongoDB.Bson.Serialization.Attributes")),
                 SyntaxFactory.UsingDirective(
                    SyntaxFactory.NameEquals(SyntaxFactory.IdentifierName("BsonTypeCustom123")),
                    SyntaxFactory.ParseName("MongoDB.Bson.BsonType")),
                SyntaxFactory.UsingDirective(
                    SyntaxFactory.NameEquals(SyntaxFactory.IdentifierName("BsonDocumentCustom123")),
                    SyntaxFactory.ParseName("MongoDB.Bson.BsonDocument")),
                SyntaxFactory.UsingDirective(
                    SyntaxFactory.NameEquals(SyntaxFactory.IdentifierName("BsonValueCustom123")),
                    SyntaxFactory.ParseName("MongoDB.Bson.BsonValue")),
                SyntaxFactory.UsingDirective(
                    SyntaxFactory.NameEquals(SyntaxFactory.IdentifierName("BsonObjectIdCustom123")),
                    SyntaxFactory.ParseName("MongoDB.Bson.BsonObjectId")))
            .AddMembers(namespaceDeclarationSyntax);

        var syntaxTree = generatedTypesCompilationUnit.SyntaxTree
            .WithRootAndOptions(generatedTypesCompilationUnit.SyntaxTree.GetRoot(), s_mqlGeneratorSyntaxTree.Options);

        return syntaxTree;
    }
}
