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
using static MongoDB.Analyzer.Core.ExpressionProcessor;
using TypeInfo = Microsoft.CodeAnalysis.TypeInfo;

namespace MongoDB.Analyzer.Core;

internal static class ExpressionProcessor
{
    public record RewriteContext(
        AnalysisType AnalysisType,
        SyntaxNode Expression,
        IEnumerable<SyntaxNode> RootNodes,
        SemanticModel SemanticModel,
        TypesProcessor TypesProcessor,
        ConstantsMapper ConstantsMapper)
    {
        public static RewriteContext Builders(SyntaxNode Expression, IEnumerable<SyntaxNode> RootNodes, SemanticModel SemanticModel, TypesProcessor TypesProcessor) =>
            new(AnalysisType.Builders, Expression, RootNodes, SemanticModel, TypesProcessor, new());

        public static RewriteContext Linq(SyntaxNode Expression, SyntaxNode RootNode, SemanticModel SemanticModel, TypesProcessor TypesProcessor) =>
            new(AnalysisType.Linq, Expression, new SyntaxNode[] { RootNode }, SemanticModel, TypesProcessor, new());
    }

    private enum RewriteAction
    {
        Unknown,
        Rewrite,
        Ignore,
        Invalid
    }

    private record RewriteResult(
        RewriteAction RewriteAction,
        SyntaxNode NodeToReplace,
        SyntaxNode NewNode)
    {
        public RewriteResult(SyntaxNode NodeToReplace, SyntaxNode NewNode) :
            this(RewriteAction.Rewrite, NodeToReplace, NewNode)
        {
        }

        public static RewriteResult Ignore { get; } = new(RewriteAction.Ignore, null, null);
        public static RewriteResult Invalid { get; } = new(RewriteAction.Invalid, null, null);
    }

