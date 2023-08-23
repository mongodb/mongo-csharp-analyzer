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

namespace MongoDB.Analyzer.Core.Linq;

internal static class LinqExpressionProcessor
{
    public static ExpressionsAnalysis ProcessSemanticModel(MongoAnalysisContext context)
    {
        var semanticModel = context.SemanticModelAnalysisContext.SemanticModel;
        var syntaxTree = semanticModel.SyntaxTree;
        var root = syntaxTree.GetRoot();

        var processedSyntaxNodes = new HashSet<SyntaxNode>();
        var analysisContexts = new List<ExpressionAnalysisContext>();
        var invalidExpressionNodes = new List<InvalidExpressionAnalysisNode>();

        var typesProcessor = context.TypesProcessor;

        foreach (var node in root.DescendantNodesWithSkipList(processedSyntaxNodes).OfType<ExpressionSyntax>())
        {
            var deepestMongoQueryableNode = node;

            if (node is QueryExpressionSyntax queryNode)
            {
                var expression = queryNode.FromClause.Expression;
                var queryMethodSymbol = semanticModel.GetTypeInfo(expression).Type;
                if (!queryMethodSymbol.IsIMongoQueryable())
                {
                    continue;
                }

                deepestMongoQueryableNode = queryNode.FromClause.Expression;
            }
            else if (node is InvocationExpressionSyntax invocationNode)
            {
                var methodSymbol = invocationNode.GetMethodSymbol(semanticModel);

                if (!methodSymbol.IsDefinedInMongoLinqOrSystemLinq() ||
                    !methodSymbol.ReceiverType.IsIQueryable() ||
                    !methodSymbol.ReturnType.IsIQueryable())
                {
                    continue;
                }

                deepestMongoQueryableNode = invocationNode
                    .NestedInvocations()
                    .FirstOrDefault(n =>
                        {
                            var currentMethodSymbol = n.GetMethodSymbol(semanticModel);

                            // Find the first method that is not receiving IQueryable or is not defined in System.Linq or MongoDB.Driver.Linq
                            return !((currentMethodSymbol?.ReceiverType).IsIQueryable() && currentMethodSymbol.IsDefinedInMongoLinqOrSystemLinq());
                        });
            }
            else
            {
                continue;
            }

            processedSyntaxNodes.Add(node);

            // Validate IMongoQueryable node candidate
            if (deepestMongoQueryableNode == null)
            {
                continue;
            }

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
                            constantsMapper,
                            node.GetLocation()));

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

    private static bool PreanalyzeLinqExpression(SyntaxNode linqExpressionNode, SemanticModel semanticModel, List<InvalidExpressionAnalysisNode> invalidLinqExpressionNodes)
    {
        var result = true;

        foreach (var expression in linqExpressionNode.DescendantNodes().Where(node => node is QueryClauseSyntax || node is SimpleLambdaExpressionSyntax))
        {
            foreach (var methodInvocation in expression.DescendantNodes().OfType<InvocationExpressionSyntax>())
            {
                var symbolInfo = semanticModel.GetSymbolInfo(methodInvocation);

                // find methods referencing lambda parameter
                if (symbolInfo.Symbol is IMethodSymbol methodSymbol)
                {
                    foreach (var arg in methodInvocation.ArgumentList.Arguments)
                    {
                        var underlyingNode = SyntaxFactoryUtilities.GetUnderlyingNameSyntax(arg.Expression);
                        if (underlyingNode == null)
                        {
                            continue;
                        }

                        var argSymbol = semanticModel.GetSymbolInfo(underlyingNode).Symbol;

                        if (argSymbol.IsContainedInLambdaOrQueryParameter(linqExpressionNode))
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
            var rewriteContext = new RewriteContext(AnalysisType.Linq, linqExpressionNode, semanticModel, typesProcessor, new ConstantsMapper());
            var result = linqExpressionNode;

            var queryableNode = SyntaxFactory.IdentifierName(MqlGeneratorSyntaxElements.Linq.QueryableName);
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
            var lambdaAndQueryIdentifiers = linqExpressionNode
                .DescendantNodes(n => n != deepestMongoQueryableNode)
                .OfType<IdentifierNameSyntax>()
                .Where(identifierNode =>
                    {
                        var symbolInfo = semanticModel.GetSymbolInfo(identifierNode);
                        return symbolInfo.Symbol != null && IsChildOfLambdaOrQueryOrBuildersParameter(rewriteContext, identifierNode, symbolInfo);
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

                var nodeToHandle = SyntaxNodeExtensions.GetTopMostInvocationOrBinaryExpressionSyntax(identifierNode, lambdaAndQueryIdentifiers);
                if (nodeToHandle != identifierNode)
                {
                    nodeProcessed.Add(nodeToHandle);
                }

                var symbolInfo = semanticModel.GetSymbolInfo(identifierNode);
                var typeInfo = rewriteContext.SemanticModel.GetTypeInfo(nodeToHandle);

                if (symbolInfo.Symbol == null)
                {
                    return default;
                }

                var rewriteResult = symbolInfo.Symbol.Kind switch
                {
                    SymbolKind.Field => HandleField(rewriteContext, nodeToHandle, symbolInfo, typeInfo),
                    SymbolKind.Method => HandleMethod(rewriteContext, nodeToHandle, symbolInfo, typeInfo),
                    SymbolKind.NamedType => HandleRemappedType(rewriteContext, identifierNode),
                    SymbolKind.Local or
                    SymbolKind.Parameter or
                    SymbolKind.Property => SubstituteExpressionWithConstant(rewriteContext, nodeToHandle, symbolInfo, typeInfo),
                    _ => RewriteResult.Ignore
                };

                switch (rewriteResult.RewriteAction)
                {
                    case RewriteAction.Rewrite:
                        nodesRemapping[rewriteResult.NodeToReplace] = rewriteResult.NewNode;
                        break;
                    case RewriteAction.Invalid:
                        return default;
                }
            }

            result = linqExpressionNode.ReplaceNodes(nodesRemapping.Keys, (n, _) => nodesRemapping[n]);

            return (result, rewriteContext.ConstantsMapper);
        }
        catch
        {
            return default;
        }
    }

    private static RewriteResult HandleRemappedType(
        RewriteContext rewriteContext,
        SimpleNameSyntax identifierNode)
    {
        var typeInfo = rewriteContext.SemanticModel.GetTypeInfo(identifierNode);
        var remappedType = rewriteContext.TypesProcessor.GetTypeSymbolToGeneratedTypeMapping(typeInfo.Type);
        var result = remappedType != null ? new(identifierNode, SyntaxFactory.IdentifierName(remappedType)) : RewriteResult.Invalid;

        return result;
    }
}
