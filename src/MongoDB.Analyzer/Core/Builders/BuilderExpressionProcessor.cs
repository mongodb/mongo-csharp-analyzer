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

namespace MongoDB.Analyzer.Core.Builders;

internal static class BuilderExpressionProcessor
{
    private record RewriteContext(
        SyntaxNode BuildersExpression,
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

        var nodesProcessed = new HashSet<SyntaxNode>();

        var filterNodes = new List<(SyntaxNode, ISymbol)>();
        var declaredNodes = new List<(SyntaxNode, ISymbol)>();
        var variableNodes = new List<(SyntaxNode, ISymbol)>();
        var builderToVariableNodeMapping = new Dictionary<SyntaxNode, List<SyntaxNode>>();

        // Find builders expressions
        // TODO skip children iterations
        foreach (var node in root.DescendantNodes(n => !nodesProcessed.Contains(n.Parent)))
        {
            if (nodesProcessed.Contains(node.Parent))
            {
                continue;
            }

            var (isValid, namedType, builderExpressionNode) = IsValidBuildersExpression(semanticModel, node);

            if (!isValid)
            {
                continue;
            }

            nodesProcessed.Add(node);

            try
            {
                foreach (var typeArgument in namedType.TypeArguments)
                {
                    typesProcessor.ProcessTypeSymbol(typeArgument);
                }

                var (newBuildersExpression, constantsMapper) = RewriteBuildersExpression(node, typesProcessor, semanticModel);

                if (newBuildersExpression != null)
                {
                    var expresionContext = new ExpressionAnalysisContext(new ExpressionAnalysisNode(
                        builderExpressionNode,
                        typesProcessor.GetTypeSymbolToGeneratedTypeMapping(namedType.TypeArguments.First()),
                        newBuildersExpression,
                        constantsMapper));

                    analysisContexts.Add(expresionContext);
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

        context.Logger.Log($"Builders: Found {linqAnalysis.AnalysisNodeContexts.Length} expressions.");

        return linqAnalysis;
    }

    private static (bool IsValid, INamedTypeSymbol, SyntaxNode) IsValidBuildersExpression(SemanticModel semanticModel, SyntaxNode node)
    {
        var builderExpressionNode = node;
        if (builderExpressionNode is AssignmentExpressionSyntax assignmentExpressionSyntax)
        {
            builderExpressionNode = assignmentExpressionSyntax.Right;
        }

        while (builderExpressionNode is ParenthesizedExpressionSyntax parenthesizedExpressionSyntax)
        {
            builderExpressionNode = parenthesizedExpressionSyntax.Expression;
        }

        if (builderExpressionNode is not InvocationExpressionSyntax &&
            builderExpressionNode is not BinaryExpressionSyntax)
        {
            return default;
        }

        if (semanticModel.GetTypeInfo(builderExpressionNode).Type is not INamedTypeSymbol namedType ||
            namedType.TypeArguments.Length == 0 ||
            !namedType.IsBuilderDefinition())
        {
            return default;
        }

        if (builderExpressionNode is BinaryExpressionSyntax binaryExpressionSyntax)
        {
            var leftValid = IsValidBuildersExpression(semanticModel, binaryExpressionSyntax.Left);
            var rightValid = IsValidBuildersExpression(semanticModel, binaryExpressionSyntax.Right);

            if (leftValid.IsValid && rightValid.IsValid)
            {
                return (true, namedType, builderExpressionNode);
            }
            else
            {
                return default;
            }
        }

        foreach (var invocationNode in builderExpressionNode.DescendantNodesAndSelf().OfType<InvocationExpressionSyntax>())
        {
            if (semanticModel.GetSymbolInfo(invocationNode).Symbol is not IMethodSymbol methodSymbol ||
                methodSymbol.ReturnType.IsBuilderDefinition() && !methodSymbol.IsBuilderMethod())
            {
                return default;
            }
        }

        if (namedType.TypeArguments.Any(t => !t.IsSupportedBuilderType()))
        {
            return default;
        }

        return (true, namedType, builderExpressionNode);
    }

    private static (SyntaxNode RewrittenLinqExpression, ConstantsMapper ConstantsMapper) RewriteBuildersExpression(
       SyntaxNode buildersExpressionNode,
       TypesProcessor typesProcessor,
       SemanticModel semanticModel)
    {
        var rewriteContext = new RewriteContext(buildersExpressionNode, semanticModel, typesProcessor, new ConstantsMapper());

        var nodesRemapping = new Dictionary<SyntaxNode, SyntaxNode>();

        foreach (var literalSyntax in buildersExpressionNode.DescendantNodes().OfType<LiteralExpressionSyntax>())
        {
            rewriteContext.ConstantsMapper.RegisterLiteral(literalSyntax);
        }

        rewriteContext.ConstantsMapper.FinalizeLiteralsRegistration();

        var nodeProcessed = new List<SyntaxNode>();

        foreach (var identifierNode in buildersExpressionNode.DescendantNodes().OfType<SimpleNameSyntax>())
        {
            if (!identifierNode.IsLeaf() ||
                nodeProcessed.Any(e => e.Contains(identifierNode)))
            {
                continue;
            }

            var nodeToHandle = SyntaxNodeExtensions.GetTopMostInvocationOrBinaryExpressionSyntax(identifierNode, null);
            if (nodeToHandle != identifierNode)
            {
                nodeProcessed.Add(nodeToHandle);
            }

            var symbolInfo = semanticModel.GetSymbolInfo(nodeToHandle);

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
                    {
                        if (nodesRemapping == null)
                        {
                            nodesRemapping = new Dictionary<SyntaxNode, SyntaxNode>();
                        }

                        nodesRemapping[rewriteResult.NodeToReplace] = rewriteResult.NewNode;
                        break;
                    }
                case RewriteAction.Invalid:
                    return (null, null);
                default:
                    continue;
            }
        }

        var result = buildersExpressionNode.ReplaceNodes(
            nodesRemapping.Keys,
            (n, _) => nodesRemapping[n]);

        return (result, rewriteContext.ConstantsMapper);
    }

    private static RewriteResult HandleRemappedType(
        RewriteContext rewriteContext,
        SimpleNameSyntax simpleNameNode)
    {
        if (simpleNameNode is GenericNameSyntax genericNameSyntax)
        {
            var genericTypeInfo = rewriteContext.SemanticModel.GetTypeInfo(genericNameSyntax);
            var genericRemappedType = rewriteContext.TypesProcessor.ProcessTypeSymbol(genericTypeInfo.Type);

            if (genericRemappedType == null)
            {
                return RewriteResult.Invalid;
            }

            return new RewriteResult(simpleNameNode, SyntaxFactory.IdentifierName(genericRemappedType));
        }

        var typeInfo = rewriteContext.SemanticModel.GetTypeInfo(simpleNameNode);
        var remappedType = rewriteContext.TypesProcessor.GetTypeSymbolToGeneratedTypeMapping(typeInfo.Type);

        if (remappedType == null)
        {
            return RewriteResult.Invalid;
        }

        SyntaxNode nodeToReplace = simpleNameNode;
        var identifierName = simpleNameNode.Identifier.Text;

        if (simpleNameNode is GenericNameSyntax)
        {
            nodeToReplace = simpleNameNode;
        }
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

        SyntaxNode newNode = simpleNameNode.Parent.Kind() switch
        {
            SyntaxKind.SimpleMemberAccessExpression => SyntaxFactoryUtilities.SimpleMemberAccess(remappedType, identifierName),
            _ => SyntaxFactory.IdentifierName(remappedType)
        };

        return new RewriteResult(nodeToReplace, newNode);
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

    private static RewriteResult HandleField(
        RewriteContext rewriteContext,
        SyntaxNode simpleNameSyntax,
        SymbolInfo symbolInfo)
    {
        var fieldSymbol = symbolInfo.Symbol as IFieldSymbol;
        if (fieldSymbol?.HasConstantValue != true)
        {
            return SubstituteExpressionWithConst(rewriteContext, simpleNameSyntax, symbolInfo);
        }

        if (!simpleNameSyntax.Parent.IsKind(SyntaxKind.SimpleMemberAccessExpression))
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

        return new RewriteResult(simpleNameSyntax.Parent, replacementNode);
    }

    private static RewriteResult HandleMethod(
        RewriteContext rewriteContext,
        SyntaxNode simpleNameSyntax,
        SymbolInfo symbolInfo)
    {
        var methodSymbol = symbolInfo.Symbol as IMethodSymbol;
        if (methodSymbol.ReceiverType.IsIMongoQueryable() ||
           methodSymbol.ReturnType.IsIMongoQueryable() ||
           methodSymbol.ReturnType == null ||
           IsChildOfLambdaParameterOrBuilders(rewriteContext, simpleNameSyntax, symbolInfo))
        {
            return RewriteResult.Ignore;
        }

        var nodeToReplace = SyntaxFactoryUtilities.ResolveAccessExpressionNode(simpleNameSyntax);
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
        {
            return null;
        }

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

    private static bool IsChildOfLambdaParameterOrBuilders(
        RewriteContext rewriteContext,
        SyntaxNode simpleNameSyntax,
        SymbolInfo symbolInfo)
    {
        if (symbolInfo.Symbol.IsContainedInLambda(rewriteContext.BuildersExpression))
        {
            return true;
        }

        var underlyingIdetifier = SyntaxFactoryUtilities.GetUnderlyingIdentifier(simpleNameSyntax);
        if (underlyingIdetifier == null)
        {
            return false;
        }

        if (underlyingIdetifier.Identifier.Text == "Builders" ||
            rewriteContext.SemanticModel.GetSymbolInfo(underlyingIdetifier).Symbol.IsContainedInLambda(rewriteContext.BuildersExpression))
        {
            return true;
        }

        return false;
    }

    private static RewriteResult SubstituteExpressionWithConst(
        RewriteContext rewriteContext,
        SyntaxNode simpleNameSyntax,
        SymbolInfo symbolInfo)
    {
        if (IsChildOfLambdaParameterOrBuilders(rewriteContext, simpleNameSyntax, symbolInfo))
        {
            return RewriteResult.Ignore;
        }

        var typeInfo = rewriteContext.SemanticModel.GetTypeInfo(simpleNameSyntax);

        if (typeInfo.Type == null)
        {
            return RewriteResult.Ignore;
        }

        var nodeToReplace = SyntaxFactoryUtilities.ResolveAccessExpressionNode(simpleNameSyntax);
        var replacementNode = GetConstantReplacementNode(rewriteContext, typeInfo.Type, nodeToReplace.ToString());

        if (replacementNode == null)
        {
            return RewriteResult.Invalid;
        }

        return new RewriteResult(nodeToReplace, replacementNode);
    }
}
