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

namespace MongoDB.Analyzer.Core.Builders;

internal static class AnalysisCodeGenerator
{
    private static readonly SyntaxTree s_mqlGeneratorSyntaxTree;

    static AnalysisCodeGenerator()
    {
        s_mqlGeneratorSyntaxTree = ResourcesUtilities.GetCodeResource(ResourceNames.MqlGenerator);
    }

    public static CompilationResult Compile(MongoAnalyzerContext context, ExpressionsAnalysis buildersExpressionAnalysis)
    {
        var semanticModel = context.SemanticModelAnalysisContext.SemanticModel;
        var referencesContainer = ReferencesProvider.GetReferences(semanticModel.Compilation.References, context.Logger);
        if (referencesContainer == null)
        {
            return CompilationResult.Failure;
        }

        var typesSyntaxTree = GenerateTypesSyntaxTree(buildersExpressionAnalysis);
        var mqlGeneratorSyntaxTree = GenerateMqlGeneratorSyntaxTree(buildersExpressionAnalysis);

        var compilation = CSharpCompilation.Create(
            BuildersAnalysisConstants.AnalysisAssemblyName,
            new[] { typesSyntaxTree, mqlGeneratorSyntaxTree },
            referencesContainer.References,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        using var memoryStream = new MemoryStream();
        var emitResult = compilation.Emit(memoryStream);

        BuildersMqlGeneratorExecutor buildersMqlCodeExecutor = null;

        if (emitResult.Success)
        {
            context.Logger.Log("Compilation successful");

            memoryStream.Seek(0, SeekOrigin.Begin);

            var mqlGeneratorType = DynamicTypeProvider.GetType(referencesContainer, memoryStream, MqlGeneratorSyntaxElements.MqlGeneratorFullName);

            buildersMqlCodeExecutor = mqlGeneratorType != null ? new BuildersMqlGeneratorExecutor(mqlGeneratorType) : null;
        }
        else
        {
            context.Logger.Log($"Compilation failed with: {string.Join(Environment.NewLine, emitResult.Diagnostics)}");
        }

        var result = new CompilationResult(
            buildersMqlCodeExecutor != null,
            buildersMqlCodeExecutor,
            referencesContainer.Version);

        return result;
    }

    private static SyntaxTree GenerateMqlGeneratorSyntaxTree(ExpressionsAnalysis builderExpressionAnalysis)
    {
        var testCodeBuilder = new BuildersMqlGeneratorTemplateBuilder(s_mqlGeneratorSyntaxTree);

        foreach (var linqContext in builderExpressionAnalysis.AnalysisNodeContexts)
        {
            var analysisNode = linqContext.Node;

            linqContext.EvaluationMethodName = testCodeBuilder.AddBuildersExpression(
                analysisNode.ArgumentTypeName,
                analysisNode.RewrittenExpression);
        }

        return testCodeBuilder.GenerateSyntaxTree();
    }

    private static SyntaxTree GenerateTypesSyntaxTree(ExpressionsAnalysis buildersdExpressionAnalysis)
    {
        var namespaceDeclarationSyntax = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.ParseName(MqlGeneratorSyntaxElements.MqlGeneratorNamespace))
            .AddMembers(buildersdExpressionAnalysis.TypesDeclarations);

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
