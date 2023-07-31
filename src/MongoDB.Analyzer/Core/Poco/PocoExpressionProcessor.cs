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

namespace MongoDB.Analyzer.Core.Poco;

internal static class PocoExpressionProcessor
{
    public static ExpressionsAnalysis ProcessSemanticModel(MongoAnalysisContext context)
    {
        if (context.Settings.JsonAnalyzerVerbosity == JsonAnalyzerVerbosity.None)
        {
            return default;
        }

        var semanticModel = context.SemanticModelAnalysisContext.SemanticModel;
        var syntaxTree = semanticModel.SyntaxTree;
        var root = syntaxTree.GetRoot();

        var analysisContexts = new List<ExpressionAnalysisContext>();
        var invalidExpressionNodes = new List<InvalidExpressionAnalysisNode>();
        var classNodes = root.DescendantNodesAndSelf().OfType<ClassDeclarationSyntax>();

        var typesProcessor = context.TypesProcessor;

        foreach (var classNode in classNodes)
        {
            if (PreanalyzeClassDeclaration(classNode, context, analysisContexts))
            {
                try
                {
                    var classSymbol = semanticModel.GetDeclaredSymbol(classNode);
                    var generatedClassName = typesProcessor.ProcessTypeSymbol(classSymbol);
                    var generatedClassNode = (ClassDeclarationSyntax)(typesProcessor.GetTypeSymbolToMemberDeclarationMapping(classSymbol));
                    var expresionContext = new ExpressionAnalysisContext(new ExpressionAnalysisNode(classNode, null, generatedClassNode, null, classNode.GetLocation()));
                    analysisContexts.Add(expresionContext);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Failed analyzing {classNode.NormalizeWhitespace()} with {ex.Message}");
                }
            }
        }

        var pocoAnalysis = new ExpressionsAnalysis()
        {
            AnalysisNodeContexts = analysisContexts.ToArray(),
            InvalidExpressionNodes = invalidExpressionNodes.ToArray(),
            TypesDeclarations = typesProcessor.TypesDeclarations
        };

        context.Logger.Log($"JSON: Found {pocoAnalysis.AnalysisNodeContexts.Length} expressions.");
        return pocoAnalysis;
    }

    private static bool PreanalyzeClassDeclaration(ClassDeclarationSyntax classDeclarationSyntax, MongoAnalysisContext context, List<ExpressionAnalysisContext> analysisContexts)
    {
        if (analysisContexts.Count == context.Settings.PocoLimit)
        {
            return false;
        }

        if (context.Settings.JsonAnalyzerVerbosity == JsonAnalyzerVerbosity.All)
        {
            return true;
        }

        var classSymbol = context.SemanticModelAnalysisContext.SemanticModel.GetDeclaredSymbol(classDeclarationSyntax);

        var classBsonAttributes = classSymbol.GetAttributes()
            .Where(attribute => attribute.AttributeClass.IsSupportedBsonAttribute() ||
            context.TypesProcessor.GetTypeSymbolToMemberDeclarationMapping(attribute.AttributeClass) != null);

        var propertyBsonAttributes = classSymbol.GetMembers().OfType<IPropertySymbol>()
            .SelectMany(property => property.GetAttributes().Where(attribute => attribute.AttributeClass.IsSupportedBsonAttribute() ||
            context.TypesProcessor.GetTypeSymbolToMemberDeclarationMapping(attribute.AttributeClass) != null));

        var fieldBsonAttributes = classSymbol.GetMembers().OfType<IFieldSymbol>()
            .SelectMany(field => field.GetAttributes().Where(attribute => attribute.AttributeClass.IsSupportedBsonAttribute() ||
            context.TypesProcessor.GetTypeSymbolToMemberDeclarationMapping(attribute.AttributeClass) != null));

        return classBsonAttributes.AnySafe() || propertyBsonAttributes.AnySafe() || fieldBsonAttributes.AnySafe();
    }
}