    private static void RewriteRootNodes(RewriteContext rewriteContext, HashSet<SyntaxNode> nodesProcessed, Dictionary<SyntaxNode, SyntaxNode> nodesRemapping)
    {
        var expressionNode = rewriteContext.Expression;
        var rootNodes = rewriteContext.RootNodes;
        var typesProcessor = rewriteContext.TypesProcessor;

        switch (rewriteContext.AnalysisType)
        {
            case AnalysisType.Builders:
                {
                    foreach (var rootNode in rootNodes)
                    {
                        var rootType = rewriteContext.SemanticModel.GetTypeInfo(rootNode).Type as INamedTypeSymbol;
                        if (rootType.IsSupportedIMongoCollection())
                        {
                            nodesProcessed.Add(rootNode);
                            nodesRemapping.Add(rootNode, SyntaxFactory.IdentifierName(MqlGeneratorSyntaxElements.Builders.CollectionName));
                        }
                        else if (rootType.IsBuilder())
                        {
                            var rootTypeArguments = rootType.TypeArguments.ToArray();

                            var typeArguments = rootTypeArguments.Select(rootTypeArgument => SyntaxFactory.ParseTypeName(typesProcessor
                                .GetTypeSymbolToGeneratedTypeMapping(rootTypeArgument))).ToArray();

                            var buildersGenericType = SyntaxFactory.GenericName("Builders").WithTypeArgumentList(
                                SyntaxFactory.TypeArgumentList(SyntaxFactory.SeparatedList(typeArguments)));

                            var buildersDefinitionNode = SyntaxFactory.MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                buildersGenericType,
                                SyntaxFactory.IdentifierName(rootType.GetBuilderDefinitionName()));

                            nodesProcessed.Add(rootNode);
                            nodesRemapping.Add(rootNode, buildersDefinitionNode);
                        }
                    }

                    break;
                }
            case AnalysisType.Linq:
                {
                    foreach (var rootNode in rootNodes)
                    {
                        nodesProcessed.Add(rootNode);
                        nodesRemapping.Add(rootNode, SyntaxFactory.IdentifierName(MqlGeneratorSyntaxElements.Linq.QueryableName));
                    }
                    
                    break;
                }
            default:
                throw new ArgumentOutOfRangeException(nameof(rewriteContext.AnalysisType), rewriteContext.AnalysisType, "Unsupported analysis type");
        }
    }

    private static void RewriteIdentifiers(RewriteContext rewriteContext, HashSet<SyntaxNode> nodesProcessed, Dictionary<SyntaxNode, SyntaxNode> nodesRemapping)
    {
        var expressionNode = rewriteContext.Expression;
        var rootNodes = rewriteContext.RootNodes;
        var typesProcessor = rewriteContext.TypesProcessor;

        // Set analysis specific parameters
        var processGenerics = false;
        var removeFluentParameters = false;
        IdentifierNameSyntax[] lambdaAndQueryIdentifiers = null;
        SimpleNameSyntax[] expressionDescendants;

        switch (rewriteContext.AnalysisType)
        {
            case AnalysisType.Builders:
                {
                    expressionDescendants = expressionNode.DescendantNodesWithSkipList<SimpleNameSyntax>(nodesProcessed).ToArray();
                    processGenerics = true;
                    removeFluentParameters = true;
                    break;
                }
            case AnalysisType.Linq:
                {
                    lambdaAndQueryIdentifiers = expressionNode
                      .DescendantNodes(n => !rootNodes.Contains(n))
                      .OfType<IdentifierNameSyntax>()
                      .Where(identifierNode =>
                      {
                          var symbolInfo = rewriteContext.SemanticModel.GetSymbolInfo(identifierNode);
                          return symbolInfo.Symbol != null && IsChildOfLambdaOrQueryOrBuildersParameter(rewriteContext, identifierNode, symbolInfo);
                      })
                      .ToArray();


                    //Ignoring generics, process only IdentifierNameSyntax
                    expressionDescendants = expressionNode.DescendantNodes(n => !rootNodes.Contains(n)).OfType<IdentifierNameSyntax>().ToArray();
                    processGenerics = false;
                    break;
                }
            default:
                throw new ArgumentOutOfRangeException(nameof(rewriteContext.AnalysisType), rewriteContext.AnalysisType, "Unsupported analysis type");
        }

        foreach (var identifierNode in expressionDescendants)
        {
            if (rootNodes.Contains(identifierNode) ||
                !identifierNode.IsLeaf() ||
                nodesProcessed.Any(e => e.Contains(identifierNode)))
            {
                continue;
            }

            var nodeToHandle = SyntaxNodeExtensions.GetTopMostInvocationOrBinaryExpressionSyntax(identifierNode, lambdaAndQueryIdentifiers);

            if (nodeToHandle != identifierNode)
            {
                nodesProcessed.Add(nodeToHandle);
            }

            var symbolInfo = rewriteContext.SemanticModel.GetSymbolInfo(nodeToHandle);
            if (symbolInfo.Symbol == null)
            {
                nodesRemapping.Clear();
                return;
            }

            var typeInfo = rewriteContext.SemanticModel.GetTypeInfo(nodeToHandle);
            var rewriteResult = removeFluentParameters ? RemoveNonFluentParameters(nodeToHandle, symbolInfo, typeInfo) : null;

            if (rewriteResult == null)
            {
                rewriteResult = symbolInfo.Symbol.Kind switch
                {
                    SymbolKind.Field => HandleField(rewriteContext, nodeToHandle, symbolInfo, typeInfo),
                    SymbolKind.Method => HandleMethod(rewriteContext, nodeToHandle, symbolInfo, typeInfo),
                    SymbolKind.NamedType => HandleRemappedType(rewriteContext, identifierNode, typeInfo, processGenerics),
                    SymbolKind.Local or
                    SymbolKind.Parameter or
                    SymbolKind.Property => SubstituteExpressionWithConstant(rewriteContext, nodeToHandle, symbolInfo, typeInfo),
                    _ => RewriteResult.Ignore
                };
            }

            switch (rewriteResult.RewriteAction)
            {
                case RewriteAction.Rewrite:
                    {
                        if (rewriteResult.NodeToReplace != nodeToHandle)
                        {
                            nodesProcessed.Add(rewriteResult.NodeToReplace);
                        }

                        nodesRemapping[rewriteResult.NodeToReplace] = rewriteResult.NewNode;
                        break;
                    }
                case RewriteAction.Invalid:
                    {
                        nodesRemapping.Clear();
                        return;
                    }
                default:
                    continue;
            }
        }
    }

    public static (SyntaxNode RewrittenLinqExpression, ConstantsMapper ConstantsMapper) RewriteExpression(RewriteContext rewriteContext)
    {
        var nodesProcessed = new HashSet<SyntaxNode>();
        var nodesRemapping = new Dictionary<SyntaxNode, SyntaxNode>();
        var expressionNode = rewriteContext.Expression;

        // Register literals
        foreach (var literalSyntax in expressionNode.DescendantNodes().OfType<LiteralExpressionSyntax>())
        {
            rewriteContext.ConstantsMapper.RegisterLiteral(literalSyntax);
        }
        rewriteContext.ConstantsMapper.FinalizeLiteralsRegistration();

        //Get Node Remappings for Root Nodes
        RewriteRootNodes(rewriteContext, nodesProcessed, nodesRemapping);

        //Get Node Remappings for Identifiers
        RewriteIdentifiers(rewriteContext, nodesProcessed, nodesRemapping);

        if (nodesRemapping.EmptyOrNull())
        {
            return default;
        }

        var result = expressionNode.ReplaceNodes(nodesRemapping.Keys, (n, _) => nodesRemapping[n]);

        return (result, rewriteContext.ConstantsMapper);
    }

    private static SyntaxNode GetConstantReplacementNode(
       RewriteContext rewriteContext,
       ITypeSymbol typeSymbol,
       string originalNodeFullName = null)
    {
        ExpressionSyntax replacementNode = null;
        var (isNullable, underlingTypeSymbol) = typeSymbol.DiscardNullable();

        if (underlingTypeSymbol.TypeKind == TypeKind.Enum)
        {
            var underlyingEnumType = (underlingTypeSymbol as INamedTypeSymbol).EnumUnderlyingType.SpecialType;
            var literalSyntax = rewriteContext.ConstantsMapper.GetExpressionByType(underlyingEnumType, originalNodeFullName);
            replacementNode = GetEnumCastNode(underlingTypeSymbol, literalSyntax.Token.Value, rewriteContext.TypesProcessor, isNullable);
        }
        else if (underlingTypeSymbol.SpecialType != SpecialType.None)
        {
            replacementNode = rewriteContext.ConstantsMapper.GetExpressionByType(underlingTypeSymbol.SpecialType, originalNodeFullName);

            if (isNullable)
            {
                replacementNode = SyntaxFactory.CastExpression(
                    SyntaxFactoryUtilities.GetNullableType(underlingTypeSymbol.Name),
                    replacementNode);
            }
        }

        return replacementNode;
    }

    private static ExpressionSyntax GetEnumCastNode(
        ITypeSymbol typeSymbol,
        object constantValue,
        TypesProcessor typesProcessor,
        bool isNullable = false)
    {
        var remappedEnumTypeName = typesProcessor.GetTypeSymbolToGeneratedTypeMapping(typeSymbol);

        if (remappedEnumTypeName.IsNullOrWhiteSpace())
        {
            return null;
        }

        return SyntaxFactoryUtilities.GetCastConstantExpression(remappedEnumTypeName, constantValue, isNullable);
    }

    private static RewriteResult HandleField(
       RewriteContext rewriteContext,
       SyntaxNode identifierNode,
       SymbolInfo symbolInfo,
       TypeInfo typeInfo)
    {
        var fieldSymbol = symbolInfo.Symbol as IFieldSymbol;
        if (fieldSymbol?.HasConstantValue != true)
        {
            return SubstituteExpressionWithConstant(rewriteContext, identifierNode, symbolInfo, typeInfo);
        }

        if (!identifierNode.Parent.IsKind(SyntaxKind.SimpleMemberAccessExpression))
        {
            return RewriteResult.Ignore;
        }

        SyntaxNode replacementNode;
        if (fieldSymbol.Type.TypeKind == TypeKind.Enum)
        {
            replacementNode = GetEnumCastNode(fieldSymbol.Type, fieldSymbol.ConstantValue, rewriteContext.TypesProcessor);
        }
        else if (fieldSymbol.Type.SpecialType != SpecialType.None)
        {
            replacementNode = rewriteContext.ConstantsMapper.GetExpressionForConstant(fieldSymbol.Type.SpecialType, fieldSymbol.ConstantValue);
        }
        else
        {
            return RewriteResult.Invalid;
        }

        return new RewriteResult(identifierNode.Parent, replacementNode);
    }

    private static RewriteResult HandleMethod(
        RewriteContext rewriteContext,
        SyntaxNode identifierNode,
        SymbolInfo symbolInfo,
        TypeInfo typeInfo)
    {
        var methodSymbol = symbolInfo.Symbol as IMethodSymbol;
        if (methodSymbol.ReturnType == null)
        {
            return RewriteResult.Ignore;
        }

        switch (rewriteContext.AnalysisType)
        {
            case AnalysisType.Builders:
                {
                    if (methodSymbol.ReceiverType.IsIMongoQueryable() ||
                        methodSymbol.ReturnType.IsIMongoQueryable() ||
                        methodSymbol.IsBuilderMethod() ||
                        methodSymbol.IsFindFluentMethod())
                    {
                        return RewriteResult.Ignore;
                    }
                    break;
                }
            case AnalysisType.Linq:
                {
                    if (methodSymbol.ReceiverType.IsIQueryable() ||
                        methodSymbol.ReturnType.IsIQueryable())
                    {
                        return RewriteResult.Ignore;
                    }

                    break;
                }
        }

        if (IsChildOfLambdaOrQueryOrBuildersParameter(rewriteContext, identifierNode, symbolInfo))
        {
            return RewriteResult.Ignore;
        }

        var typeSymbol = typeInfo.ConvertedType ?? methodSymbol.ReturnType;
        var nodeToReplace = SyntaxFactoryUtilities.ResolveAccessExpressionNode(identifierNode);
        var replacementNode = GetConstantReplacementNode(
            rewriteContext,
            typeSymbol,
            nodeToReplace.ToString());

        if (replacementNode == null)
        {
            return RewriteResult.Invalid;
        }

        return new RewriteResult(nodeToReplace, replacementNode);
    }

    private static RewriteResult HandleRemappedType(
        RewriteContext rewriteContext,
        SyntaxNode identifierNode,
        TypeInfo typeInfo,
        bool processGenericTypes = false)
    {
        var remappedType = processGenericTypes && identifierNode is GenericNameSyntax ?
            rewriteContext.TypesProcessor.ProcessTypeSymbol(typeInfo.Type) :
            rewriteContext.TypesProcessor.GetTypeSymbolToGeneratedTypeMapping(typeInfo.Type);

        var result = remappedType != null ?
            new(identifierNode, SyntaxFactory.IdentifierName(remappedType)) :
            RewriteResult.Invalid;

        return result;
    }

    private static bool IsChildOfLambdaOrQueryOrBuildersParameter(
        RewriteContext rewriteContext,
        SyntaxNode identifier,
        SymbolInfo symbolInfo)
    {
        if (symbolInfo.Symbol.IsContainedInLambdaOrQueryParameter(rewriteContext.Expression))
        {
            return true;
        }

        var underlyingIdentifier = SyntaxFactoryUtilities.GetUnderlyingIdentifier(identifier);
        if (underlyingIdentifier == null)
        {
            return false;
        }

        if (rewriteContext.AnalysisType == AnalysisType.Builders &&
            underlyingIdentifier.Identifier.Text == "Builders")
        {
            return true;
        }

        var result = rewriteContext.SemanticModel
            .GetSymbolInfo(underlyingIdentifier)
            .Symbol?
            .IsContainedInLambdaOrQueryParameter(rewriteContext.Expression) == true;

        return result;
    }

    private static GenericNameSyntax ProcessGenericType(RewriteContext rewriteContext, GenericNameSyntax genericNameSyntax)
    {
        var typeInfo = rewriteContext.SemanticModel.GetTypeInfo(genericNameSyntax);
        var remappedType = rewriteContext.TypesProcessor.ProcessTypeSymbol(typeInfo.Type);

        if (remappedType == null)
        {
            return null;
        }

        var typeArguments = new List<TypeSyntax>();

        foreach (var typeArgument in genericNameSyntax.TypeArgumentList.Arguments)
        {
            TypeSyntax typeSyntax;

            if (typeArgument is GenericNameSyntax nestedGenericNameSyntax)
            {
                typeSyntax = ProcessGenericType(rewriteContext, nestedGenericNameSyntax);

                if (typeSyntax == null)
                {
                    return null;
                }
            }
            else
            {
                var typeArgumentTypeInfo = rewriteContext.SemanticModel.GetTypeInfo(typeArgument);
                var typeArgumentRemappedType = rewriteContext.TypesProcessor.ProcessTypeSymbol(typeArgumentTypeInfo.Type);

                if (typeArgumentRemappedType == null)
                {
                    return null;
                }

                typeSyntax = SyntaxFactory.IdentifierName(typeArgumentRemappedType);
            }

            typeArguments.Add(typeSyntax);
        }

        var newGenericNameSyntax = SyntaxFactory.GenericName(
            SyntaxFactory.Identifier(remappedType),
            SyntaxFactory.TypeArgumentList(SyntaxFactory.SeparatedList(typeArguments)));

        return newGenericNameSyntax;
    }

    private static RewriteResult RemoveNonFluentParameters(
        SyntaxNode syntaxNode,
        SymbolInfo symbolInfo,
        TypeInfo typeInfo)
    {
        var typeSymbol = symbolInfo.Symbol.Kind switch
        {
            SymbolKind.Field => (symbolInfo.Symbol as IFieldSymbol).Type,
            SymbolKind.Method => (symbolInfo.Symbol as IMethodSymbol).ReturnType,
            SymbolKind.NamedType => symbolInfo.Symbol as INamedTypeSymbol,
            SymbolKind.Local or
            SymbolKind.Parameter or
            SymbolKind.Property => typeInfo.Type,
            _ => null
        };

        if (typeSymbol.IsFindOptions())
        {
            var argumentNode = syntaxNode.GetParentArgumentSyntaxIfExists();
            if (argumentNode == null)
            {
                return RewriteResult.Invalid;
            }

            return new(argumentNode, SyntaxFactoryUtilities.NewFindOptionsArgument);
        }

        return null;
    }

    private static RewriteResult SubstituteExpressionWithConstant(
        RewriteContext rewriteContext,
        SyntaxNode simpleNameSyntax,
        SymbolInfo symbolInfo,
        TypeInfo typeInfo)
    {
        if (IsChildOfLambdaOrQueryOrBuildersParameter(rewriteContext, simpleNameSyntax, symbolInfo) ||
            simpleNameSyntax.IsMemberOfAnonymousObject() ||
            typeInfo.Type == null)
        {
            return RewriteResult.Ignore;
        }

        var nodeToReplace = SyntaxFactoryUtilities.ResolveAccessExpressionNode(simpleNameSyntax);
        var type = rewriteContext.AnalysisType == AnalysisType.Builders ? typeInfo.Type : typeInfo.ConvertedType;

        var replacementNode = GetConstantReplacementNode(
            rewriteContext,
            type,
            nodeToReplace.ToString());

        if (replacementNode == null)
        {
            return RewriteResult.Invalid;
        }

        return new(nodeToReplace, replacementNode);
    }
}
