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

namespace MongoDB.Analyzer.Core.Builders;

internal static class BuilderExpressionProcessor
{
    private enum NodeType
    {
        Unknown = 0,
        Invalid,
        Builders,
        Fluent
    }

    public static ExpressionsAnalysis ProcessSemanticModel(MongoAnalysisContext context)
    {
        var semanticModel = context.SemanticModelAnalysisContext.SemanticModel;
        var syntaxTree = semanticModel.SyntaxTree;
        var root = syntaxTree.GetRoot();

        var analysisContexts = new List<ExpressionAnalysisContext>();
        var invalidExpressionNodes = new List<InvalidExpressionAnalysisNode>();

        var typesProcessor = context.TypesProcessor;
        var nodesProcessed = new HashSet<SyntaxNode>();
        var buildersToAnalysisContextMap = context.Settings.EnableVariableTracking ? new Dictionary<SyntaxNode, ExpressionAnalysisContext>() : null;

        foreach (var node in root.DescendantNodesWithSkipList(nodesProcessed))
        {
            SyntaxNode collectionNode = null;
            var (nodeType, namedType, expressionNode) = GetNodeType(semanticModel, node);

            switch (nodeType)
            {
                case NodeType.Builders:
                    {
                        break;
                    }
                case NodeType.Fluent:
                    {
                        collectionNode = expressionNode
                            .NestedInvocations()
                            .FirstOrDefault(n => semanticModel.GetTypeInfo(n).Type.IsSupportedIMongoCollection());
                        break;
                    }
                case NodeType.Invalid:
                    {
                        nodesProcessed.Add(node);
                        continue;
                    }
                default:
                    {
                        continue;
                    }
            }

            nodesProcessed.Add(node);

            try
            {
                foreach (var typeArgument in namedType.TypeArguments)
                {
                    typesProcessor.ProcessTypeSymbol(typeArgument);
                }

                var (newBuildersExpression, constantsMapper) = RewriteBuildersExpression(expressionNode, typesProcessor, semanticModel, collectionNode);

                if (newBuildersExpression != null)
                {
                    var expressionContext = new ExpressionAnalysisContext(new ExpressionAnalysisNode(
                        expressionNode,
                        typesProcessor.GetTypeSymbolToGeneratedTypeMapping(namedType.TypeArguments.First()),
                        newBuildersExpression,
                        constantsMapper,
                        expressionNode.GetLocation()));

                    analysisContexts.Add(expressionContext);
                    buildersToAnalysisContextMap?.Add(expressionNode, expressionContext);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed analyzing {node.NormalizeWhitespace()} with {ex.Message}");
            }
        }

        if (context.Settings.EnableVariableTracking)
        {
            try
            {
                analysisContexts = BuildersVariablesResolver.ResolveVariables(analysisContexts, buildersToAnalysisContextMap, semanticModel);
            }
            catch (Exception ex)
            {
                context.Logger.Log($"Failed resolving variables with {ex.Message}.");
            }
        }

        var builderAnalysis = new ExpressionsAnalysis()
        {
            AnalysisNodeContexts = analysisContexts.ToArray(),
            InvalidExpressionNodes = invalidExpressionNodes.ToArray(),
            TypesDeclarations = typesProcessor.TypesDeclarations
        };

        context.Logger.Log($"Builders: Found {builderAnalysis.AnalysisNodeContexts.Length} expressions.");

        return builderAnalysis;
    }

    private static (NodeType NodeType, INamedTypeSymbol NamedSymbol, SyntaxNode ExpressionNode) GetNodeType(SemanticModel semanticModel, SyntaxNode node)
    {
        var expressionNode = node;
        var nodeType = NodeType.Unknown;

        if (expressionNode is AssignmentExpressionSyntax assignmentExpressionSyntax)
        {
            expressionNode = assignmentExpressionSyntax.Right;
        }

        expressionNode = expressionNode.TrimParenthesis();

        if (expressionNode is not InvocationExpressionSyntax &&
            expressionNode is not BinaryExpressionSyntax)
        {
            return default;
        }

        if (semanticModel.GetTypeInfo(expressionNode).Type is not INamedTypeSymbol namedType ||
            namedType.TypeArguments.Length == 0)
        {
            return default;
        }

        if (namedType.IsFindFluent())
        {
            nodeType = NodeType.Fluent;
        }
        else if (namedType.IsBuilderDefinition())
        {
            nodeType = NodeType.Builders;

            if (expressionNode is BinaryExpressionSyntax binaryExpressionSyntax)
            {
                var childNodeType = GetNodeType(semanticModel, binaryExpressionSyntax.Left);

                if (childNodeType.NodeType == NodeType.Builders)
                {
                    childNodeType = GetNodeType(semanticModel, binaryExpressionSyntax.Right);

                    if (childNodeType.NodeType == NodeType.Builders)
                    {
                        return (nodeType, namedType, expressionNode);
                    }
                }

                return default;
            }

            foreach (var invocationNode in expressionNode.DescendantNodesAndSelf().OfType<InvocationExpressionSyntax>())
            {
                if (semanticModel.GetSymbolInfo(invocationNode).Symbol is not IMethodSymbol methodSymbol ||
                    methodSymbol.ReturnType.IsBuilderDefinition() && !methodSymbol.IsBuilderMethod())
                {
                    return default;
                }
            }
        }
        else
        {
            return default;
        }

        if (namedType.TypeArguments.Any(t => !t.IsSupportedBuilderType()))
        {
            return (NodeType.Invalid, default, default);
        }

        return (nodeType, namedType, expressionNode);
    }

    private static RewriteResult HandleRemappedType(
        RewriteContext rewriteContext,
        SimpleNameSyntax simpleNameNode,
        TypeInfo typeInfo)
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

    private static (SyntaxNode RewrittenLinqExpression, ConstantsMapper ConstantsMapper) RewriteBuildersExpression(
       SyntaxNode buildersExpressionNode,
       TypesProcessor typesProcessor,
       SemanticModel semanticModel,
       SyntaxNode collectionNode = null)
    {
        var rewriteContext = new RewriteContext(AnalysisType.Builders, buildersExpressionNode, semanticModel, typesProcessor, new ConstantsMapper());

        var nodesRemapping = new Dictionary<SyntaxNode, SyntaxNode>();
        var nodeProcessed = new HashSet<SyntaxNode>();

        if (collectionNode != null)
        {
            nodesRemapping.Add(collectionNode, SyntaxFactory.IdentifierName(MqlGeneratorSyntaxElements.Builders.CollectionName));
            nodeProcessed.Add(collectionNode);
        }

        foreach (var literalSyntax in buildersExpressionNode.DescendantNodes().OfType<LiteralExpressionSyntax>())
        {
            rewriteContext.ConstantsMapper.RegisterLiteral(literalSyntax);
        }

        rewriteContext.ConstantsMapper.FinalizeLiteralsRegistration();


        foreach (var identifierNode in buildersExpressionNode.DescendantNodesWithSkipList(nodeProcessed).OfType<SimpleNameSyntax>())
        {
            if (identifierNode == collectionNode ||
                !identifierNode.IsLeaf() ||
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
            var typeInfo = rewriteContext.SemanticModel.GetTypeInfo(nodeToHandle);

            var rewriteResult = RemoveNonFluentParams(nodeToHandle, symbolInfo, typeInfo);
            if (rewriteResult == null)
            {
                rewriteResult = symbolInfo.Symbol.Kind switch
                {
                    SymbolKind.Field => HandleField(rewriteContext, nodeToHandle, symbolInfo, typeInfo),
                    SymbolKind.Method => HandleMethod(rewriteContext, nodeToHandle, symbolInfo, typeInfo),
                    SymbolKind.NamedType => HandleRemappedType(rewriteContext, identifierNode, typeInfo),
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
                        if (symbolInfo.Symbol.Kind == SymbolKind.NamedType)
                        {
                            nodeProcessed.Add(rewriteResult.NodeToReplace);
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
}
