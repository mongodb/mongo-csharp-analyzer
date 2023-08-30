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
        if (context.Settings.PocoAnalysisVerbosity == PocoAnalysisVerbosity.None ||
            context.Settings.PocoLimit <= 0)
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
            if (analysisContexts.Count == context.Settings.PocoLimit)
            {
                break;
            }

            try
            {
                var classSymbol = semanticModel.GetDeclaredSymbol(classNode);
                if (PreanalyzeClassDeclaration(context, classSymbol))
                {
                    var generatedClassName = typesProcessor.ProcessTypeSymbol(classSymbol);
                    var generatedClassNode = (ClassDeclarationSyntax)(typesProcessor.GetTypeSymbolToMemberDeclarationMapping(classSymbol));
                    var expressionContext = new ExpressionAnalysisContext(new ExpressionAnalysisNode(classNode, null, generatedClassNode, null, classNode.GetLocation()));
                    analysisContexts.Add(expressionContext);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed analyzing {classNode.NormalizeWhitespace()} with {ex.Message}");
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

    private static bool ContainsBsonAttributes(MongoAnalysisContext context, INamedTypeSymbol classSymbol) =>
        classSymbol.GetAttributes().Where(attribute => IsValidBsonAttribute(context, attribute)).AnySafe();

    private static bool ContainsPropertiesWithBsonAttributes(MongoAnalysisContext context, INamedTypeSymbol classSymbol) =>
        classSymbol.GetMembers().OfType<IPropertySymbol>().SelectMany(property => property.GetAttributes().Where(attribute => IsValidBsonAttribute(context, attribute))).AnySafe();

    private static bool ContainsFieldsWithBsonAttributes(MongoAnalysisContext context, INamedTypeSymbol classSymbol) =>
        classSymbol.GetMembers().OfType<IFieldSymbol>().SelectMany(field => field.GetAttributes().Where(attribute => IsValidBsonAttribute(context, attribute))).AnySafe();

    private static bool IsValidBsonAttribute(MongoAnalysisContext context, AttributeData attribute) =>
        attribute.AttributeClass.IsSupportedBsonAttribute() || context.TypesProcessor.GetTypeSymbolToMemberDeclarationMapping(attribute.AttributeClass) != null;

    private static bool PreanalyzeClassDeclaration(MongoAnalysisContext context, INamedTypeSymbol classSymbol) =>
        context.Settings.PocoAnalysisVerbosity == PocoAnalysisVerbosity.All ||
        (classSymbol != null &&
        (ContainsBsonAttributes(context, classSymbol) ||
        ContainsPropertiesWithBsonAttributes(context, classSymbol) ||
        ContainsFieldsWithBsonAttributes(context, classSymbol)));
}
