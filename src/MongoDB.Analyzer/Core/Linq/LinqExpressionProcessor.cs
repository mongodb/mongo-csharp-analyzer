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

namespace MongoDB.Analyzer.Core.Linq;

internal static class LinqExpressionProcessor
{
    private record RewriteContext(
        SyntaxNode LinqExpression,
        SemanticModel SemanticModel,
        TypesProcessor TypesProcessor,
        ConstantsMapper ConstantsMapper);

    private enum RewriteAction
    {
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

        public static RewriteResult Ignore = new(RewriteAction.Ignore, null, null);
        public static RewriteResult Invalid = new(RewriteAction.Invalid, null, null);
    }

    public static ExpressionsAnalysis ProcessSemanticModel(MongoAnalyzerContext context)
    {
        var semanticModel = context.SemanticModelAnalysisContext.SemanticModel;
        var syntaxTree = semanticModel.SyntaxTree;
        var root = syntaxTree.GetRoot();

        var processedSyntaxNodes = new HashSet<SyntaxNode>();
        var analysisContexts = new List<ExpressionAnalysisContext>();
        var invalidExpressionNodes = new List<InvalidExpressionAnalysisNode>();

        var typesProcessor = new TypesProcessor();

        Func<SyntaxNode, bool> descendToChildrenPredicate = n =>
        {
            return !processedSyntaxNodes.Contains(n.Parent);
        };

        foreach (var node in root.DescendantNodes(descendToChildrenPredicate).OfType<InvocationExpressionSyntax>())
        {
            var methodSymbol = semanticModel.GetSymbolInfo(node).Symbol as IMethodSymbol;

            if (!methodSymbol.IsDefinedInMongoLinq() ||
                !methodSymbol.ReceiverType.IsIMongoQueryable() ||
                !methodSymbol.ReturnType.IsIMongoQueryable())
            {
                continue;
            }

            processedSyntaxNodes.Add(node);

            // Find the deepest node that supplies IMongoQueryable symbol type
            var deepestMongoQueryableNode = GetNextNestedInvocation(node);

            while (deepestMongoQueryableNode != null)
            {
                var currentMethodSymbol = semanticModel.GetSymbolInfo(deepestMongoQueryableNode).Symbol as IMethodSymbol;

                if (currentMethodSymbol?.ReducedFrom?.ReceiverType.IsMongoQueryable() != true)
                    break;

                deepestMongoQueryableNode = GetNextNestedInvocation(deepestMongoQueryableNode);
            }

            // Validate IMongoQueryable node candidate
            if (deepestMongoQueryableNode == null)
                continue;

            var mongoQueryableTypeInfo = semanticModel.GetTypeInfo(deepestMongoQueryableNode);
            if (!mongoQueryableTypeInfo.Type.IsIMongoQueryable() ||
                mongoQueryableTypeInfo.Type is not INamedTypeSymbol mongoQueryableNamedType ||
                mongoQueryableNamedType.TypeArguments.Length != 1 ||
                !mongoQueryableNamedType.TypeArguments[0].IsSupportedMongoCollectionType())
            {
                continue;
            }

            try
            {
                if (PreanalyzeLinqExpression(node, semanticModel, invalidExpressionNodes))
                {
                    var generatedMongoQueryableTypeName = typesProcessor.ProcessTypeSymbol(mongoQueryableNamedType.TypeArguments[0]);

                    var (newLinqExpression, constantsMapper) = RewriteLinqExpression(node, deepestMongoQueryableNode, typesProcessor, semanticModel);

                    if (newLinqExpression != null)
                    {
                        var linqContext = new ExpressionAnalysisContext(new ExpressionAnalysisNode(
                            node,
                            generatedMongoQueryableTypeName,
                            newLinqExpression,
                            constantsMapper));

                        analysisContexts.Add(linqContext);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed analyzing {node.NormalizeWhitespace()} with {ex.Message}");
            }
        }

        var linqAnalysis = new ExpressionsAnalysis()
        {
            AnalysisNodeContexts = analysisContexts.ToArray(),
            InvalidExpressionNodes = invalidExpressionNodes.ToArray(),
            TypesDeclarations = typesProcessor.TypesDeclarations
        };

        context.Logger.Log($"Linq: Found {linqAnalysis.AnalysisNodeContexts.Length} expressions");

        return linqAnalysis;
    }

    private static ExpressionSyntax GetNextNestedInvocation(ExpressionSyntax expressionSyntax) =>
        ((expressionSyntax as InvocationExpressionSyntax)?.Expression as MemberAccessExpressionSyntax)?.Expression;

    private static bool PreanalyzeLinqExpression(SyntaxNode linqExpressionNode, SemanticModel semanticModel, List<InvalidExpressionAnalysisNode> invalidLinqExpressionNodes)
    {
        var result = true;

        foreach (var lambdaExpression in linqExpressionNode.DescendantNodes().OfType<SimpleLambdaExpressionSyntax>())
        {
            foreach (var methodInvocation in lambdaExpression.DescendantNodes().OfType<InvocationExpressionSyntax>())
            {
                var symbolInfo = semanticModel.GetSymbolInfo(methodInvocation);

                // find methods referencing lambda parameter
                if (symbolInfo.Symbol is IMethodSymbol methodSymbol)
                {
                    foreach (var arg in methodInvocation.ArgumentList.Arguments)
                    {
                        var underlyingNode = SyntaxFactoryUtilities.GetUnderlyingNameSyntax(arg.Expression);
                        if (underlyingNode == null)
                            continue;

                        var argSymbol = semanticModel.GetSymbolInfo(underlyingNode).Symbol;

                        if (argSymbol.IsContainedInLambda(linqExpressionNode))
                        {
                            invalidLinqExpressionNodes.Add(new InvalidExpressionAnalysisNode(
                                methodInvocation,
                                LinqAnalysisErrorMessages.MethodInvocationNotSupported));

                            result = false;
                            break;
                        }
                    }
                }
            }
        }

        return result;
    }

    private static (SyntaxNode RewrittenLinqExpression, ConstantsMapper ConstantsMapper) RewriteLinqExpression(
        SyntaxNode linqExpressionNode,
        SyntaxNode deepestMongoQueryableNode,
        TypesProcessor typesProcessor,
        SemanticModel semanticModel)
    {
        // Temporary catch, this method should not throw, but does not handle all cases yet
        try
        {
            var rewriteContext = new RewriteContext(linqExpressionNode, semanticModel, typesProcessor, new ConstantsMapper());
            var result = linqExpressionNode;

            var queryableNode = SyntaxFactory.IdentifierName(MqlGeneratorSyntaxElements.QueryableVarialbeName);
            var nodesRemapping = new Dictionary<SyntaxNode, SyntaxNode>()
            {
                { deepestMongoQueryableNode, queryableNode }
            };

            foreach (var literalSyntax in linqExpressionNode.DescendantNodes(n => n != deepestMongoQueryableNode).OfType<LiteralExpressionSyntax>())
            {
                if (!literalSyntax.Parent.IsKind(SyntaxKind.Argument))
                {
                    rewriteContext.ConstantsMapper.RegisterLiteral(literalSyntax);
                }
            }

            rewriteContext.ConstantsMapper.FinalizeLiteralsRegistration();

            var nodeProcessed = new List<SyntaxNode>();
            var lambdaIdentifiers = linqExpressionNode
                .DescendantNodes(n => n != deepestMongoQueryableNode)
                .OfType<IdentifierNameSyntax>()
                .Where(identifierNode =>
                {
                    var symbolInfo = semanticModel.GetSymbolInfo(identifierNode);
                    return symbolInfo.Symbol != null && IsChildOfLambdaParameter(rewriteContext, identifierNode, symbolInfo);
                })
                .ToArray();

            foreach (var identifierNode in linqExpressionNode.DescendantNodes(n => n != deepestMongoQueryableNode).OfType<IdentifierNameSyntax>())
            {
                if (identifierNode == deepestMongoQueryableNode ||
                    !identifierNode.IsLeaf() ||
                    nodeProcessed.Any(e => e.Contains(identifierNode)))
                {
                    continue;
                }

                var nodeToHandle = SyntaxNodeExtensions.GetTopMostInvocationOrBinaryExpressionSyntax(identifierNode, lambdaIdentifiers);
                if (nodeToHandle != identifierNode)
                {
                    nodeProcessed.Add(nodeToHandle);
                }

                var symbolInfo = semanticModel.GetSymbolInfo(identifierNode);

                var rewriteResult = symbolInfo.Symbol.Kind switch
                {
                    SymbolKind.Field => HandleField(rewriteContext, nodeToHandle, symbolInfo),
                    SymbolKind.Method => HandleMethod(rewriteContext, nodeToHandle, symbolInfo),
                    SymbolKind.NamedType => HandleRemappedType(rewriteContext, identifierNode),
                    SymbolKind.Local or
                    SymbolKind.Parameter or
                    SymbolKind.Property => SubstituteExpressionWithConst(rewriteContext, nodeToHandle, symbolInfo),
                    _ => RewriteResult.Ignore
                };

                switch (rewriteResult.RewriteAction)
                {
                    case RewriteAction.Rewrite:
                        nodesRemapping[rewriteResult.NodeToReplace] = rewriteResult.NewNode;
                        break;
                    case RewriteAction.Invalid:
                        return (null, null);
                }
            }

            result = linqExpressionNode.ReplaceNodes(nodesRemapping.Keys, (n, _) => nodesRemapping[n]);

            return (result, rewriteContext.ConstantsMapper);
        }
        catch
        {
            return (null, null);
        }
    }

    private static RewriteResult HandleRemappedType(
        RewriteContext rewriteContext,
        SimpleNameSyntax identifierNode)
    {
        var typeInfo = rewriteContext.SemanticModel.GetTypeInfo(identifierNode);
        var remmapedType = rewriteContext.TypesProcessor.GetTypeSymbolToGeneratedTypeMapping(typeInfo.Type);

        if (remmapedType == null)
            return RewriteResult.Invalid;

        SyntaxNode nodeToReplace = identifierNode;
        var identifierName = identifierNode.Identifier.Text;

        if (typeInfo.Type.TypeKind == TypeKind.Enum)
        {
            if (nodeToReplace.Parent is MemberAccessExpressionSyntax memberAccessExpressionSyntax)
            {
                nodeToReplace = memberAccessExpressionSyntax;
                identifierName = memberAccessExpressionSyntax.Name.Identifier.Text;
            }
            else
            {
                return RewriteResult.Ignore;
            }
        }
        else
        {
            while (nodeToReplace.Parent.IsKind(SyntaxKind.SimpleMemberAccessExpression))
            {
                nodeToReplace = nodeToReplace.Parent;
            }
        }

        SyntaxNode newNode = identifierNode.Parent.Kind() switch
        {
            SyntaxKind.SimpleMemberAccessExpression => SyntaxFactoryUtilities.SimpleMemberAccess(remmapedType, identifierName),
            _ => SyntaxFactory.IdentifierName(remmapedType)
        };

        return new RewriteResult(nodeToReplace, newNode);
    }

    private static RewriteResult HandleField(
        RewriteContext rewriteContext,
        SyntaxNode identifierNode,
        SymbolInfo symbolInfo)
    {
        var fieldSymbol = symbolInfo.Symbol as IFieldSymbol;
        if (fieldSymbol?.HasConstantValue != true)
        {
            return SubstituteExpressionWithConst(rewriteContext, identifierNode, symbolInfo);
        }

        if (!identifierNode.Parent.IsKind(SyntaxKind.SimpleMemberAccessExpression))
            return RewriteResult.Ignore;

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
        SymbolInfo symbolInfo)
    {
        var methodSymbol = symbolInfo.Symbol as IMethodSymbol;
        if (methodSymbol.ReceiverType.IsIMongoQueryable() ||
           methodSymbol.ReturnType.IsIMongoQueryable() ||
           methodSymbol.ReturnType == null ||
           IsChildOfLambdaParameter(rewriteContext, identifierNode, symbolInfo))
        {
            return RewriteResult.Ignore;
        }

        var nodeToReplace = SyntaxFactoryUtilities.ResolveAccessExpressionNode(identifierNode);
        var replacementNode = GetConstantReplacementNode(rewriteContext, methodSymbol.ReturnType, nodeToReplace.ToString());

        if (replacementNode == null)
        {
            return RewriteResult.Invalid;
        }

        return new RewriteResult(nodeToReplace, replacementNode);
    }

    private static SyntaxNode GetEnumCastNode(ITypeSymbol typeSymbol, object constantValue, TypesProcessor typesProcessor)
    {
        var remappedEnumTypeName = typesProcessor.GetTypeSymbolToGeneratedTypeMapping(typeSymbol);

        if (remappedEnumTypeName.IsNullOrWhiteSpace())
            return null;

        return SyntaxFactoryUtilities.GetCastConstantExpression(remappedEnumTypeName, constantValue);
    }

    private static SyntaxNode GetConstantReplacementNode(
        RewriteContext rewriteContext,
        ITypeSymbol typeSymbol,
        string fullName = null)
    {
        SyntaxNode replacementNode = null;

        if (typeSymbol.TypeKind == TypeKind.Enum)
        {
            var underlyingEnumType = (typeSymbol as INamedTypeSymbol).EnumUnderlyingType.SpecialType;

            var literalSyntax = rewriteContext.ConstantsMapper.GetExpressionByType(underlyingEnumType, fullName);
            replacementNode = GetEnumCastNode(typeSymbol, literalSyntax.Token.Value, rewriteContext.TypesProcessor);
        }
        else if (typeSymbol.SpecialType != SpecialType.None)
        {
            replacementNode = rewriteContext.ConstantsMapper.GetExpressionByType(typeSymbol.SpecialType, fullName);
        }

        return replacementNode;
    }

    private static bool IsChildOfLambdaParameter(
        RewriteContext rewriteContext,
        SyntaxNode identifier,
        SymbolInfo symbolInfo)
    {
        if (symbolInfo.Symbol.IsContainedInLambda(rewriteContext.LinqExpression))
            return true;

        var underlyingIdetifier = SyntaxFactoryUtilities.GetUnderlyingIdentifier(identifier);
        if (underlyingIdetifier == null)
            return false;

        if (rewriteContext.SemanticModel.GetSymbolInfo(underlyingIdetifier).Symbol.IsContainedInLambda(rewriteContext.LinqExpression))
            return true;

        return false;
    }

    private static RewriteResult SubstituteExpressionWithConst(
        RewriteContext rewriteContext,
        SyntaxNode identifierNode,
        SymbolInfo symbolInfo)
    {
        if (IsChildOfLambdaParameter(rewriteContext, identifierNode, symbolInfo) ||
            identifierNode.IsMemberOfAnonymousObject())
            return RewriteResult.Ignore;

        var typeInfo = rewriteContext.SemanticModel.GetTypeInfo(identifierNode);

        if (typeInfo.Type == null)
            return RewriteResult.Ignore;

        var nodeToReplace = SyntaxFactoryUtilities.ResolveAccessExpressionNode(identifierNode);
        var replacementNode = GetConstantReplacementNode(rewriteContext, typeInfo.Type, nodeToReplace.ToString());

        if (replacementNode == null)
        {
            return RewriteResult.Invalid;
        }

        return new RewriteResult(nodeToReplace, replacementNode);
    }
}
